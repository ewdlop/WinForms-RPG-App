using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.ComponentModel;

namespace WinFormsApp1.Controls
{
    public partial class GameInputControl : UserControl
    {
        private TextBox inputTextBox;
        private Button submitButton;
        private Button quickInventoryButton;
        private Label historyLabel;
        private ComboBox commandHistoryComboBox;
        private Button helpButton;

        private List<string> commandHistory;
        private int historyIndex;

        public event EventHandler<string> CommandSubmitted;
        public event EventHandler InventoryRequested;
        public event EventHandler HelpRequested;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool InputEnabled
        {
            get => inputTextBox.Enabled;
            set
            {
                inputTextBox.Enabled = value;
                submitButton.Enabled = value;
                commandHistoryComboBox.Enabled = value;
            }
        }

        public GameInputControl()
        {
            commandHistory = new List<string>();
            historyIndex = -1;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Size = new Size(600, 100);
            this.BackColor = Color.DarkGray;
            this.Padding = new Padding(5);

            // Main layout
            TableLayoutPanel mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 4,
                RowCount = 2,
                Padding = new Padding(2)
            };

            // Set column styles
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F)); // Input
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15F)); // Submit
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15F)); // Inventory
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F)); // History/Help

            // Set row styles
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 60F)); // Main input row
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 40F)); // History/help row

            // Create input controls
            CreateInputControls(mainLayout);

            this.Controls.Add(mainLayout);
        }

        private void CreateInputControls(TableLayoutPanel mainLayout)
        {
            // Command input textbox
            inputTextBox = new TextBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Consolas", 10),
                PlaceholderText = "Enter command...",
                BackColor = Color.White,
                ForeColor = Color.Black
            };
            inputTextBox.KeyDown += InputTextBox_KeyDown;
            mainLayout.Controls.Add(inputTextBox, 0, 0);

            // Submit button
            submitButton = new Button
            {
                Text = "Submit",
                Dock = DockStyle.Fill,
                BackColor = Color.DarkBlue,
                ForeColor = Color.White,
                Font = new Font("Arial", 9, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat
            };
            submitButton.Click += SubmitButton_Click;
            mainLayout.Controls.Add(submitButton, 1, 0);

            // Quick inventory button
            quickInventoryButton = new Button
            {
                Text = "Inventory",
                Dock = DockStyle.Fill,
                BackColor = Color.DarkGreen,
                ForeColor = Color.White,
                Font = new Font("Arial", 9, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat
            };
            quickInventoryButton.Click += QuickInventoryButton_Click;
            mainLayout.Controls.Add(quickInventoryButton, 2, 0);

            // Help button
            helpButton = new Button
            {
                Text = "Help (F1)",
                Dock = DockStyle.Fill,
                BackColor = Color.DarkOrange,
                ForeColor = Color.White,
                Font = new Font("Arial", 8, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat
            };
            helpButton.Click += HelpButton_Click;
            mainLayout.Controls.Add(helpButton, 3, 0);

            // Command history dropdown
            commandHistoryComboBox = new ComboBox
            {
                Dock = DockStyle.Fill,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Consolas", 8),
                BackColor = Color.LightGray
            };
            commandHistoryComboBox.SelectedIndexChanged += CommandHistoryComboBox_SelectedIndexChanged;
            mainLayout.Controls.Add(commandHistoryComboBox, 0, 1);

            // History label
            historyLabel = new Label
            {
                Text = "Use ↑↓ for command history | Recent commands →",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                ForeColor = Color.DarkBlue,
                Font = new Font("Arial", 8),
                AutoEllipsis = true
            };
            mainLayout.SetColumnSpan(historyLabel, 3);
            mainLayout.Controls.Add(historyLabel, 1, 1);

            // Add tooltips
            var toolTip = new ToolTip();
            toolTip.SetToolTip(inputTextBox, "Type game commands here. Use arrow keys for history.");
            toolTip.SetToolTip(submitButton, "Submit command (Enter)");
            toolTip.SetToolTip(quickInventoryButton, "Open inventory (Tab)");
            toolTip.SetToolTip(helpButton, "Show help (F1)");
            toolTip.SetToolTip(commandHistoryComboBox, "Select from recent commands");
        }

        private void InputTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    ProcessInput();
                    e.Handled = true;
                    break;

                case Keys.Up:
                    NavigateHistory(-1);
                    e.Handled = true;
                    break;

                case Keys.Down:
                    NavigateHistory(1);
                    e.Handled = true;
                    break;

                case Keys.Tab:
                    InventoryRequested?.Invoke(this, EventArgs.Empty);
                    e.Handled = true;
                    break;

                case Keys.F1:
                    HelpRequested?.Invoke(this, EventArgs.Empty);
                    e.Handled = true;
                    break;

                case Keys.Escape:
                    inputTextBox.Clear();
                    e.Handled = true;
                    break;
            }
        }

        private void SubmitButton_Click(object sender, EventArgs e)
        {
            ProcessInput();
        }

        private void QuickInventoryButton_Click(object sender, EventArgs e)
        {
            InventoryRequested?.Invoke(this, EventArgs.Empty);
        }

        private void HelpButton_Click(object sender, EventArgs e)
        {
            HelpRequested?.Invoke(this, EventArgs.Empty);
        }

        private void CommandHistoryComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (commandHistoryComboBox.SelectedItem != null)
            {
                inputTextBox.Text = commandHistoryComboBox.SelectedItem.ToString();
                inputTextBox.SelectionStart = inputTextBox.Text.Length;
                inputTextBox.Focus();
            }
        }

        private void ProcessInput()
        {
            string input = inputTextBox.Text.Trim();
            if (!string.IsNullOrEmpty(input))
            {
                AddToHistory(input);
                CommandSubmitted?.Invoke(this, input);
                inputTextBox.Clear();
                historyIndex = -1;
            }
        }

        private void AddToHistory(string command)
        {
            // Remove if already exists to avoid duplicates
            commandHistory.Remove(command);
            
            // Add to beginning of list
            commandHistory.Insert(0, command);
            
            // Keep only last 20 commands
            if (commandHistory.Count > 20)
            {
                commandHistory.RemoveAt(commandHistory.Count - 1);
            }

            // Update dropdown
            UpdateHistoryDropdown();
        }

        private void UpdateHistoryDropdown()
        {
            commandHistoryComboBox.Items.Clear();
            commandHistoryComboBox.Items.AddRange(commandHistory.Take(10).ToArray());
            
            // Update history label
            if (commandHistory.Any())
            {
                historyLabel.Text = $"Recent: {string.Join(", ", commandHistory.Take(3))}...";
            }
        }

        private void NavigateHistory(int direction)
        {
            if (!commandHistory.Any()) return;

            historyIndex += direction;
            
            if (historyIndex < 0)
            {
                historyIndex = -1;
                inputTextBox.Clear();
            }
            else if (historyIndex >= commandHistory.Count)
            {
                historyIndex = commandHistory.Count - 1;
            }

            if (historyIndex >= 0 && historyIndex < commandHistory.Count)
            {
                inputTextBox.Text = commandHistory[historyIndex];
                inputTextBox.SelectionStart = inputTextBox.Text.Length;
            }
        }

        public void FocusInput()
        {
            inputTextBox.Focus();
        }

        public void ClearInput()
        {
            inputTextBox.Clear();
        }

        public void SetInputText(string text)
        {
            inputTextBox.Text = text;
            inputTextBox.SelectionStart = inputTextBox.Text.Length;
        }

        public void ClearHistory()
        {
            commandHistory.Clear();
            UpdateHistoryDropdown();
            historyLabel.Text = "Use ↑↓ for command history | Recent commands →";
        }

        public void SetQuickButtonsEnabled(bool enabled)
        {
            quickInventoryButton.Enabled = enabled;
        }

        public void ApplyTheme(string themeName)
        {
            switch (themeName.ToLower())
            {
                case "classic":
                    inputTextBox.BackColor = Color.Black;
                    inputTextBox.ForeColor = Color.LimeGreen;
                    this.BackColor = Color.DarkGreen;
                    break;
                case "modern":
                    inputTextBox.BackColor = Color.White;
                    inputTextBox.ForeColor = Color.Black;
                    this.BackColor = Color.LightGray;
                    break;
                case "fantasy":
                    inputTextBox.BackColor = Color.DarkSlateBlue;
                    inputTextBox.ForeColor = Color.Gold;
                    this.BackColor = Color.Purple;
                    break;
                case "dark":
                    inputTextBox.BackColor = Color.DarkGray;
                    inputTextBox.ForeColor = Color.White;
                    this.BackColor = Color.Black;
                    break;
                case "high contrast":
                    inputTextBox.BackColor = Color.Black;
                    inputTextBox.ForeColor = Color.Yellow;
                    this.BackColor = Color.DarkBlue;
                    break;
                default:
                    inputTextBox.BackColor = Color.White;
                    inputTextBox.ForeColor = Color.Black;
                    this.BackColor = Color.DarkGray;
                    break;
            }
        }
    }
} 