using System;
using System.Drawing;
using System.Windows.Forms;

namespace WinFormsApp1.Controls
{
    public partial class GameStatusBarControl : UserControl
    {
        private Panel mainPanel;
        private Label statusLabel;
        private Label healthLabel;
        private Label levelLabel;
        private Label goldLabel;
        private Label locationLabel;
        private Label timeLabel;
        private System.Windows.Forms.Timer timeUpdateTimer;

        public GameStatusBarControl()
        {
            InitializeComponent();
            SetupTimer();
        }

        private void InitializeComponent()
        {
            this.Size = new Size(800, 25);
            this.BackColor = Color.LightGray;
            this.BorderStyle = BorderStyle.Fixed3D;

            // Main panel with table layout
            mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(2)
            };

            TableLayoutPanel layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 6,
                RowCount = 1,
                BackColor = Color.Transparent
            };

            // Set column styles for even distribution
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F)); // Status message
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15F)); // Health
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10F)); // Level
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10F)); // Gold
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F)); // Location
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15F)); // Time

            // Create status labels
            CreateStatusLabels(layout);

            mainPanel.Controls.Add(layout);
            this.Controls.Add(mainPanel);
        }

        private void CreateStatusLabels(TableLayoutPanel layout)
        {
            // Main status message
            statusLabel = new Label
            {
                Text = "Ready to start your adventure...",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Arial", 8),
                ForeColor = Color.Black,
                AutoEllipsis = true
            };
            layout.Controls.Add(statusLabel, 0, 0);

            // Health status
            healthLabel = new Label
            {
                Text = "Health: --/--",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Arial", 8, FontStyle.Bold),
                ForeColor = Color.DarkRed,
                BorderStyle = BorderStyle.FixedSingle
            };
            layout.Controls.Add(healthLabel, 1, 0);

            // Level status
            levelLabel = new Label
            {
                Text = "Lvl: --",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Arial", 8, FontStyle.Bold),
                ForeColor = Color.DarkBlue,
                BorderStyle = BorderStyle.FixedSingle
            };
            layout.Controls.Add(levelLabel, 2, 0);

            // Gold status
            goldLabel = new Label
            {
                Text = "Gold: --",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Arial", 8, FontStyle.Bold),
                ForeColor = Color.DarkGoldenrod,
                BorderStyle = BorderStyle.FixedSingle
            };
            layout.Controls.Add(goldLabel, 3, 0);

            // Location status
            locationLabel = new Label
            {
                Text = "Location: Unknown",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Arial", 8),
                ForeColor = Color.DarkGreen,
                BorderStyle = BorderStyle.FixedSingle,
                AutoEllipsis = true
            };
            layout.Controls.Add(locationLabel, 4, 0);

            // Time display
            timeLabel = new Label
            {
                Text = DateTime.Now.ToString("HH:mm:ss"),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Arial", 8),
                ForeColor = Color.DarkSlateGray,
                BorderStyle = BorderStyle.FixedSingle
            };
            layout.Controls.Add(timeLabel, 5, 0);
        }

        private void SetupTimer()
        {
            timeUpdateTimer = new System.Windows.Forms.Timer
            {
                Interval = 1000, // Update every second
                Enabled = true
            };
            timeUpdateTimer.Tick += TimeUpdateTimer_Tick;
        }

        private void TimeUpdateTimer_Tick(object sender, EventArgs e)
        {
            if (timeLabel != null)
            {
                timeLabel.Text = DateTime.Now.ToString("HH:mm:ss");
            }
        }

        public void UpdateStatus(string message)
        {
            if (statusLabel.InvokeRequired)
            {
                statusLabel.Invoke(new Action(() => UpdateStatus(message)));
                return;
            }
            statusLabel.Text = message;
        }

        public void UpdatePlayerStats(Player player)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => UpdatePlayerStats(player)));
                return;
            }

            if (player != null)
            {
                // Update health with color coding
                healthLabel.Text = $"Health: {player.Health}/{player.MaxHealth}";
                double healthPercent = (double)player.Health / player.MaxHealth;
                if (healthPercent > 0.6)
                    healthLabel.ForeColor = Color.DarkGreen;
                else if (healthPercent > 0.3)
                    healthLabel.ForeColor = Color.DarkOrange;
                else
                    healthLabel.ForeColor = Color.DarkRed;

                // Update level
                levelLabel.Text = $"Lvl: {player.Level}";

                // Update gold
                goldLabel.Text = $"Gold: {player.Gold}";
            }
            else
            {
                ClearPlayerStats();
            }
        }

        public void UpdateLocation(string locationName)
        {
            if (locationLabel.InvokeRequired)
            {
                locationLabel.Invoke(new Action(() => UpdateLocation(locationName)));
                return;
            }
            locationLabel.Text = $"Location: {locationName}";
        }

        public void ClearPlayerStats()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(ClearPlayerStats));
                return;
            }

            healthLabel.Text = "Health: --/--";
            healthLabel.ForeColor = Color.DarkRed;
            levelLabel.Text = "Lvl: --";
            goldLabel.Text = "Gold: --";
            locationLabel.Text = "Location: Unknown";
        }

        public void SetCombatMode(bool inCombat)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => SetCombatMode(inCombat)));
                return;
            }

            if (inCombat)
            {
                mainPanel.BackColor = Color.LightCoral;
                statusLabel.Text = "⚔️ IN COMBAT ⚔️";
                statusLabel.Font = new Font("Arial", 8, FontStyle.Bold);
                statusLabel.ForeColor = Color.DarkRed;
            }
            else
            {
                mainPanel.BackColor = Color.LightGray;
                statusLabel.Font = new Font("Arial", 8);
                statusLabel.ForeColor = Color.Black;
            }
        }

        public void ShowExperienceGain(int expGained, int newLevel = -1)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => ShowExperienceGain(expGained, newLevel)));
                return;
            }

            string message = $"💫 +{expGained} EXP";
            if (newLevel > 0)
            {
                message += $" | LEVEL UP! Now level {newLevel}! 🎉";
                levelLabel.ForeColor = Color.Gold;
                
                // Reset level color after 3 seconds
                System.Windows.Forms.Timer resetTimer = new System.Windows.Forms.Timer { Interval = 3000 };
                resetTimer.Tick += (s, e) =>
                {
                    levelLabel.ForeColor = Color.DarkBlue;
                    resetTimer.Stop();
                    resetTimer.Dispose();
                };
                resetTimer.Start();
            }

            statusLabel.Text = message;
            statusLabel.ForeColor = Color.Blue;

            // Reset status color after 2 seconds
            System.Windows.Forms.Timer statusResetTimer = new System.Windows.Forms.Timer { Interval = 2000 };
            statusResetTimer.Tick += (s, e) =>
            {
                statusLabel.ForeColor = Color.Black;
                statusResetTimer.Stop();
                statusResetTimer.Dispose();
            };
            statusResetTimer.Start();
        }

        public void ShowGoldChange(int goldChange)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => ShowGoldChange(goldChange)));
                return;
            }

            if (goldChange > 0)
            {
                statusLabel.Text = $"💰 +{goldChange} Gold";
                goldLabel.ForeColor = Color.Gold;
            }
            else if (goldChange < 0)
            {
                statusLabel.Text = $"💸 {goldChange} Gold";
                goldLabel.ForeColor = Color.DarkRed;
            }

            // Reset gold color after 2 seconds
            System.Windows.Forms.Timer resetTimer = new System.Windows.Forms.Timer { Interval = 2000 };
            resetTimer.Tick += (s, e) =>
            {
                goldLabel.ForeColor = Color.DarkGoldenrod;
                resetTimer.Stop();
                resetTimer.Dispose();
            };
            resetTimer.Start();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                timeUpdateTimer?.Stop();
                timeUpdateTimer?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
} 