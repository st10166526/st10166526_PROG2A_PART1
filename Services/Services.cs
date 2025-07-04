using System;
using System.Collections.Generic;
using System.ComponentModel;
using CyberSecurityBot;   // for TaskItem and QuizQuestion

namespace CyberSecurityBot.Services
{
    /// <summary>
    /// Chat/NLP memory, history and response interface
    /// </summary>
    public interface IChatService
    {
        (string Response, string Category) GetResponse(string userInput);
        IEnumerable<string> GetHistory();
        void RememberTopic(string topic);
        void ResetMemory();
    }

    /// <summary>
    /// Task management, reminders and persistence interface
    /// </summary>
    public interface ITaskService
    {
        /// <summary>
        /// Two-way data bound list of tasks for UI binding
        /// </summary>
        BindingList<TaskItem> Tasks { get; }

        void AddTask(TaskItem t);
        void DeleteTask(TaskItem t);
        void CompleteTask(TaskItem t);

        /// <summary>
        /// Fires when a reminder time is reached
        /// </summary>
        event EventHandler<TaskItem> ReminderDue;
    }

    /// <summary>
    /// Quiz engine interface: question bank, navigation, scoring
    /// </summary>
    public interface IQuizService
    {
        /// <summary>
        /// Read-only list of all loaded questions
        /// </summary>
        IReadOnlyList<QuizQuestion> Questions { get; }

        /// <summary>
        /// Move to the next question (wraps & reshuffles)
        /// </summary>
        QuizQuestion NextQuestion();

        /// <summary>
        /// Submit an answer for the current question; returns true if correct
        /// </summary>
        bool SubmitAnswer(QuizQuestion question, int selectedIndex);

        /// <summary>
        /// Total number of correct answers in this session
        /// </summary>
        int CorrectCount { get; }
    }
}
