using System.ComponentModel;
using System.Windows.Forms;

namespace CyberSecurityBot
{
    class AddEntryDialog : Form
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Keyword { get; private set; } = string.Empty;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Answer  { get; private set; } = string.Empty;

        TextBox _kBox, _aBox;
        Button  _ok;

        public AddEntryDialog()
        {
            Text = "Add Q&A Entry";
            Width  = 400; Height = 200;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition  = FormStartPosition.CenterParent;

            _kBox = new TextBox { PlaceholderText = "Keyword", Dock = DockStyle.Top,    Margin = new Padding(10) };
            _aBox = new TextBox { PlaceholderText = "Answer",  Dock = DockStyle.Top,    Margin = new Padding(10) };
            _ok   = new Button  { Text = "OK", Dock = DockStyle.Bottom };

            _ok.Click += (s, e) =>
            {
                Keyword       = _kBox.Text;
                Answer        = _aBox.Text;
                DialogResult  = DialogResult.OK;
                Close();
            };

            Controls.AddRange(new Control[]{ _ok, _aBox, _kBox });
        }
    }
}

