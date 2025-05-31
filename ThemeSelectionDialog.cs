using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;

namespace WinFormsApp1
{
    public partial class ThemeSelectionDialog : Form
    {
        private RadioButton classicRadio;
        private RadioButton modernRadio;
        private RadioButton fantasyRadio;
        private Panel previewPanel;
        private Label previewLabel;
        private Button okButton;
        private Button cancelButton;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public GameTheme SelectedTheme { get; private set; }

        public ThemeSelectionDialog()
        {
            InitializeComponent();
            SelectedTheme = GameTheme.Classic; // Default
        }

        private void InitializeComponent()
        {
            this.Text = "Select Theme";
            this.Size = new Size(400, 300);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Title
            Label titleLabel = new Label
            {
                Text = "Choose Your Theme",
                Font = new Font("Arial", 14, FontStyle.Bold),
                Location = new Point(20, 20),
                Size = new Size(350, 25),
                TextAlign = ContentAlignment.MiddleCenter
            };

            // Theme options
            classicRadio = new RadioButton
            {
                Text = "Classic Terminal",
                Location = new Point(30, 60),
                Size = new Size(150, 20),
                Checked = true
            };
            classicRadio.CheckedChanged += ThemeRadio_CheckedChanged;

            modernRadio = new RadioButton
            {
                Text = "Modern Clean",
                Location = new Point(30, 90),
                Size = new Size(150, 20)
            };
            modernRadio.CheckedChanged += ThemeRadio_CheckedChanged;

            fantasyRadio = new RadioButton
            {
                Text = "Fantasy Mystical",
                Location = new Point(30, 120),
                Size = new Size(150, 20)
            };
            fantasyRadio.CheckedChanged += ThemeRadio_CheckedChanged;

            // Preview panel
            Label previewTitleLabel = new Label
            {
                Text = "Preview:",
                Location = new Point(200, 60),
                Size = new Size(60, 20),
                Font = new Font("Arial", 10, FontStyle.Bold)
            };

            previewPanel = new Panel
            {
                Location = new Point(200, 85),
                Size = new Size(150, 80),
                BorderStyle = BorderStyle.FixedSingle
            };

            previewLabel = new Label
            {
                Text = "Sample game text\n> look\nYou are in a village.\nExits: north, east",
                Location = new Point(5, 5),
                Size = new Size(140, 70),
                Font = new Font("Consolas", 8)
            };

            previewPanel.Controls.Add(previewLabel);

            // Buttons
            okButton = new Button
            {
                Text = "OK",
                Location = new Point(200, 220),
                Size = new Size(75, 30),
                DialogResult = DialogResult.OK
            };

            cancelButton = new Button
            {
                Text = "Cancel",
                Location = new Point(285, 220),
                Size = new Size(75, 30),
                DialogResult = DialogResult.Cancel
            };

            // Add controls
            this.Controls.AddRange(new Control[] {
                titleLabel, classicRadio, modernRadio, fantasyRadio,
                previewTitleLabel, previewPanel, okButton, cancelButton
            });

            // Update preview initially
            UpdatePreview();
        }

        private void ThemeRadio_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton radio = sender as RadioButton;
            if (radio.Checked)
            {
                if (radio == classicRadio)
                    SelectedTheme = GameTheme.Classic;
                else if (radio == modernRadio)
                    SelectedTheme = GameTheme.Modern;
                else if (radio == fantasyRadio)
                    SelectedTheme = GameTheme.Fantasy;

                UpdatePreview();
            }
        }

        private void UpdatePreview()
        {
            switch (SelectedTheme)
            {
                case GameTheme.Classic:
                    previewPanel.BackColor = Color.Black;
                    previewLabel.ForeColor = Color.LimeGreen;
                    break;
                case GameTheme.Modern:
                    previewPanel.BackColor = Color.White;
                    previewLabel.ForeColor = Color.Black;
                    break;
                case GameTheme.Fantasy:
                    previewPanel.BackColor = Color.DarkSlateBlue;
                    previewLabel.ForeColor = Color.Gold;
                    break;
            }
        }
    }
} 