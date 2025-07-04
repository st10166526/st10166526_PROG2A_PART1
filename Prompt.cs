using System;
using System.Windows.Forms;

namespace CyberSecurityBot
{
    public static class Prompt
    {
        public static string ShowDialog(string text, string caption)
        {
            using var prompt = new Form()
            {
                Width = 400,
                Height = 150,
                Text = caption,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterParent,
                MaximizeBox = false,
                MinimizeBox = false
            };

            var lbl = new Label { Left = 20, Top = 20, Text = text, Width = 340 };
            var tb  = new TextBox { Left = 20, Top = 50, Width = 340 };
            var ok  = new Button  { Text = "OK", Left = 280, Width = 80, Top = 80, DialogResult = DialogResult.OK };

            ok.Click += (s, e) => prompt.Close();
            prompt.Controls.AddRange(new Control[]{ lbl, tb, ok });
            prompt.AcceptButton = ok;

            return prompt.ShowDialog() == DialogResult.OK ? tb.Text : "Guest";
        }
    }
}
