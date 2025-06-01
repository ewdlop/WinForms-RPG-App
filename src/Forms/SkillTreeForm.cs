using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using WinFormsApp1.Controls;

namespace WinFormsApp1
{
    public partial class SkillTreeForm : Form
    {
        private SkillTreeControl skillTreeControl;
        private Player player;
        private GameEngine gameEngine;
        private Label skillPointsLabel;
        private Panel skillsPanel;
        private Dictionary<string, Skill> availableSkills;

        public SkillTreeForm(Player player, GameEngine gameEngine)
        {
            this.player = player;
            this.gameEngine = gameEngine;
            InitializeComponent();
            InitializeSkills();
            UpdateDisplay();
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

        private void InitializeSkills()
        {
            availableSkills = new Dictionary<string, Skill>
            {
                ["Combat Training"] = new Skill
                {
                    Name = "Combat Training",
                    Description = "Increases attack damage by 2 per level",
                    MaxLevel = 5,
                    Cost = 1,
                    Type = SkillType.Combat
                },
                ["Defensive Stance"] = new Skill
                {
                    Name = "Defensive Stance",
                    Description = "Increases defense by 1 per level",
                    MaxLevel = 5,
                    Cost = 1,
                    Type = SkillType.Combat
                },
                ["Health Boost"] = new Skill
                {
                    Name = "Health Boost",
                    Description = "Increases maximum health by 10 per level",
                    MaxLevel = 10,
                    Cost = 1,
                    Type = SkillType.Passive
                },
                ["Lucky Find"] = new Skill
                {
                    Name = "Lucky Find",
                    Description = "Increases gold found by 25% per level",
                    MaxLevel = 3,
                    Cost = 2,
                    Type = SkillType.Utility
                },
                ["Quick Learner"] = new Skill
                {
                    Name = "Quick Learner",
                    Description = "Increases experience gained by 15% per level",
                    MaxLevel = 3,
                    Cost = 2,
                    Type = SkillType.Utility
                },
                ["Magic Resistance"] = new Skill
                {
                    Name = "Magic Resistance",
                    Description = "Reduces magic damage taken by 10% per level",
                    MaxLevel = 5,
                    Cost = 1,
                    Type = SkillType.Passive
                },
                ["Critical Strike"] = new Skill
                {
                    Name = "Critical Strike",
                    Description = "Increases critical hit chance by 5% per level",
                    MaxLevel = 4,
                    Cost = 2,
                    Type = SkillType.Combat
                },
                ["Meditation"] = new Skill
                {
                    Name = "Meditation",
                    Description = "Regenerates 1 health per turn per level",
                    MaxLevel = 3,
                    Cost = 2,
                    Type = SkillType.Passive
                }
            };
        }

        private void UpdateDisplay()
        {
            skillPointsLabel.Text = $"Available Skill Points: {player.SkillPoints}";
            
            skillsPanel.Controls.Clear();
            int yOffset = 10;

            foreach (var skill in availableSkills.Values)
            {
                CreateSkillControl(skill, yOffset);
                yOffset += 80;
            }
        }

        private void CreateSkillControl(Skill skill, int yOffset)
        {
            var skillPanel = new Panel
            {
                Location = new Point(10, yOffset),
                Size = new Size(500, 70),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = GetSkillBackgroundColor(skill.Type)
            };

            // Skill name
            var nameLabel = new Label
            {
                Text = skill.Name,
                Font = new Font("Arial", 10, FontStyle.Bold),
                Location = new Point(10, 5),
                Size = new Size(200, 20)
            };
            skillPanel.Controls.Add(nameLabel);

            // Skill description
            var descLabel = new Label
            {
                Text = skill.Description,
                Location = new Point(10, 25),
                Size = new Size(300, 20),
                ForeColor = Color.DarkBlue
            };
            skillPanel.Controls.Add(descLabel);

            // Current level
            int currentLevel = player.GetSkillLevel(skill.Name);
            var levelLabel = new Label
            {
                Text = $"Level: {currentLevel}/{skill.MaxLevel}",
                Location = new Point(10, 45),
                Size = new Size(100, 20)
            };
            skillPanel.Controls.Add(levelLabel);

            // Cost
            var costLabel = new Label
            {
                Text = $"Cost: {skill.Cost} SP",
                Location = new Point(120, 45),
                Size = new Size(80, 20),
                ForeColor = Color.Red
            };
            skillPanel.Controls.Add(costLabel);

            // Upgrade button
            var upgradeButton = new Button
            {
                Text = "Upgrade",
                Location = new Point(400, 20),
                Size = new Size(80, 30),
                Enabled = CanUpgradeSkill(skill, currentLevel)
            };
            upgradeButton.Click += (s, e) => UpgradeSkill(skill);
            skillPanel.Controls.Add(upgradeButton);

            skillsPanel.Controls.Add(skillPanel);
        }

        private Color GetSkillBackgroundColor(SkillType type)
        {
            return type switch
            {
                SkillType.Combat => Color.LightCoral,
                SkillType.Magic => Color.LightBlue,
                SkillType.Utility => Color.LightGreen,
                SkillType.Passive => Color.LightYellow,
                _ => Color.LightGray
            };
        }

        private bool CanUpgradeSkill(Skill skill, int currentLevel)
        {
            return player.SkillPoints >= skill.Cost && 
                   currentLevel < skill.MaxLevel;
        }

        private void UpgradeSkill(Skill skill)
        {
            if (!CanUpgradeSkill(skill, player.GetSkillLevel(skill.Name)))
                return;

            // Spend skill points
            player.SkillPoints -= skill.Cost;
            
            // Increase skill level
            player.AddSkillPoint(skill.Name);
            
            // Apply skill effects
            ApplySkillEffect(skill);
            
            // Update display
            UpdateDisplay();
            
            // Show confirmation
            gameEngine.DisplayMessage($"Upgraded {skill.Name}! New level: {player.GetSkillLevel(skill.Name)}", Color.Green);
        }

        private void ApplySkillEffect(Skill skill)
        {
            int skillLevel = player.GetSkillLevel(skill.Name);
            
            switch (skill.Name)
            {
                case "Combat Training":
                    player.Attack += 2;
                    break;
                case "Defensive Stance":
                    player.Defense += 1;
                    break;
                case "Health Boost":
                    int healthIncrease = 10;
                    player.MaxHealth += healthIncrease;
                    player.Health += healthIncrease; // Also increase current health
                    break;
                // Other skills would have their effects applied during gameplay
                // (Lucky Find, Quick Learner, etc. would be checked when relevant)
            }
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
                UpdateDisplay();
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