using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace WinFormsApp1.Controls
{
    public partial class CharacterStatsDisplayControl : UserControl
    {
        private Label statsLabel;

        public CharacterStatsDisplayControl()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Size = new Size(350, 230);
            this.BackColor = Color.LightGray;
            this.BorderStyle = BorderStyle.FixedSingle;

            statsLabel = new Label
            {
                Dock = DockStyle.Fill,
                Font = new Font("Consolas", 9),
                Text = "Select a character class to view stats...",
                BackColor = Color.LightGray,
                TextAlign = ContentAlignment.TopLeft,
                Padding = new Padding(10)
            };

            this.Controls.Add(statsLabel);
        }

        public void UpdateDisplay(CharacterClassInfo classInfo, DataLoader dataLoader)
        {
            if (classInfo == null)
            {
                statsLabel.Text = "Select a character class to view stats...";
                return;
            }

            var startingItems = GetStartingItemsDisplay(classInfo, dataLoader);

            statsLabel.Text = $"Class: {classInfo.Name}\n\n" +
                             $"{classInfo.Description}\n\n" +
                             $"Base Stats:\n" +
                             $"Health: {classInfo.BaseStats.MaxHealth}\n" +
                             $"Attack: {classInfo.BaseStats.Attack}\n" +
                             $"Defense: {classInfo.BaseStats.Defense}\n\n" +
                             $"Starting Items:\n{startingItems}";
        }

        private string GetStartingItemsDisplay(CharacterClassInfo classInfo, DataLoader dataLoader)
        {
            var itemNames = new List<string>();
            
            foreach (var itemId in classInfo.StartingItems)
            {
                var item = dataLoader.CreateItemFromId(itemId);
                itemNames.Add($"• {item?.Name ?? itemId}");
            }

            return string.Join("\n", itemNames);
        }

        public void ClearDisplay()
        {
            statsLabel.Text = "Select a character class to view stats...";
        }
    }
} 