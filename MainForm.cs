using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Runtime.Versioning;
using System.Threading;
using System.Windows.Forms;
using System.ComponentModel;
using CyberSecurityBot.Services;
using CyberSecurityBot.Services.Impl;

namespace CyberSecurityBot
{
    public class MainForm : Form
    {
        // Services
        private readonly IChatService _chatSvc;
        private readonly ITaskService _taskSvc;
        private readonly IQuizService _quizSvc;

        // Menu & Status
        private MenuStrip _menu;
        private StatusStrip _status;
        private ToolStripStatusLabel _statusLabel, _clockLabel;

        // TabControl
        private TabControl _tabs;

        // Chat controls
        private RichTextBox _chatBox;
        private TextBox     _chatInput;
        private Button      _chatSend;

        // Tasks controls
        private DataGridView    _taskGrid;
        private FlowLayoutPanel _taskToolbar;

        // Quiz controls
        private Label        _quizHeader;
        private ProgressBar  _quizProgress;
        private Label        _quizQuestionLabel;
        private GroupBox     _quizGroup;
        private RadioButton[] _quizOptions;
        private Button       _quizAction;

        // Activity Log
        private ListBox _logList;

        // Quiz state
        private QuizQuestion? _currentQuiz;
        private int _totalQuestions;

        public MainForm()
        {
            // instantiate services
            _chatSvc = new ChatService();
            _taskSvc = new TaskService();
            _quizSvc = new QuizService(Path.Combine(AppContext.BaseDirectory, "questions.csv"));
            _taskSvc.ReminderDue += OnReminderDue;

            // form basics
            Text = "CyberSecurity Awareness Chatbot";
            MinimumSize = new Size(800, 600);
            Size = new Size(900, 650);
            StartPosition = FormStartPosition.CenterScreen;
            Font = new Font("Segoe UI", 9F);

            // build UI
            BuildMenu();
            MainMenuStrip = _menu;

            BuildStatusBar();
            BuildClockTimer();

            try
            {
                BuildTabs();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Failed to build tabs:\n{ex.Message}\n\n{ex.StackTrace}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }

            // startup flow
            PlayGreetingWithText("Hello! Welcome to the Cybersecurity Awareness Bot. I'm here to keep you safe online.");
            DisplayWelcome();
            AskName();
        }

        #region Menu / StatusStrip

        private void BuildMenu()
        {
            _menu = new MenuStrip();
            var file = new ToolStripMenuItem("File");
            file.DropDownItems.Add("Exit", null, (_,__) => Close());
            var help = new ToolStripMenuItem("Help");
            help.DropDownItems.Add("About", null, ShowAbout);
            _menu.Items.AddRange(new[] { file, help });
            Controls.Add(_menu);
        }

