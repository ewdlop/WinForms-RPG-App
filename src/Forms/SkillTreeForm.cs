using System;
using System.Drawing;
using System.Windows.Forms;
using WinFormsApp1.Controls;
using WinFormsApp1.Interfaces;

namespace WinFormsApp1
{
    public partial class SkillTreeForm : Form
    {
        private readonly Player player;
        private readonly ISkillManager skillManager;
        private SkillTreeControl skillTreeControl;

        public SkillTreeForm(Player player, ISkillManager skillManager)
        {
            this.player = player;
            this.skillManager = skillManager;
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
            // The skill manager will handle the actual learning and publish events
            // The UI will be updated through event handlers in the main form
            
            // Refresh the skill tree display
            skillTreeControl.UpdateSkillTree(player);
        }

        private void ResetButton_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show(
                "Are you sure you want to reset all skills? This action cannot be undone.",
                "Reset Skills",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                // For now, just show a message since ResetAllSkills doesn't exist yet
                MessageBox.Show("Skill reset functionality will be implemented in a future update.", 
                    "Feature Not Available", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                // Refresh the display
                skillTreeControl.UpdateSkillTree(player);
            }
        }
    }
} 