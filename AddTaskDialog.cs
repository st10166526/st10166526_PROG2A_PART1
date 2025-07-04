using System;
using System.Windows.Forms;

namespace CyberSecurityBot
{
    class AddTaskDialog : Form
    {
        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public TaskItem? Result { get; private set; }
        TextBox _titleBox, _descBox;
        DateTimePicker _whenPicker;
        Button _okButton;

        public AddTaskDialog()
        {
            Text = "Add New Task";
            Width = 400; Height = 260;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition  = FormStartPosition.CenterParent;
            MaximizeBox    = false;
            MinimizeBox    = false;

            _titleBox   = new TextBox { PlaceholderText = "Title",       Dock = DockStyle.Top,    Margin = new Padding(10) };
            _descBox    = new TextBox { PlaceholderText = "Description", Dock = DockStyle.Top,    Height = 60, Multiline = true, Margin = new Padding(10) };
            _whenPicker = new DateTimePicker { Dock = DockStyle.Top, Format = DateTimePickerFormat.Custom, CustomFormat = "dd MMM yyyy  HH:mm", Margin = new Padding(10) };
            _okButton   = new Button { Text = "OK", DialogResult = DialogResult.OK, Dock = DockStyle.Bottom, Height = 30 };

            _okButton.Click += (s, e) =>
            {
                Result = new TaskItem
                {
                    Title       = _titleBox.Text,
                    Description = _descBox.Text,
                    Reminder    = _whenPicker.Value
                };
                Close();
            };

            Controls.AddRange(new Control[]{ _okButton, _whenPicker, _descBox, _titleBox });
        }
    }
}
