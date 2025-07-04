namespace CyberSecurityBot
{
    public class TaskItem
    {
        public string Title       { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime? Reminder { get; set; }
        public bool Completed     { get; set; }

        public override string ToString()
            => $"{(Completed ? "[✔]" : "[ ]")} {Title}" +
               (Reminder.HasValue ? $" (remind {Reminder.Value:g})" : "");
    }

    public class QuizQuestion
    {
        public string   Question     { get; set; } = string.Empty;
        public string[] Options      { get; set; } = Array.Empty<string>();
        public int      CorrectIndex { get; set; }
        public string   Explanation  { get; set; } = string.Empty;
    }
}
