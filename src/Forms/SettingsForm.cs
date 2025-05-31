using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace WinFormsApp1
{
    public partial class SettingsForm : Form
    {
        private TabControl tabControl;
        private CheckBox autoSaveCheckBox;
        private CheckBox soundEffectsCheckBox;
        private ComboBox difficultyComboBox;
        private ComboBox animationSpeedComboBox;
        private TrackBar fontSizeTrackBar;
        private Label fontSizeLabel;
        private ComboBox themeComboBox;
        private CheckBox confirmExitCheckBox;
        private CheckBox showTooltipsCheckBox;
        private NumericUpDown autoSaveIntervalNumeric;
        private Button okButton;
        private Button cancelButton;
        private Button applyButton;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public GameSettings Settings { get; private set; }

        public SettingsForm(GameSettings currentSettings)
        {
            Settings = new GameSettings
            {
                Difficulty = currentSettings.Difficulty,
                AutoSaveEnabled = currentSettings.AutoSaveEnabled,
                AutoSaveInterval = currentSettings.AutoSaveInterval,
                SoundEffectsEnabled = currentSettings.SoundEffectsEnabled,
                AnimationSpeed = currentSettings.AnimationSpeed,
                Theme = currentSettings.Theme,
                FontSize = currentSettings.FontSize,
                ShowTooltips = currentSettings.ShowTooltips,
                ConfirmExit = currentSettings.ConfirmExit
            };
            InitializeComponent();
            LoadSettings();
        }

        private void InitializeComponent()
        {
            this.Text = "Game Settings";
            this.Size = new Size(500, 400);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Create tab control
            tabControl = new TabControl
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(10)
            };

            // Create tabs
            CreateGameplayTab();
            CreateDisplayTab();
            CreateAdvancedTab();

            // Create button panel
            Panel buttonPanel = CreateButtonPanel();

            // Add controls to form
            this.Controls.Add(tabControl);
            this.Controls.Add(buttonPanel);
        }

        private void CreateGameplayTab()
        {
            TabPage gameplayTab = new TabPage("Gameplay");
            
            TableLayoutPanel layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 6,
                Padding = new Padding(10)
            };

            // Difficulty setting
            Label difficultyLabel = new Label
            {
                Text = "Difficulty:",
                AutoSize = true,
                Anchor = AnchorStyles.Left
            };

            difficultyComboBox = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Anchor = AnchorStyles.Left | AnchorStyles.Right
            };
            difficultyComboBox.Items.AddRange(new[] { "Easy", "Normal", "Hard", "Expert" });

            // Auto-save setting
            autoSaveCheckBox = new CheckBox
            {
                Text = "Enable auto-save",
                AutoSize = true,
                Anchor = AnchorStyles.Left
            };

            Label autoSaveIntervalLabel = new Label
            {
                Text = "Auto-save interval (minutes):",
                AutoSize = true,
                Anchor = AnchorStyles.Left
            };

            autoSaveIntervalNumeric = new NumericUpDown
            {
                Minimum = 1,
                Maximum = 60,
                Value = 5,
                Anchor = AnchorStyles.Left
            };

            // Sound effects
            soundEffectsCheckBox = new CheckBox
            {
                Text = "Enable sound effects",
                AutoSize = true,
                Anchor = AnchorStyles.Left
            };

            // Animation speed
            Label animationLabel = new Label
            {
                Text = "Animation speed:",
                AutoSize = true,
                Anchor = AnchorStyles.Left
            };

            animationSpeedComboBox = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Anchor = AnchorStyles.Left | AnchorStyles.Right
            };
            animationSpeedComboBox.Items.AddRange(new[] { "Slow", "Normal", "Fast", "Instant" });

            // Add controls to layout
            layout.Controls.Add(difficultyLabel, 0, 0);
            layout.Controls.Add(difficultyComboBox, 1, 0);
            layout.Controls.Add(autoSaveCheckBox, 0, 1);
            layout.SetColumnSpan(autoSaveCheckBox, 2);
            layout.Controls.Add(autoSaveIntervalLabel, 0, 2);
            layout.Controls.Add(autoSaveIntervalNumeric, 1, 2);
            layout.Controls.Add(soundEffectsCheckBox, 0, 3);
            layout.SetColumnSpan(soundEffectsCheckBox, 2);
            layout.Controls.Add(animationLabel, 0, 4);
            layout.Controls.Add(animationSpeedComboBox, 1, 4);

            gameplayTab.Controls.Add(layout);
            tabControl.TabPages.Add(gameplayTab);
        }

        private void CreateDisplayTab()
        {
            TabPage displayTab = new TabPage("Display");
            
            TableLayoutPanel layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 5,
                Padding = new Padding(10)
            };

            // Theme setting
            Label themeLabel = new Label
            {
                Text = "Theme:",
                AutoSize = true,
                Anchor = AnchorStyles.Left
            };

            themeComboBox = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Anchor = AnchorStyles.Left | AnchorStyles.Right
            };
            themeComboBox.Items.AddRange(new[] { "Classic", "Modern", "Fantasy", "Dark", "High Contrast" });

            // Font size setting
            Label fontLabel = new Label
            {
                Text = "Font size:",
                AutoSize = true,
                Anchor = AnchorStyles.Left
            };

            Panel fontPanel = new Panel
            {
                Height = 50,
                Anchor = AnchorStyles.Left | AnchorStyles.Right
            };

            fontSizeTrackBar = new TrackBar
            {
                Minimum = 8,
                Maximum = 20,
                Value = 10,
                TickFrequency = 2,
                Dock = DockStyle.Top
            };
            fontSizeTrackBar.ValueChanged += FontSizeTrackBar_ValueChanged;

            fontSizeLabel = new Label
            {
                Text = "10pt",
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Bottom
            };

            fontPanel.Controls.Add(fontSizeTrackBar);
            fontPanel.Controls.Add(fontSizeLabel);

            // Show tooltips
            showTooltipsCheckBox = new CheckBox
            {
                Text = "Show tooltips",
                AutoSize = true,
                Anchor = AnchorStyles.Left
            };

            // Add controls to layout
            layout.Controls.Add(themeLabel, 0, 0);
            layout.Controls.Add(themeComboBox, 1, 0);
            layout.Controls.Add(fontLabel, 0, 1);
            layout.Controls.Add(fontPanel, 1, 1);
            layout.Controls.Add(showTooltipsCheckBox, 0, 2);
            layout.SetColumnSpan(showTooltipsCheckBox, 2);

            displayTab.Controls.Add(layout);
            tabControl.TabPages.Add(displayTab);
        }

        private void CreateAdvancedTab()
        {
            TabPage advancedTab = new TabPage("Advanced");
            
            TableLayoutPanel layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4,
                Padding = new Padding(10)
            };

            // Confirm exit
            confirmExitCheckBox = new CheckBox
            {
                Text = "Confirm before exiting",
                AutoSize = true,
                Anchor = AnchorStyles.Left
            };

            // Debug options group
            GroupBox debugGroup = new GroupBox
            {
                Text = "Debug Options",
                Height = 100,
                Anchor = AnchorStyles.Left | AnchorStyles.Right
            };

            CheckBox showDebugInfoCheckBox = new CheckBox
            {
                Text = "Show debug information",
                Location = new Point(10, 25),
                AutoSize = true
            };

            CheckBox enableLoggingCheckBox = new CheckBox
            {
                Text = "Enable detailed logging",
                Location = new Point(10, 50),
                AutoSize = true
            };

            debugGroup.Controls.Add(showDebugInfoCheckBox);
            debugGroup.Controls.Add(enableLoggingCheckBox);

            // Performance group
            GroupBox performanceGroup = new GroupBox
            {
                Text = "Performance",
                Height = 80,
                Anchor = AnchorStyles.Left | AnchorStyles.Right
            };

            CheckBox reducedAnimationsCheckBox = new CheckBox
            {
                Text = "Reduce animations for better performance",
                Location = new Point(10, 25),
                AutoSize = true
            };

            performanceGroup.Controls.Add(reducedAnimationsCheckBox);

            // Add controls to layout
            layout.Controls.Add(confirmExitCheckBox, 0, 0);
            layout.Controls.Add(debugGroup, 0, 1);
            layout.Controls.Add(performanceGroup, 0, 2);

            advancedTab.Controls.Add(layout);
            tabControl.TabPages.Add(advancedTab);
        }

        private Panel CreateButtonPanel()
        {
            Panel buttonPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 50,
                Padding = new Padding(10)
            };

            okButton = new Button
            {
                Text = "OK",
                Size = new Size(75, 30),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                DialogResult = DialogResult.OK
            };
            okButton.Location = new Point(buttonPanel.Width - 250, 10);
            okButton.Click += OkButton_Click;

            cancelButton = new Button
            {
                Text = "Cancel",
                Size = new Size(75, 30),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                DialogResult = DialogResult.Cancel
            };
            cancelButton.Location = new Point(buttonPanel.Width - 170, 10);

            applyButton = new Button
            {
                Text = "Apply",
                Size = new Size(75, 30),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right
            };
            applyButton.Location = new Point(buttonPanel.Width - 90, 10);
            applyButton.Click += ApplyButton_Click;

            buttonPanel.Controls.AddRange(new Control[] { okButton, cancelButton, applyButton });
            return buttonPanel;
        }

        private void LoadSettings()
        {
            difficultyComboBox.SelectedItem = Settings.Difficulty;
            autoSaveCheckBox.Checked = Settings.AutoSaveEnabled;
            autoSaveIntervalNumeric.Value = Settings.AutoSaveInterval;
            soundEffectsCheckBox.Checked = Settings.SoundEffectsEnabled;
            animationSpeedComboBox.SelectedItem = Settings.AnimationSpeed;
            themeComboBox.SelectedItem = Settings.Theme;
            fontSizeTrackBar.Value = Settings.FontSize;
            showTooltipsCheckBox.Checked = Settings.ShowTooltips;
            confirmExitCheckBox.Checked = Settings.ConfirmExit;
        }

        private void SaveSettings()
        {
            Settings.Difficulty = difficultyComboBox.SelectedItem?.ToString() ?? "Normal";
            Settings.AutoSaveEnabled = autoSaveCheckBox.Checked;
            Settings.AutoSaveInterval = (int)autoSaveIntervalNumeric.Value;
            Settings.SoundEffectsEnabled = soundEffectsCheckBox.Checked;
            Settings.AnimationSpeed = animationSpeedComboBox.SelectedItem?.ToString() ?? "Normal";
            Settings.Theme = themeComboBox.SelectedItem?.ToString() ?? "Classic";
            Settings.FontSize = fontSizeTrackBar.Value;
            Settings.ShowTooltips = showTooltipsCheckBox.Checked;
            Settings.ConfirmExit = confirmExitCheckBox.Checked;
        }

        private void FontSizeTrackBar_ValueChanged(object sender, EventArgs e)
        {
            fontSizeLabel.Text = $"{fontSizeTrackBar.Value}pt";
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            SaveSettings();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void ApplyButton_Click(object sender, EventArgs e)
        {
            SaveSettings();
        }
    }

    public class GameSettings
    {
        public string Difficulty { get; set; } = "Normal";
        public bool AutoSaveEnabled { get; set; } = true;
        public int AutoSaveInterval { get; set; } = 5;
        public bool SoundEffectsEnabled { get; set; } = false;
        public string AnimationSpeed { get; set; } = "Normal";
        public string Theme { get; set; } = "Classic";
        public int FontSize { get; set; } = 10;
        public bool ShowTooltips { get; set; } = true;
        public bool ConfirmExit { get; set; } = true;
    }
} 