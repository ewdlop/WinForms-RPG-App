using System;
using System.Drawing;
using System.Windows.Forms;

namespace WinFormsApp1.Controls
{
    public partial class GameTextDisplayControl : UserControl
    {
        private RichTextBox gameTextBox;
        private Panel headerPanel;
        private Label titleLabel;
        private Button clearButton;
        private Button scrollToTopButton;
        private Button scrollToBottomButton;

        public event EventHandler ClearRequested;
        public event EventHandler ScrollToTopRequested;
        public event EventHandler ScrollToBottomRequested;

        public GameTextDisplayControl()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Size = new Size(600, 400);
            this.BackColor = Color.Black;

            // Main layout
            TableLayoutPanel mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                Padding = new Padding(2)
            };

            // Set row styles
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Header
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); // Text area

            // Create header panel
            CreateHeaderPanel();
            mainLayout.Controls.Add(headerPanel, 0, 0);

            // Create main text display
            CreateTextDisplay();
            mainLayout.Controls.Add(gameTextBox, 0, 1);

            this.Controls.Add(mainLayout);
        }

        private void CreateHeaderPanel()
        {
            headerPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Height = 30,
                BackColor = Color.DarkSlateGray,
                Padding = new Padding(5, 2, 5, 2)
            };

            titleLabel = new Label
            {
                Text = "Game Console",
                Location = new Point(5, 5),
                Size = new Size(100, 20),
                ForeColor = Color.White,
                Font = new Font("Arial", 9, FontStyle.Bold)
            };

            clearButton = new Button
            {
                Text = "Clear",
                Size = new Size(50, 20),
                Location = new Point(headerPanel.Width - 160, 2),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                BackColor = Color.DarkRed,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Arial", 7)
            };
            clearButton.Click += ClearButton_Click;

            scrollToTopButton = new Button
            {
                Text = "↑ Top",
                Size = new Size(50, 20),
                Location = new Point(headerPanel.Width - 105, 2),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                BackColor = Color.DarkBlue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Arial", 7)
            };
            scrollToTopButton.Click += ScrollToTopButton_Click;

            scrollToBottomButton = new Button
            {
                Text = "↓ Bot",
                Size = new Size(50, 20),
                Location = new Point(headerPanel.Width - 50, 2),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                BackColor = Color.DarkGreen,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Arial", 7)
            };
            scrollToBottomButton.Click += ScrollToBottomButton_Click;

            headerPanel.Controls.AddRange(new Control[] {
                titleLabel, clearButton, scrollToTopButton, scrollToBottomButton
            });
        }

        private void CreateTextDisplay()
        {
            gameTextBox = new RichTextBox
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                BackColor = Color.Black,
                ForeColor = Color.LimeGreen,
                Font = new Font("Consolas", 10),
                ScrollBars = RichTextBoxScrollBars.Vertical,
                BorderStyle = BorderStyle.None,
                WordWrap = true
            };

            // Add context menu
            var contextMenu = new ContextMenuStrip();
            contextMenu.Items.Add("Copy All Text", null, (s, e) => CopyAllText());
            contextMenu.Items.Add("Copy Selected", null, (s, e) => CopySelectedText());
            contextMenu.Items.Add("-");
            contextMenu.Items.Add("Clear Console", null, (s, e) => ClearText());
            contextMenu.Items.Add("-");
            contextMenu.Items.Add("Scroll to Top", null, (s, e) => ScrollToTop());
            contextMenu.Items.Add("Scroll to Bottom", null, (s, e) => ScrollToBottom());

            gameTextBox.ContextMenuStrip = contextMenu;
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

        public void ClearText()
        {
            if (gameTextBox.InvokeRequired)
            {
                gameTextBox.Invoke(new Action(ClearText));
                return;
            }
            gameTextBox.Clear();
        }

        public void ScrollToTop()
        {
            if (gameTextBox.InvokeRequired)
            {
                gameTextBox.Invoke(new Action(ScrollToTop));
                return;
            }
            gameTextBox.SelectionStart = 0;
            gameTextBox.ScrollToCaret();
        }

        public void ScrollToBottom()
        {
            if (gameTextBox.InvokeRequired)
            {
                gameTextBox.Invoke(new Action(ScrollToBottom));
                return;
            }
            gameTextBox.SelectionStart = gameTextBox.TextLength;
            gameTextBox.ScrollToCaret();
        }

        public void ApplyTheme(string themeName)
        {
            switch (themeName.ToLower())
            {
                case "classic":
                    gameTextBox.BackColor = Color.Black;
                    gameTextBox.ForeColor = Color.LimeGreen;
                    break;
                case "modern":
                    gameTextBox.BackColor = Color.White;
                    gameTextBox.ForeColor = Color.Black;
                    break;
                case "fantasy":
                    gameTextBox.BackColor = Color.DarkSlateBlue;
                    gameTextBox.ForeColor = Color.Gold;
                    break;
                case "dark":
                    gameTextBox.BackColor = Color.DarkGray;
                    gameTextBox.ForeColor = Color.White;
                    break;
                case "high contrast":
                    gameTextBox.BackColor = Color.Black;
                    gameTextBox.ForeColor = Color.Yellow;
                    break;
                default:
                    gameTextBox.BackColor = Color.Black;
                    gameTextBox.ForeColor = Color.LimeGreen;
                    break;
            }
        }

        public void SetFont(Font font)
        {
            gameTextBox.Font = font;
        }

        public Font GetFont()
        {
            return gameTextBox.Font;
        }

        public void SetTitle(string title)
        {
            titleLabel.Text = title;
        }

        private void CopyAllText()
        {
            if (!string.IsNullOrEmpty(gameTextBox.Text))
            {
                Clipboard.SetText(gameTextBox.Text);
            }
        }

        private void CopySelectedText()
        {
            if (!string.IsNullOrEmpty(gameTextBox.SelectedText))
            {
                Clipboard.SetText(gameTextBox.SelectedText);
            }
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            ClearRequested?.Invoke(this, EventArgs.Empty);
        }

        private void ScrollToTopButton_Click(object sender, EventArgs e)
        {
            ScrollToTop();
            ScrollToTopRequested?.Invoke(this, EventArgs.Empty);
        }

        private void ScrollToBottomButton_Click(object sender, EventArgs e)
        {
            ScrollToBottom();
            ScrollToBottomRequested?.Invoke(this, EventArgs.Empty);
        }
    }
} 