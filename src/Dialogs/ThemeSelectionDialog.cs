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
        private RadioButton darkRadio;
        private RadioButton highContrastRadio;
        private Panel previewPanel;
        private Label previewLabel;
        private Button okButton;
        private Button cancelButton;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string SelectedTheme { get; private set; }

        public ThemeSelectionDialog()
        {
            InitializeComponent();
            SelectedTheme = "Classic"; // Default
        }

        private void InitializeComponent()
        {
            this.Text = "Select Theme";
            this.Size = new Size(400, 350);
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

            darkRadio = new RadioButton
            {
                Text = "Dark Mode",
                Location = new Point(30, 150),
                Size = new Size(150, 20)
            };
            darkRadio.CheckedChanged += ThemeRadio_CheckedChanged;

            highContrastRadio = new RadioButton
            {
                Text = "High Contrast",
                Location = new Point(30, 180),
                Size = new Size(150, 20)
            };
            highContrastRadio.CheckedChanged += ThemeRadio_CheckedChanged;

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
                Size = new Size(150, 100),
                BorderStyle = BorderStyle.FixedSingle
            };

            previewLabel = new Label
            {
                Text = "Sample game text\n> look\nYou are in a village.\nExits: north, east",
                Location = new Point(5, 5),
                Size = new Size(140, 90),
                Font = new Font("Consolas", 8)
            };

            previewPanel.Controls.Add(previewLabel);

            // Buttons
            okButton = new Button
            {
                Text = "OK",
                Location = new Point(200, 270),
                Size = new Size(75, 30),
                DialogResult = DialogResult.OK
            };

            cancelButton = new Button
            {
                Text = "Cancel",
                Location = new Point(285, 270),
                Size = new Size(75, 30),
                DialogResult = DialogResult.Cancel
            };

            // Add controls
            this.Controls.AddRange(new Control[] {
                titleLabel, classicRadio, modernRadio, fantasyRadio, darkRadio, highContrastRadio,
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
                    SelectedTheme = "Classic";
                else if (radio == modernRadio)
                    SelectedTheme = "Modern";
                else if (radio == fantasyRadio)
                    SelectedTheme = "Fantasy";
                else if (radio == darkRadio)
                    SelectedTheme = "Dark";
                else if (radio == highContrastRadio)
                    SelectedTheme = "High Contrast";

                UpdatePreview();
            }
        }

        private void UpdatePreview()
        {
            switch (SelectedTheme.ToLower())
            {
                case "classic":
                    previewPanel.BackColor = Color.Black;
                    previewLabel.ForeColor = Color.LimeGreen;
                    break;
                case "modern":
                    previewPanel.BackColor = Color.White;
                    previewLabel.ForeColor = Color.Black;
                    break;
                case "fantasy":
                    previewPanel.BackColor = Color.DarkSlateBlue;
                    previewLabel.ForeColor = Color.Gold;
                    break;
                case "dark":
                    previewPanel.BackColor = Color.DarkGray;
                    previewLabel.ForeColor = Color.White;
                    break;
                case "high contrast":
                    previewPanel.BackColor = Color.Black;
                    previewLabel.ForeColor = Color.Yellow;
                    break;
                default:
                    previewPanel.BackColor = Color.Black;
                    previewLabel.ForeColor = Color.LimeGreen;
                    break;
            }
        }
    }
} 