        private void ShowAbout(object s, EventArgs e)
        {
            MessageBox.Show(
                "CyberSecurity Awareness Chatbot\nVersion 1.0",
                "About",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }

        private void BuildStatusBar()
        {
            _status = new StatusStrip();
            _statusLabel = new ToolStripStatusLabel("Ready");
            _clockLabel  = new ToolStripStatusLabel { Spring = true, TextAlign = ContentAlignment.MiddleRight };
            _status.Items.AddRange(new ToolStripItem[] { _statusLabel, _clockLabel });
            Controls.Add(_status);
        }

        private void BuildClockTimer()
        {
            var timer = new System.Windows.Forms.Timer { Interval = 1000 };
            timer.Tick += (_,__) => _clockLabel.Text = DateTime.Now.ToString("HH:mm:ss");
            timer.Start();
        }

        private void UpdateStatus(string text)
            => _statusLabel.Text = text;

        #endregion

        #region Tabs

        private void BuildTabs()
        {
            _tabs = new TabControl { Dock = DockStyle.Fill };
            _tabs.SelectedIndexChanged += (_,__) =>
                UpdateStatus($"Viewing: {_tabs.SelectedTab.Text}");

            // Add all four tabs
            _tabs.TabPages.Add(CreateChatTab());
            _tabs.TabPages.Add(CreateTasksTab());
            _tabs.TabPages.Add(CreateQuizTab());
            _tabs.TabPages.Add(CreateLogTab());

            Controls.Add(_tabs);
            _tabs.BringToFront();
        }

        #endregion

        #region Chat Tab

        private TabPage CreateChatTab()
        {
            var tab = new TabPage("Chat");
            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10),
                RowCount = 2
            };
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 85));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 15));

            _chatBox = new RichTextBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Consolas", 10F),
                ReadOnly = true,
                BackColor = Color.White
            };

            var inputPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2
            };
            inputPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 80));
            inputPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));

            _chatInput = new TextBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10F),
                ForeColor = Color.Gray,
                Text = "Type a message..."
            };
            _chatInput.GotFocus += (s,e) =>
            {
                if (_chatInput.Text == "Type a message...")
                {
                    _chatInput.Clear();
                    _chatInput.ForeColor = Color.Black;
                }
            };
            _chatInput.LostFocus += (s,e) =>
            {
                if (string.IsNullOrWhiteSpace(_chatInput.Text))
                {
                    _chatInput.ForeColor = Color.Gray;
                    _chatInput.Text = "Type a message...";
                }
            };

            _chatSend = CreateAccentButton("Send");
            _chatSend.Click += ChatSend_Click;

            inputPanel.Controls.Add(_chatInput, 0, 0);
            inputPanel.Controls.Add(_chatSend, 1, 0);

            layout.Controls.Add(_chatBox, 0, 0);
            layout.Controls.Add(inputPanel, 0, 1);
            tab.Controls.Add(layout);
            return tab;
        }

        private void ChatSend_Click(object sender, EventArgs e)
        {
            var text = _chatInput.Text.Trim();
            if (string.IsNullOrEmpty(text) || text == "Type a message...") return;

            _chatInput.Clear();
            _chatBox.SelectionColor = Color.Green;
            _chatBox.AppendText($"You: {text}\n");
            _chatBox.ScrollToCaret();

            var (reply,_) = _chatSvc.GetResponse(text);
            _chatBox.SelectionColor = Color.Blue;
            _chatBox.AppendText($"Bot: {reply}\n\n");
            _chatBox.ScrollToCaret();

            Log($"Chat: {reply}");
        }

        #endregion

        #region Tasks Tab

        private TabPage CreateTasksTab()
        {
            var tab = new TabPage("Tasks");
            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10),
                RowCount = 2
            };
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 85));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 15));

            _taskGrid = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AutoGenerateColumns = false,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                ColumnHeadersDefaultCellStyle = { BackColor = Color.FromArgb(33,150,243), ForeColor = Color.White },
                EnableHeadersVisualStyles = false,
                AlternatingRowsDefaultCellStyle = { BackColor = Color.WhiteSmoke }
            };
            _taskGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Title", DataPropertyName = "Title", Width = 200 });
            _taskGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Description", DataPropertyName = "Description", Width = 300 });
            _taskGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Reminder", DataPropertyName = "Reminder", Width = 200 });
            _taskGrid.Columns.Add(new DataGridViewCheckBoxColumn { HeaderText = "Done", DataPropertyName = "Completed", Width = 60 });
            _taskGrid.DataSource = _taskSvc.Tasks;

            _taskToolbar = new FlowLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(5) };
            var allBtn    = CreateAccentButton("All");
            var activeBtn = CreateAccentButton("Active");
            var doneBtn   = CreateAccentButton("Done");
            var addBtn    = CreateAccentButton("Add Task");
            var markBtn   = CreateAccentButton("Mark Done");
            var delBtn    = CreateAccentButton("Delete");

            allBtn.Click    += (_,__) => FilterTasks(null);
            activeBtn.Click += (_,__) => FilterTasks(false);
            doneBtn.Click   += (_,__) => FilterTasks(true);
            addBtn.Click    += OnAddTask;
            markBtn.Click   += OnCompleteTask;
            delBtn.Click    += OnDeleteTask;

            _taskToolbar.Controls.AddRange(new Control[]{ allBtn, activeBtn, doneBtn, addBtn, markBtn, delBtn });

            layout.Controls.Add(_taskGrid, 0, 0);
            layout.Controls.Add(_taskToolbar, 0, 1);
            tab.Controls.Add(layout);
            return tab;
        }

        private void FilterTasks(bool? completed)
        {
            foreach (DataGridViewRow row in _taskGrid.Rows)
            {
                if (row.DataBoundItem is TaskItem t)
                    row.Visible = completed == null || t.Completed == completed;
            }
            UpdateStatus($"Tasks filter: {(completed == null?"All":completed==true?"Done":"Active")}");
        }

        private void OnAddTask(object? sender, EventArgs e)
        {
            using var dlg = new AddTaskDialog();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                _taskSvc.AddTask(dlg.Result);
                Log($"Task added: {dlg.Result.Title}");
            }
        }

        private void OnCompleteTask(object? sender, EventArgs e)
        {
            if (_taskGrid.CurrentRow?.DataBoundItem is TaskItem t)
            {
                _taskSvc.CompleteTask(t);
                Log($"Task completed: {t.Title}");
                _taskGrid.Refresh();
            }
        }

        private void OnDeleteTask(object? sender, EventArgs e)
        {
            if (_taskGrid.CurrentRow?.DataBoundItem is TaskItem t)
            {
                _taskSvc.DeleteTask(t);
                Log($"Task deleted: {t.Title}");
            }
        }

        #endregion

        #region Quiz Tab

        private TabPage CreateQuizTab()
        {
            var tab = new TabPage("Quiz");
            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10),
                RowCount = 4
            };
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 20));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 60));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));

            _quizHeader   = new Label { Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft, Font = new Font("Segoe UI", 9F, FontStyle.Bold) };
            _quizProgress = new ProgressBar { Dock = DockStyle.Fill, Minimum = 0, Maximum = _quizSvc.Questions.Count, Value = 0 };

            _quizQuestionLabel = new Label
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter
            };

            _quizGroup = new GroupBox { Dock = DockStyle.Fill, Text = "Select an answer:" };
            _quizOptions = Enumerable.Range(0,4)
                .Select(_ => new RadioButton { Dock = DockStyle.Top, AutoSize = true, Padding=new Padding(5) })
                .ToArray();
            foreach (var rb in _quizOptions)
            {
                rb.CheckedChanged += (_,__) => _quizAction.Enabled = _quizOptions.Any(r=>r.Checked);
                _quizGroup.Controls.Add(rb);
            }

            _quizAction = CreateAccentButton("Start Quiz");
            _quizAction.Dock = DockStyle.Fill;
            _quizAction.Click += QuizAction;

            layout.Controls.Add(_quizHeader, 0, 0);
            layout.Controls.Add(_quizQuestionLabel, 0, 1);
            layout.Controls.Add(_quizGroup,     0, 2);
            layout.Controls.Add(_quizAction,    0, 3);

            tab.Controls.Add(layout);
            return tab;
        }

        private void QuizAction(object? sender, EventArgs e)
        {
            if (_quizAction.Text == "Start Quiz")
            {
                _totalQuestions = _quizSvc.Questions.Count;
                _quizProgress.Maximum = _totalQuestions;
                NextQuizQuestion();
                _quizAction.Text = "Submit";
            }
            else if (_currentQuiz != null)
            {
                int sel = Array.FindIndex(_quizOptions, rb => rb.Checked);
                bool correct = sel >= 0 && _quizSvc.SubmitAnswer(_currentQuiz, sel);

                MessageBox.Show(
                  (correct ? "✅ Correct!\n" : "❌ Wrong.\n") + _currentQuiz.Explanation,
                  "Quiz Feedback", MessageBoxButtons.OK,
                  correct ? MessageBoxIcon.Information : MessageBoxIcon.Warning
                );
                Log($"Quiz Q#{_quizProgress.Value}: {(correct?"correct":"wrong")}");
                NextQuizQuestion();
            }
        }

        private void NextQuizQuestion()
        {
            _currentQuiz = _quizSvc.NextQuestion();
            _quizProgress.Value++;
            _quizHeader.Text = $"Question {_quizProgress.Value} of {_totalQuestions}";
            _quizQuestionLabel.Text = _currentQuiz.Question;

            for (int i = 0; i < _quizOptions.Length; i++)
            {
                _quizOptions[i].Text = _currentQuiz.Options[i];
                _quizOptions[i].Checked = false;
            }
            _quizAction.Enabled = false;
        }

        #endregion

        #region Activity Log

        private TabPage CreateLogTab()
        {
            var tab = new TabPage("Activity Log");
            _logList = new ListBox { Dock = DockStyle.Fill, Font = new Font("Consolas", 9F) };
            tab.Controls.Add(_logList);
            return tab;
        }

        private void Log(string msg)
        {
            var entry = $"[{DateTime.Now:HH:mm}] {msg}";
            _logList.Items.Add(entry);
            if (_logList.Items.Count > 200)
                _logList.Items.RemoveAt(0);
        }

        #endregion

        #region Reminder Handler

        private void OnReminderDue(object? sender, TaskItem t)
        {
            BeginInvoke(new Action(() =>
            {
                MessageBox.Show($"🔔 Reminder: {t.Title}", "Reminder", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Log($"Reminder fired: {t.Title}");
            }));
        }

        #endregion

        #region Startup Sequence

        [SupportedOSPlatform("windows")]
        private void PlayGreetingWithText(string msg)
        {
            var thr = new Thread(() =>
            {
                try
                {
                    using var sp = new SoundPlayer(Path.Combine(AppContext.BaseDirectory, "greeting.wav"));
                    sp.PlaySync();
                }
                catch { }
            });
            thr.Start();
            _chatBox.SelectionFont = new Font("Consolas", 10F);
            _chatBox.SelectionColor = Color.Blue;
            _chatBox.AppendText("Bot: " + msg + "\n\n");
            thr.Join();
        }

        private void DisplayWelcome()
        {
            try
            {
                var art = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "ascii_logo.txt"));
                _chatBox.SelectionFont = new Font("Consolas", 10F);
                _chatBox.SelectionColor = Color.DarkBlue;
                _chatBox.AppendText(art + "\n\n");
            }
            catch
            {
                _chatBox.AppendText("=== Cybersecurity Awareness Bot ===\n\n");
            }
        }

        private void AskName()
        {
            _chatBox.SelectionColor = Color.Blue;
            _chatBox.AppendText("Bot: What is your name?\n");
            var name = Prompt.ShowDialog("Enter your name:", "Welcome");
            _chatSvc.RememberTopic(name);
            _chatBox.SelectionColor = Color.Blue;
            _chatBox.AppendText($"Bot: Hello {name}! Ask me anything or use /commands.\n\n");
            Log($"User name set to: {name}");
        }

        #endregion

        // accent‐colored button helper
        private Button CreateAccentButton(string text)
        {
            var btn = new Button
            {
                Text = text,
                BackColor = Color.FromArgb(33,150,243),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Margin = new Padding(5),
                AutoSize = true
            };
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }
    }
}
