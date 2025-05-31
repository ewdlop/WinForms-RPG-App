using System;
using System.Drawing;
using System.Windows.Forms;

namespace WinFormsApp1.Controls
{
    public partial class CharacterStatsControl : UserControl
    {
        private TableLayoutPanel statsLayout;
        private Label levelValueLabel;
        private Label goldValueLabel;
        private Label attackValueLabel;
        private Label defenseValueLabel;
        private Label expValueLabel;
        private Label classValueLabel;
        private Label weaponValueLabel;
        private Label armorValueLabel;

        public CharacterStatsControl()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Size = new Size(300, 250);
            this.BackColor = Color.Transparent;

            // Create scrollable panel for stats
            Panel scrollablePanel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BorderStyle = BorderStyle.None
            };

            // Create stats layout
            statsLayout = new TableLayoutPanel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                ColumnCount = 2,
                RowCount = 8,
                Location = new Point(0, 0),
                Padding = new Padding(5)
            };

            // Set column styles
            statsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            statsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

            CreateStatLabels();
            AddLabelsToLayout();

            scrollablePanel.Controls.Add(statsLayout);
            this.Controls.Add(scrollablePanel);
        }

        private void CreateStatLabels()
        {
            // Level
            var levelLabel = new Label { Text = "Level:", AutoSize = true, Margin = new Padding(2) };
            levelValueLabel = new Label { Text = "1", AutoSize = true, Font = new Font("Arial", 8, FontStyle.Bold), Margin = new Padding(2) };

            // Gold
            var goldLabel = new Label { Text = "Gold:", AutoSize = true, Margin = new Padding(2) };
            goldValueLabel = new Label { Text = "50", AutoSize = true, Font = new Font("Arial", 8, FontStyle.Bold), ForeColor = Color.Goldenrod, Margin = new Padding(2) };

            // Attack
            var attackLabel = new Label { Text = "Attack:", AutoSize = true, Margin = new Padding(2) };
            attackValueLabel = new Label { Text = "15", AutoSize = true, Font = new Font("Arial", 8, FontStyle.Bold), ForeColor = Color.Red, Margin = new Padding(2) };

            // Defense
            var defenseLabel = new Label { Text = "Defense:", AutoSize = true, Margin = new Padding(2) };
            defenseValueLabel = new Label { Text = "8", AutoSize = true, Font = new Font("Arial", 8, FontStyle.Bold), ForeColor = Color.Blue, Margin = new Padding(2) };

            // Experience
            var expLabel = new Label { Text = "Exp:", AutoSize = true, Margin = new Padding(2) };
            expValueLabel = new Label { Text = "0", AutoSize = true, Font = new Font("Arial", 8, FontStyle.Bold), ForeColor = Color.Purple, Margin = new Padding(2) };

            // Class
            var classLabel = new Label { Text = "Class:", AutoSize = true, Margin = new Padding(2) };
            classValueLabel = new Label { Text = "Warrior", AutoSize = true, Font = new Font("Arial", 8, FontStyle.Bold), ForeColor = Color.DarkGreen, Margin = new Padding(2) };

            // Weapon
            var weaponLabel = new Label { Text = "Weapon:", AutoSize = true, Margin = new Padding(2) };
            weaponValueLabel = new Label { Text = "None", AutoSize = true, Font = new Font("Arial", 8, FontStyle.Bold), ForeColor = Color.DarkOrange, Margin = new Padding(2) };

            // Armor
            var armorLabel = new Label { Text = "Armor:", AutoSize = true, Margin = new Padding(2) };
            armorValueLabel = new Label { Text = "None", AutoSize = true, Font = new Font("Arial", 8, FontStyle.Bold), ForeColor = Color.DarkCyan, Margin = new Padding(2) };
        }

        private void AddLabelsToLayout()
        {
            statsLayout.Controls.Add(new Label { Text = "Level:", AutoSize = true, Margin = new Padding(2) }, 0, 0);
            statsLayout.Controls.Add(levelValueLabel, 1, 0);

            statsLayout.Controls.Add(new Label { Text = "Gold:", AutoSize = true, Margin = new Padding(2) }, 0, 1);
            statsLayout.Controls.Add(goldValueLabel, 1, 1);

            statsLayout.Controls.Add(new Label { Text = "Attack:", AutoSize = true, Margin = new Padding(2) }, 0, 2);
            statsLayout.Controls.Add(attackValueLabel, 1, 2);

            statsLayout.Controls.Add(new Label { Text = "Defense:", AutoSize = true, Margin = new Padding(2) }, 0, 3);
            statsLayout.Controls.Add(defenseValueLabel, 1, 3);

            statsLayout.Controls.Add(new Label { Text = "Exp:", AutoSize = true, Margin = new Padding(2) }, 0, 4);
            statsLayout.Controls.Add(expValueLabel, 1, 4);

            statsLayout.Controls.Add(new Label { Text = "Class:", AutoSize = true, Margin = new Padding(2) }, 0, 5);
            statsLayout.Controls.Add(classValueLabel, 1, 5);

            statsLayout.Controls.Add(new Label { Text = "Weapon:", AutoSize = true, Margin = new Padding(2) }, 0, 6);
            statsLayout.Controls.Add(weaponValueLabel, 1, 6);

            statsLayout.Controls.Add(new Label { Text = "Armor:", AutoSize = true, Margin = new Padding(2) }, 0, 7);
            statsLayout.Controls.Add(armorValueLabel, 1, 7);
        }

        public void UpdateStats(Player player)
        {
            if (player == null) return;

            levelValueLabel.Text = player.Level.ToString();
            goldValueLabel.Text = player.Gold.ToString();
            attackValueLabel.Text = player.GetTotalAttack().ToString();
            defenseValueLabel.Text = player.GetTotalDefense().ToString();
            expValueLabel.Text = $"{player.Experience}/{player.ExperienceToNextLevel}";
            classValueLabel.Text = player.CharacterClass.ToString();
            weaponValueLabel.Text = player.EquippedWeapon?.Name ?? "None";
            armorValueLabel.Text = player.EquippedArmor?.Name ?? "None";
        }

        public void ClearStats()
        {
            levelValueLabel.Text = "-";
            goldValueLabel.Text = "-";
            attackValueLabel.Text = "-";
            defenseValueLabel.Text = "-";
            expValueLabel.Text = "-";
            classValueLabel.Text = "-";
            weaponValueLabel.Text = "None";
            armorValueLabel.Text = "None";
        }
    }
} 