using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using CyberSecurityBot.Services;

namespace CyberSecurityBot.Services.Impl
{
    /// <summary>
    /// Wraps KnowledgeBase and adds history, memory, follow-ups and sentiment.
    /// </summary>
    public class ChatService : IChatService
    {
        private readonly List<string> _history = new();
        private readonly Dictionary<string,string> _memory = new(StringComparer.OrdinalIgnoreCase);
        private string? _lastCategory;
        private string? _lastAnswer;

        public IEnumerable<string> GetHistory() => _history.AsReadOnly();

        public void RememberTopic(string topic)
        {
            _memory["favoriteTopic"] = topic;
        }

        public void ResetMemory()
        {
            _history.Clear();
            _memory.Clear();
            _lastCategory = _lastAnswer = null;
        }

        public (string Response, string Category) GetResponse(string userInput)
        {
            var input = userInput?.Trim() ?? "";
            _history.Add(input);

            // Follow-up: "more", "another", "next"
            if (_lastCategory != null 
             && Regex.IsMatch(input, @"\b(more|another|next)\b", RegexOptions.IgnoreCase))
            {
                var tips = KnowledgeBase.GetRelatedTips(_lastCategory);
                if (tips.Count > 0)
                {
                    var idx = _lastAnswer != null 
                              ? tips.IndexOf(_lastAnswer) 
                              : -1;
                    var next = (idx >= 0 && idx + 1 < tips.Count) 
                               ? tips[idx + 1] 
                               : tips[0];
                    _lastAnswer = next;
                    return (next, _lastCategory);
                }
            }

            // Memory declaration: "interested in X"
            if (Regex.IsMatch(input, @"\b(interested in|favorite topic)\b", RegexOptions.IgnoreCase))
            {
                // match category
                var cat = KnowledgeBase.GetAllCategories()
                          .FirstOrDefault(c => 
                              Regex.IsMatch(input, "\\b" + Regex.Escape(c) + "\\b", RegexOptions.IgnoreCase));
                if (cat != null)
                {
                    _memory["favoriteTopic"] = cat;
                    return ($"Got it—I’ll remember you like {cat}.", cat);
                }
            }

            // Core lookup
            var (answer, category) = KnowledgeBase.LookupWithCategory(input);
            _lastAnswer   = answer;
            _lastCategory = category;

            // Personalization
            if (_memory.TryGetValue("favoriteTopic", out var fav) 
             && !string.IsNullOrEmpty(category) 
             && category.Equals(fav, StringComparison.OrdinalIgnoreCase))
            {
                answer = $"As someone interested in {fav}, here’s another tip:\n{answer}";
            }

            return (answer, category);
        }
    }
}
