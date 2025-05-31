using System;
using System.Drawing;
using System.Windows.Forms;

namespace WinFormsApp1.Controls
{
    public partial class ProgressDisplayControl : UserControl
    {
        private Label healthLabel;
        private ProgressBar healthBar;
        private Label experienceLabel;
        private ProgressBar experienceBar;

        public ProgressDisplayControl()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Size = new Size(300, 100);
            this.BackColor = Color.Transparent;

            TableLayoutPanel progressLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4,
                Padding = new Padding(5)
            };

            // Health label
            healthLabel = new Label
            {
                Text = "Health: 100/100",
                AutoSize = true,
                ForeColor = Color.DarkRed,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };

            // Health bar
            healthBar = new ProgressBar
            {
                Dock = DockStyle.Fill,
                Height = 20,
                Style = ProgressBarStyle.Continuous,
                ForeColor = Color.Red,
                Minimum = 0,
                Maximum = 100,
                Value = 100
            };

            // Experience label
            experienceLabel = new Label
            {
                Text = "Experience: 0/100",
                AutoSize = true,
                ForeColor = Color.DarkBlue,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };

            // Experience bar
            experienceBar = new ProgressBar
            {
                Dock = DockStyle.Fill,
                Height = 20,
                Style = ProgressBarStyle.Continuous,
                ForeColor = Color.Blue,
                Minimum = 0,
                Maximum = 100,
                Value = 0
            };

            // Add controls to layout
            progressLayout.Controls.Add(healthLabel, 0, 0);
            progressLayout.Controls.Add(healthBar, 0, 1);
            progressLayout.Controls.Add(experienceLabel, 0, 2);
            progressLayout.Controls.Add(experienceBar, 0, 3);

            this.Controls.Add(progressLayout);
        }

        public void UpdateHealth(int currentHealth, int maxHealth)
        {
            if (maxHealth <= 0) maxHealth = 1; // Prevent division by zero

            healthBar.Maximum = maxHealth;
            healthBar.Value = Math.Max(0, Math.Min(currentHealth, maxHealth));
            healthLabel.Text = $"Health: {currentHealth}/{maxHealth}";

            // Change color based on health percentage
            float healthPercent = (float)currentHealth / maxHealth;
            if (healthPercent <= 0.25f)
                healthLabel.ForeColor = Color.DarkRed;
            else if (healthPercent <= 0.5f)
                healthLabel.ForeColor = Color.Orange;
            else
                healthLabel.ForeColor = Color.DarkGreen;
        }

        public void UpdateExperience(int currentExp, int maxExp)
        {
            if (maxExp <= 0) maxExp = 1; // Prevent division by zero

            experienceBar.Maximum = maxExp;
            experienceBar.Value = Math.Max(0, Math.Min(currentExp, maxExp));
            experienceLabel.Text = $"Experience: {currentExp}/{maxExp}";

            // Change color based on experience progress
            float expPercent = (float)currentExp / maxExp;
            if (expPercent >= 0.9f)
                experienceLabel.ForeColor = Color.Gold;
            else if (expPercent >= 0.5f)
                experienceLabel.ForeColor = Color.Blue;
            else
                experienceLabel.ForeColor = Color.DarkBlue;
        }

        public void UpdateProgress(Player player)
        {
            if (player == null)
            {
                ClearProgress();
                return;
            }

            UpdateHealth(player.Health, player.MaxHealth);
            UpdateExperience(player.Experience, player.ExperienceToNextLevel);
        }

        public void ClearProgress()
        {
            healthLabel.Text = "Health: -/-";
            healthBar.Value = 0;
            experienceLabel.Text = "Experience: -/-";
            experienceBar.Value = 0;
        }
    }
} 