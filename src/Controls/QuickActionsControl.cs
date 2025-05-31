using System;
using System.Drawing;
using System.Windows.Forms;

namespace WinFormsApp1.Controls
{
    public partial class QuickActionsControl : UserControl
    {
        private TableLayoutPanel actionsLayout;
        
        public event EventHandler<string> ActionClicked;

        public QuickActionsControl()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Size = new Size(300, 200);
            this.BackColor = Color.Transparent;

            actionsLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 3,
                Padding = new Padding(5)
            };

            // Set equal column and row styles for uniform button sizing
            actionsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            actionsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            actionsLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 33.33F));
            actionsLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 33.33F));
            actionsLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 33.34F));

            CreateDefaultButtons();
            this.Controls.Add(actionsLayout);
        }

        private void CreateDefaultButtons()
        {
            AddActionButton("Look", Color.DarkCyan, 0, 0);
            AddActionButton("Stats", Color.DarkMagenta, 1, 0);
            AddActionButton("Save", Color.DarkOrange, 0, 1);
            AddActionButton("Load", Color.DarkSlateBlue, 1, 1);
            AddActionButton("Help", Color.DarkGoldenrod, 0, 2);
            AddActionButton("Exit", Color.DarkRed, 1, 2);
        }

        public void AddActionButton(string text, Color backgroundColor, int column, int row)
        {
            Button actionButton = new Button
            {
                Text = text,
                Dock = DockStyle.Fill,
                BackColor = backgroundColor,
                ForeColor = Color.White,
                Margin = new Padding(2),
                FlatStyle = FlatStyle.Flat,
                Tag = text.ToLower() // Store action identifier
            };

            actionButton.FlatAppearance.BorderColor = Color.DarkGray;
            actionButton.Click += ActionButton_Click;

            actionsLayout.Controls.Add(actionButton, column, row);
        }

        private void ActionButton_Click(object sender, EventArgs e)
        {
            if (sender is Button button && button.Tag is string action)
            {
                ActionClicked?.Invoke(this, action);
            }
        }

        public void SetButtonEnabled(string buttonText, bool enabled)
        {
            foreach (Control control in actionsLayout.Controls)
            {
                if (control is Button button && 
                    string.Equals(button.Text, buttonText, StringComparison.OrdinalIgnoreCase))
                {
                    button.Enabled = enabled;
                    break;
                }
            }
        }

        public void SetAllButtonsEnabled(bool enabled)
        {
            foreach (Control control in actionsLayout.Controls)
            {
                if (control is Button button)
                {
                    button.Enabled = enabled;
                }
            }
        }

        public void SetButtonsEnabled(bool enabled, params string[] buttonTexts)
        {
            foreach (string buttonText in buttonTexts)
            {
                SetButtonEnabled(buttonText, enabled);
            }
        }

        public void ClearButtons()
        {
            actionsLayout.Controls.Clear();
        }

        public void SetButtonColor(string buttonText, Color backgroundColor)
        {
            foreach (Control control in actionsLayout.Controls)
            {
                if (control is Button button && 
                    string.Equals(button.Text, buttonText, StringComparison.OrdinalIgnoreCase))
                {
                    button.BackColor = backgroundColor;
                    break;
                }
            }
        }
    }
} 