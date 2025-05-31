using System;
using System.Drawing;
using System.Windows.Forms;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        private RichTextBox gameTextBox;
        private TextBox inputTextBox;
        private Button submitButton;
        private MenuStrip menuStrip;
        private StatusStrip statusStrip;
        private ToolStripStatusLabel statusLabel;
        private GameEngine gameEngine;

        public Form1()
        {
            InitializeComponent();
            InitializeGameUI();
            gameEngine = new GameEngine(this);
            gameEngine.StartNewGame();
        }

        private void InitializeGameUI()
        {
            this.Text = "RPG Text Adventure";
            this.Size = new Size(800, 600);
            this.MinimumSize = new Size(600, 400);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Normal;

            // Create menu strip
            CreateMenuStrip();

            // Create main game text display
            gameTextBox = new RichTextBox
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                BackColor = Color.Black,
                ForeColor = Color.LimeGreen,
                Font = new Font("Consolas", 10),
                ScrollBars = RichTextBoxScrollBars.Vertical,
                WordWrap = true
            };

            // Create input panel
            Panel inputPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 60,
                BackColor = Color.DarkGray
            };

            // Create input textbox
            inputTextBox = new TextBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Consolas", 10),
                Margin = new Padding(5),
                Multiline = false
            };
            inputTextBox.KeyDown += InputTextBox_KeyDown;

            // Create submit button
            submitButton = new Button
            {
                Text = "Submit",
                Dock = DockStyle.Right,
                Width = 80,
                BackColor = Color.DarkSlateGray,
                ForeColor = Color.White
            };
            submitButton.Click += SubmitButton_Click;

            // Create status strip
            CreateStatusStrip();

            // Add controls to input panel
            inputPanel.Controls.Add(inputTextBox);
            inputPanel.Controls.Add(submitButton);

            // Add controls to form in proper order for docking
            this.Controls.Add(gameTextBox);
            this.Controls.Add(inputPanel);
            this.Controls.Add(statusStrip);
            this.Controls.Add(menuStrip);

            // Set focus to input
            inputTextBox.Focus();
        }

        private void CreateMenuStrip()
        {
            menuStrip = new MenuStrip();

            // Game menu
            ToolStripMenuItem gameMenu = new ToolStripMenuItem("Game");
            gameMenu.DropDownItems.Add("New Game", null, (s, e) => gameEngine.StartNewGame());
            gameMenu.DropDownItems.Add("Save Game", null, (s, e) => gameEngine.SaveGame());
            gameMenu.DropDownItems.Add("Load Game", null, (s, e) => gameEngine.LoadGame());
            gameMenu.DropDownItems.Add(new ToolStripSeparator());
            gameMenu.DropDownItems.Add("Exit", null, (s, e) => this.Close());

            // Character menu
            ToolStripMenuItem characterMenu = new ToolStripMenuItem("Character");
            characterMenu.DropDownItems.Add("View Stats", null, (s, e) => gameEngine.ShowCharacterStats());
            characterMenu.DropDownItems.Add("Inventory", null, (s, e) => gameEngine.ShowInventory());

            // Settings menu
            ToolStripMenuItem settingsMenu = new ToolStripMenuItem("Settings");
            settingsMenu.DropDownItems.Add("Change Theme", null, (s, e) => ChangeTheme());
            settingsMenu.DropDownItems.Add("Font Size", null, (s, e) => ChangeFontSize());

            // Help menu
            ToolStripMenuItem helpMenu = new ToolStripMenuItem("Help");
            helpMenu.DropDownItems.Add("Commands", null, (s, e) => gameEngine.ShowHelp());
            helpMenu.DropDownItems.Add("About", null, (s, e) => ShowAbout());

            menuStrip.Items.AddRange(new ToolStripItem[] { gameMenu, characterMenu, settingsMenu, helpMenu });
        }

        private void CreateStatusStrip()
        {
            statusStrip = new StatusStrip();
            statusLabel = new ToolStripStatusLabel("Ready to adventure...");
            statusStrip.Items.Add(statusLabel);
        }

        private void InputTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ProcessInput();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Tab)
            {
                gameEngine.ShowInventory();
                e.Handled = true;
            }
            else if (e.Control && e.KeyCode == Keys.S)
            {
                gameEngine.SaveGame();
                e.Handled = true;
            }
            else if (e.Control && e.KeyCode == Keys.L)
            {
                gameEngine.LoadGame();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.F1)
            {
                gameEngine.ShowHelp();
                e.Handled = true;
            }
        }

        private void SubmitButton_Click(object sender, EventArgs e)
        {
            ProcessInput();
        }

        private void ProcessInput()
        {
            string input = inputTextBox.Text.Trim();
            if (!string.IsNullOrEmpty(input))
            {
                DisplayText($"> {input}", Color.Yellow);
                gameEngine.ProcessCommand(input);
                inputTextBox.Clear();
            }
        }

        public void DisplayText(string text, Color? color = null)
        {
            if (gameTextBox.InvokeRequired)
            {
                gameTextBox.Invoke(new Action(() => DisplayText(text, color)));
                return;
            }

            gameTextBox.SelectionStart = gameTextBox.TextLength;
            gameTextBox.SelectionLength = 0;
            gameTextBox.SelectionColor = color ?? Color.LimeGreen;
            gameTextBox.AppendText(text + Environment.NewLine);
            gameTextBox.ScrollToCaret();
        }

        public void UpdateStatus(string status)
        {
            if (statusLabel.Owner.InvokeRequired)
            {
                statusLabel.Owner.Invoke(new Action(() => UpdateStatus(status)));
                return;
            }
            statusLabel.Text = status;
        }

        public void ClearScreen()
        {
            if (gameTextBox.InvokeRequired)
            {
                gameTextBox.Invoke(new Action(ClearScreen));
                return;
            }
            gameTextBox.Clear();
        }

        private void ChangeTheme()
        {
            using (var themeDialog = new ThemeSelectionDialog())
            {
                if (themeDialog.ShowDialog() == DialogResult.OK)
                {
                    ApplyTheme(themeDialog.SelectedTheme);
                }
            }
        }

        private void ApplyTheme(GameTheme theme)
        {
            switch (theme)
            {
                case GameTheme.Classic:
                    gameTextBox.BackColor = Color.Black;
                    gameTextBox.ForeColor = Color.LimeGreen;
                    break;
                case GameTheme.Modern:
                    gameTextBox.BackColor = Color.White;
                    gameTextBox.ForeColor = Color.Black;
                    break;
                case GameTheme.Fantasy:
                    gameTextBox.BackColor = Color.DarkSlateBlue;
                    gameTextBox.ForeColor = Color.Gold;
                    break;
            }
        }

        private void ChangeFontSize()
        {
            using (var fontDialog = new FontDialog())
            {
                fontDialog.Font = gameTextBox.Font;
                if (fontDialog.ShowDialog() == DialogResult.OK)
                {
                    gameTextBox.Font = fontDialog.Font;
                    inputTextBox.Font = fontDialog.Font;
                }
            }
        }

        private void ShowAbout()
        {
            MessageBox.Show(
                "RPG Text Adventure v1.0\n\n" +
                "A classic text-based RPG built with WinForms\n" +
                "Navigate through mystical lands and forge your legend!\n\n" +
                "Built with .NET 9.0",
                "About RPG Text Adventure",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (gameEngine?.HasUnsavedChanges == true)
            {
                var result = MessageBox.Show(
                    "You have unsaved progress. Do you want to save before exiting?",
                    "Unsaved Changes",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    gameEngine.SaveGame();
                }
                else if (result == DialogResult.Cancel)
                {
                    e.Cancel = true;
                    return;
                }
            }
            base.OnFormClosing(e);
        }
    }

    public enum GameTheme
    {
        Classic,
        Modern,
        Fantasy
    }
}
