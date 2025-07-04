using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CyberSecurityBot.Services;

namespace CyberSecurityBot.Services.Impl
{
    public class QuizService : IQuizService
    {
        private List<QuizQuestion> _bank;
        private readonly Random _rng = new();
        private int _currentIndex = -1;
        public int CorrectCount { get; private set; }

        public IReadOnlyList<QuizQuestion> Questions => _bank;

        public QuizService(string csvPath)
{
    if (!File.Exists(csvPath))
        throw new FileNotFoundException("Quiz CSV not found", csvPath);

    var lines = File.ReadAllLines(csvPath)
                    .Where(l => !string.IsNullOrWhiteSpace(l))
                    .ToArray();

    // Optional: if you have a header row, skip it:
    // lines = lines.Skip(1).ToArray();

    var bank = new List<QuizQuestion>();
    foreach (var line in lines)
    {
        // Split into at most 7 pieces: 1 question + 4 options + correctIndex + explanation
        var parts = line.Split(',', 7, StringSplitOptions.None);
        if (parts.Length < 7)
        {
            // Log or ignore malformed line
            continue;
        }

        if (!int.TryParse(parts[5], out var idx) || idx < 0 || idx > 3)
        {
            // Skip if the correctIndex isn't 0–3
            continue;
        }

        bank.Add(new QuizQuestion
        {
            Question     = parts[0].Trim(),
            Options      = parts.Skip(1).Take(4).Select(o => o.Trim()).ToArray(),
            CorrectIndex = idx,
            Explanation  = parts[6].Trim()
        });
    }

    if (bank.Count == 0)
        throw new InvalidOperationException("No valid quiz questions loaded from CSV.");

    _bank = bank;
}

        public QuizQuestion NextQuestion()
        {
            if (_bank == null || _bank.Count == 0)
                throw new InvalidOperationException("No quiz questions loaded.");

            _currentIndex = (_currentIndex + 1) % _bank.Count;

            // Once we wrap around, shuffle the bank to get a new random order
            if (_currentIndex == 0)
                _bank = _bank.OrderBy(_ => _rng.Next()).ToList();

            return _bank[_currentIndex];
        }

        public bool SubmitAnswer(QuizQuestion question, int selectedIndex)
        {
            bool correct = selectedIndex == question.CorrectIndex;
            if (correct) CorrectCount++;
            return correct;
        }
    }
}
