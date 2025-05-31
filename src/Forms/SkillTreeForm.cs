using System;
using System.Drawing;
using System.Windows.Forms;
using WinFormsApp1.Controls;

namespace WinFormsApp1
{
    public partial class SkillTreeForm : Form
    {
        private SkillTreeControl skillTreeControl;
        private Player player;
        private GameEngine gameEngine;

        public SkillTreeForm(Player player, GameEngine gameEngine)
        {
            this.player = player;
            this.gameEngine = gameEngine;
            InitializeComponent();
            LoadSkillTree();
        }

        private void InitializeComponent()
        {
            this.Text = "Skill Tree - Character Development";
            this.Size = new Size(800, 600);
            this.MinimumSize = new Size(600, 400);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MaximizeBox = true;
            this.MinimizeBox = false;

            // Main layout
            TableLayoutPanel mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                Padding = new Padding(10)
            };

            // Set row styles
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 90F));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 10F));

            // Skill tree control
            skillTreeControl = new SkillTreeControl
            {
                Dock = DockStyle.Fill
            };
            skillTreeControl.SkillLearned += SkillTreeControl_SkillLearned;

            // Button panel
            Panel buttonPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Height = 50
            };

            Button closeButton = new Button
            {
                Text = "Close",
                Size = new Size(100, 30),
                Location = new Point(10, 10),
                BackColor = Color.Gray,
                ForeColor = Color.White,
                DialogResult = DialogResult.OK
            };
            closeButton.Click += (s, e) => this.Close();

            Button resetButton = new Button
            {
                Text = "Reset Skills",
                Size = new Size(100, 30),
                Location = new Point(120, 10),
                BackColor = Color.DarkRed,
                ForeColor = Color.White
            };
            resetButton.Click += ResetButton_Click;

            Label helpLabel = new Label
            {
                Text = "💡 Tip: Double-click a skill to learn it, or select and click 'Learn Skill'",
                Location = new Point(240, 15),
                AutoSize = true,
                ForeColor = Color.DarkBlue,
                Font = new Font("Arial", 9)
            };

            buttonPanel.Controls.AddRange(new Control[] { closeButton, resetButton, helpLabel });

            // Add to main layout
            mainLayout.Controls.Add(skillTreeControl, 0, 0);
            mainLayout.Controls.Add(buttonPanel, 0, 1);

            this.Controls.Add(mainLayout);
        }

        private void LoadSkillTree()
        {
            skillTreeControl.UpdateSkillTree(player);
        }

        private void SkillTreeControl_SkillLearned(object sender, SkillLearnedEventArgs e)
        {
            // Display a message about the learned skill
            gameEngine?.DisplayMessage($"Learned skill: {e.Skill.Name}!", Color.Green);
            
            // Apply skill effects
            ApplySkillEffects(e.Skill);
        }

        private void ApplySkillEffects(SkillNode skill)
        {
            // Apply the effects of the learned skill
            switch (skill.Id)
            {
                case "combat_basic_attack":
                    player.Attack += 2; // Increase attack by 2
                    break;
                case "combat_power_strike":
                    // Power strike is a special ability, no stat change needed
                    break;
                case "combat_berserker":
                    // Berserker rage affects combat calculations
                    break;
                case "magic_fire_bolt":
                case "magic_heal":
                case "magic_fireball":
                case "magic_greater_heal":
                    // Magic skills could increase mana or spell power
                    break;
                case "defense_block":
                    player.Defense += 1;
                    break;
                case "defense_armor_mastery":
                    player.Defense += 3;
                    break;
                case "defense_fortress":
                    // Special defensive ability
                    break;
                case "utility_appraise":
                case "utility_lockpicking":
                case "utility_stealth":
                    // Utility skills unlock new commands
                    break;
            }

            // Update the main form's character display
            gameEngine?.UpdateCharacterDisplay();
        }

        private void ResetButton_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show(
                "Are you sure you want to reset all learned skills? This will refund all skill points but cannot be undone.",
                "Reset Skills",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                ResetPlayerSkills();
                LoadSkillTree();
                MessageBox.Show("All skills have been reset and skill points refunded.", "Skills Reset", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void ResetPlayerSkills()
        {
            // Calculate total skill points to refund
            int pointsToRefund = 0;
            // In a real implementation, you'd track how many points each skill cost
            // For now, we'll just give back 50 points as an example
            pointsToRefund = player.LearnedSkills.Count * 10; // Rough estimate

            // Reset learned skills
            player.LearnedSkills.Clear();
            player.SkillPoints += pointsToRefund;

            // Reset any stat bonuses from skills
            // In a real implementation, you'd need to track original base stats
            // For now, we'll reset to class defaults
            switch (player.CharacterClass)
            {
                case CharacterClass.Warrior:
                    player.Attack = 15;
                    player.Defense = 8;
                    break;
                case CharacterClass.Mage:
                    player.Attack = 20;
                    player.Defense = 5;
                    break;
                case CharacterClass.Rogue:
                    player.Attack = 18;
                    player.Defense = 6;
                    break;
                case CharacterClass.Cleric:
                    player.Attack = 12;
                    player.Defense = 10;
                    break;
            }

            gameEngine?.UpdateCharacterDisplay();
        }
    }
} 