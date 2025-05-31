using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace WinFormsApp1.Controls
{
    public partial class SkillTreeControl : UserControl
    {
        private TreeView skillTreeView;
        private Panel detailsPanel;
        private Label skillNameLabel;
        private Label skillDescriptionLabel;
        private Label skillCostLabel;
        private Label skillRequirementsLabel;
        private Button learnSkillButton;
        private ProgressBar skillPointsBar;
        private Label skillPointsLabel;

        private Player player;
        private List<SkillNode> allSkills;

        public event EventHandler<SkillLearnedEventArgs> SkillLearned;

        public SkillTreeControl()
        {
            InitializeComponent();
            InitializeSkillTree();
        }

        private void InitializeComponent()
        {
            this.Size = new Size(600, 400);
            this.BackColor = Color.White;

            // Main layout
            TableLayoutPanel mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 2,
                Padding = new Padding(5)
            };

            // Set column and row styles
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60F));
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            // Skill points display
            Panel skillPointsPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Height = 50
            };

            skillPointsLabel = new Label
            {
                Text = "Skill Points: 0/100",
                Location = new Point(5, 5),
                AutoSize = true,
                Font = new Font("Arial", 10, FontStyle.Bold)
            };

            skillPointsBar = new ProgressBar
            {
                Location = new Point(5, 25),
                Size = new Size(200, 20),
                Style = ProgressBarStyle.Continuous,
                ForeColor = Color.Blue,
                Minimum = 0,
                Maximum = 100,
                Value = 0
            };

            skillPointsPanel.Controls.Add(skillPointsLabel);
            skillPointsPanel.Controls.Add(skillPointsBar);

            // Tree view for skills
            skillTreeView = new TreeView
            {
                Dock = DockStyle.Fill,
                ShowLines = true,
                ShowPlusMinus = true,
                ShowRootLines = true,
                FullRowSelect = true,
                HideSelection = false,
                ImageList = CreateSkillImageList()
            };
            skillTreeView.AfterSelect += SkillTreeView_AfterSelect;
            skillTreeView.NodeMouseDoubleClick += SkillTreeView_NodeMouseDoubleClick;

            // Details panel
            CreateDetailsPanel();

            // Add controls to layout
            mainLayout.Controls.Add(skillPointsPanel, 0, 0);
            mainLayout.SetColumnSpan(skillPointsPanel, 2);
            mainLayout.Controls.Add(skillTreeView, 0, 1);
            mainLayout.Controls.Add(detailsPanel, 1, 1);

            this.Controls.Add(mainLayout);
        }

        private void CreateDetailsPanel()
        {
            detailsPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.LightGray,
                Padding = new Padding(10)
            };

            skillNameLabel = new Label
            {
                Text = "Select a skill",
                Font = new Font("Arial", 12, FontStyle.Bold),
                Location = new Point(10, 10),
                Size = new Size(200, 25),
                ForeColor = Color.DarkBlue
            };

            skillDescriptionLabel = new Label
            {
                Text = "Skill description will appear here...",
                Location = new Point(10, 45),
                Size = new Size(200, 80),
                AutoSize = false,
                Font = new Font("Arial", 9)
            };

            skillCostLabel = new Label
            {
                Text = "Cost: -",
                Location = new Point(10, 135),
                Size = new Size(100, 20),
                Font = new Font("Arial", 9, FontStyle.Bold),
                ForeColor = Color.DarkGreen
            };

            skillRequirementsLabel = new Label
            {
                Text = "Requirements: -",
                Location = new Point(10, 160),
                Size = new Size(200, 60),
                AutoSize = false,
                Font = new Font("Arial", 8),
                ForeColor = Color.DarkRed
            };

            learnSkillButton = new Button
            {
                Text = "Learn Skill",
                Location = new Point(10, 230),
                Size = new Size(100, 30),
                BackColor = Color.Green,
                ForeColor = Color.White,
                Enabled = false
            };
            learnSkillButton.Click += LearnSkillButton_Click;

            detailsPanel.Controls.AddRange(new Control[] {
                skillNameLabel, skillDescriptionLabel, skillCostLabel,
                skillRequirementsLabel, learnSkillButton
            });
        }

        private ImageList CreateSkillImageList()
        {
            ImageList imageList = new ImageList
            {
                ImageSize = new Size(16, 16),
                ColorDepth = ColorDepth.Depth32Bit
            };

            // Create simple colored squares for different skill types
            var categoryIcon = new Bitmap(16, 16);
            using (var g = Graphics.FromImage(categoryIcon))
            {
                g.FillRectangle(Brushes.Blue, 0, 0, 16, 16);
                g.DrawRectangle(Pens.Black, 0, 0, 15, 15);
            }

            var availableIcon = new Bitmap(16, 16);
            using (var g = Graphics.FromImage(availableIcon))
            {
                g.FillRectangle(Brushes.Green, 0, 0, 16, 16);
                g.DrawRectangle(Pens.Black, 0, 0, 15, 15);
            }

            var learnedIcon = new Bitmap(16, 16);
            using (var g = Graphics.FromImage(learnedIcon))
            {
                g.FillRectangle(Brushes.Gold, 0, 0, 16, 16);
                g.DrawRectangle(Pens.Black, 0, 0, 15, 15);
            }

            var lockedIcon = new Bitmap(16, 16);
            using (var g = Graphics.FromImage(lockedIcon))
            {
                g.FillRectangle(Brushes.Gray, 0, 0, 16, 16);
                g.DrawRectangle(Pens.Black, 0, 0, 15, 15);
            }

            imageList.Images.Add("category", categoryIcon);
            imageList.Images.Add("available", availableIcon);
            imageList.Images.Add("learned", learnedIcon);
            imageList.Images.Add("locked", lockedIcon);

            return imageList;
        }

        private void InitializeSkillTree()
        {
            allSkills = new List<SkillNode>
            {
                // Combat Skills
                new SkillNode("combat_basic_attack", "Basic Attack", "Increases basic attack damage by 10%", SkillCategory.Combat, 1, null, 5),
                new SkillNode("combat_power_strike", "Power Strike", "Powerful attack that deals 2x damage", SkillCategory.Combat, 2, new[] { "combat_basic_attack" }, 10),
                new SkillNode("combat_berserker", "Berserker Rage", "Increase damage by 50% but reduce defense by 25%", SkillCategory.Combat, 3, new[] { "combat_power_strike" }, 20),
                
                // Magic Skills
                new SkillNode("magic_fire_bolt", "Fire Bolt", "Cast a fire bolt dealing magical damage", SkillCategory.Magic, 1, null, 8),
                new SkillNode("magic_heal", "Heal", "Restore health points", SkillCategory.Magic, 1, null, 6),
                new SkillNode("magic_fireball", "Fireball", "Powerful fire spell with area damage", SkillCategory.Magic, 2, new[] { "magic_fire_bolt" }, 15),
                new SkillNode("magic_greater_heal", "Greater Heal", "Restore large amount of health", SkillCategory.Magic, 2, new[] { "magic_heal" }, 12),
                
                // Defense Skills
                new SkillNode("defense_block", "Shield Block", "Chance to block incoming attacks", SkillCategory.Defense, 1, null, 7),
                new SkillNode("defense_armor_mastery", "Armor Mastery", "Increases armor effectiveness by 25%", SkillCategory.Defense, 2, new[] { "defense_block" }, 10),
                new SkillNode("defense_fortress", "Fortress", "Become immobile but gain 75% damage reduction", SkillCategory.Defense, 3, new[] { "defense_armor_mastery" }, 25),
                
                // Utility Skills
                new SkillNode("utility_appraise", "Appraise", "Identify unknown items", SkillCategory.Utility, 1, null, 3),
                new SkillNode("utility_lockpicking", "Lockpicking", "Open locked doors and chests", SkillCategory.Utility, 1, null, 5),
                new SkillNode("utility_stealth", "Stealth", "Avoid random encounters", SkillCategory.Utility, 2, new[] { "utility_lockpicking" }, 12)
            };
        }

        public void UpdateSkillTree(Player player)
        {
            this.player = player;
            
            // Update skill points display
            int availablePoints = player.SkillPoints;
            skillPointsLabel.Text = $"Skill Points: {availablePoints}";
            skillPointsBar.Value = Math.Min(availablePoints, skillPointsBar.Maximum);

            // Clear and rebuild tree
            skillTreeView.Nodes.Clear();

            // Group skills by category
            var categories = Enum.GetValues<SkillCategory>().Cast<SkillCategory>();
            
            foreach (var category in categories)
            {
                var categoryNode = new TreeNode(category.ToString())
                {
                    ImageKey = "category",
                    SelectedImageKey = "category"
                };

                var categorySkills = allSkills.Where(s => s.Category == category).OrderBy(s => s.Tier);
                
                foreach (var skill in categorySkills)
                {
                    var skillNode = new TreeNode(skill.Name)
                    {
                        Tag = skill
                    };

                    // Determine skill state and icon
                    if (player.LearnedSkills.Contains(skill.Id))
                    {
                        skillNode.ImageKey = "learned";
                        skillNode.SelectedImageKey = "learned";
                        skillNode.ForeColor = Color.Gold;
                    }
                    else if (CanLearnSkill(skill))
                    {
                        skillNode.ImageKey = "available";
                        skillNode.SelectedImageKey = "available";
                        skillNode.ForeColor = Color.Green;
                    }
                    else
                    {
                        skillNode.ImageKey = "locked";
                        skillNode.SelectedImageKey = "locked";
                        skillNode.ForeColor = Color.Gray;
                    }

                    categoryNode.Nodes.Add(skillNode);
                }

                skillTreeView.Nodes.Add(categoryNode);
                categoryNode.Expand();
            }
        }

        private bool CanLearnSkill(SkillNode skill)
        {
            if (player == null) return false;
            if (player.LearnedSkills.Contains(skill.Id)) return false;
            if (player.SkillPoints < skill.Cost) return false;

            // Check prerequisites
            if (skill.Prerequisites != null)
            {
                foreach (var prereq in skill.Prerequisites)
                {
                    if (!player.LearnedSkills.Contains(prereq))
                        return false;
                }
            }

            return true;
        }

        private void SkillTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Tag is SkillNode skill)
            {
                DisplaySkillDetails(skill);
            }
            else
            {
                ClearSkillDetails();
            }
        }

        private void SkillTreeView_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node.Tag is SkillNode skill && CanLearnSkill(skill))
            {
                LearnSkill(skill);
            }
        }

        private void DisplaySkillDetails(SkillNode skill)
        {
            skillNameLabel.Text = skill.Name;
            skillDescriptionLabel.Text = skill.Description;
            skillCostLabel.Text = $"Cost: {skill.Cost} SP";

            // Display prerequisites
            if (skill.Prerequisites != null && skill.Prerequisites.Length > 0)
            {
                var prereqNames = skill.Prerequisites.Select(id => 
                    allSkills.FirstOrDefault(s => s.Id == id)?.Name ?? id);
                skillRequirementsLabel.Text = $"Requirements:\n{string.Join("\n", prereqNames)}";
            }
            else
            {
                skillRequirementsLabel.Text = "Requirements: None";
            }

            // Update learn button
            bool canLearn = CanLearnSkill(skill);
            bool alreadyLearned = player?.LearnedSkills.Contains(skill.Id) == true;
            
            learnSkillButton.Enabled = canLearn;
            learnSkillButton.Text = alreadyLearned ? "Learned" : "Learn Skill";
            learnSkillButton.BackColor = alreadyLearned ? Color.Gray : (canLearn ? Color.Green : Color.DarkGray);
        }

        private void ClearSkillDetails()
        {
            skillNameLabel.Text = "Select a skill";
            skillDescriptionLabel.Text = "Skill description will appear here...";
            skillCostLabel.Text = "Cost: -";
            skillRequirementsLabel.Text = "Requirements: -";
            learnSkillButton.Enabled = false;
            learnSkillButton.Text = "Learn Skill";
            learnSkillButton.BackColor = Color.DarkGray;
        }

        private void LearnSkillButton_Click(object sender, EventArgs e)
        {
            if (skillTreeView.SelectedNode?.Tag is SkillNode skill)
            {
                LearnSkill(skill);
            }
        }

        private void LearnSkill(SkillNode skill)
        {
            if (!CanLearnSkill(skill)) return;

            // Learn the skill
            player.LearnedSkills.Add(skill.Id);
            player.SkillPoints -= skill.Cost;

            // Refresh the tree
            UpdateSkillTree(player);

            // Fire event
            SkillLearned?.Invoke(this, new SkillLearnedEventArgs(skill));

            MessageBox.Show($"Learned {skill.Name}!", "Skill Learned", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }

    public enum SkillCategory
    {
        Combat,
        Magic,
        Defense,
        Utility
    }

    public class SkillNode
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public SkillCategory Category { get; set; }
        public int Tier { get; set; }
        public string[] Prerequisites { get; set; }
        public int Cost { get; set; }

        public SkillNode(string id, string name, string description, SkillCategory category, 
                        int tier, string[] prerequisites, int cost)
        {
            Id = id;
            Name = name;
            Description = description;
            Category = category;
            Tier = tier;
            Prerequisites = prerequisites;
            Cost = cost;
        }
    }

    public class SkillLearnedEventArgs : EventArgs
    {
        public SkillNode Skill { get; }

        public SkillLearnedEventArgs(SkillNode skill)
        {
            Skill = skill;
        }
    }
} 