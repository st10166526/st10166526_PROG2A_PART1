using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using CyberSecurityBot;
using CyberSecurityBot.Services;
using CyberSecurityBot.Services;


namespace CyberSecurityBot.Services.Impl
{
    public class TaskService : ITaskService, IDisposable
    {
        private readonly string _dataFile;
        private readonly BindingList<TaskItem> _tasks;
        private readonly System.Threading.Timer _timer;
        private readonly object _lock = new();

        public event EventHandler<TaskItem>? ReminderDue;
        public BindingList<TaskItem> Tasks => _tasks;

        public TaskService()
        {
            _dataFile = Path.Combine(AppContext.BaseDirectory, "tasks.json");
            _tasks = new BindingList<TaskItem>();
            _tasks.ListChanged += (_,__) => SaveTasks();

            LoadTasks();

            // Check every minute for due reminders
            _timer = new System.Threading.Timer(_ => CheckReminders(), null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
        }

        private void LoadTasks()
        {
            if (!File.Exists(_dataFile)) return;

            try
            {
                var json = File.ReadAllText(_dataFile);
                var list = JsonSerializer.Deserialize<TaskItem[]>(json)
                           ?? Array.Empty<TaskItem>();
                foreach (var t in list)
                    _tasks.Add(t);
            }
            catch
            {
                // if load fails, start fresh
                _tasks.Clear();
            }
        }

        private void SaveTasks()
        {
            lock (_lock)
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                var json = JsonSerializer.Serialize(_tasks.ToList(), options);
                File.WriteAllText(_dataFile, json);
            }
        }

        public void AddTask(TaskItem t)
        {
            lock (_lock)
            {
                _tasks.Add(t);
            }
        }

        public void DeleteTask(TaskItem t)
        {
            lock (_lock)
            {
                _tasks.Remove(t);
            }
        }

        public void CompleteTask(TaskItem t)
        {
            lock (_lock)
            {
                var item = _tasks.FirstOrDefault(x => x == t);
                if (item != null)
                {
                    item.Completed = true;
                    SaveTasks();
                }
            }
        }

        private void CheckReminders()
        {
            var due = _tasks
                .Where(t => t.Reminder.HasValue && !t.Completed && t.Reminder <= DateTime.Now)
                .ToList();

            foreach (var t in due)
            {
                ReminderDue?.Invoke(this, t);
                t.Reminder = null;
            }

            if (due.Count > 0)
                SaveTasks();
        }

        public void Dispose()
        {
            _timer.Dispose();
        }
    }
}

