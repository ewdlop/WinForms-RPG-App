using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Linq;

namespace WinFormsApp1
{
    public partial class CharacterCreationDialog : Form
    {
        private TextBox nameTextBox;
        private ComboBox classComboBox;
        private Label statsLabel;
        private Button createButton;
        private Button cancelButton;
        private DataLoader dataLoader;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Player CreatedCharacter { get; private set; }

        public CharacterCreationDialog(DataLoader dataLoader)
        {
            this.dataLoader = dataLoader;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Create Your Character";
            this.Size = new Size(400, 400);
            this.MinimumSize = new Size(380, 380);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MaximizeBox = true;
            this.MinimizeBox = false;

            // Title label
            Label titleLabel = new Label
            {
                Text = "Create Your Hero",
                Font = new Font("Arial", 16, FontStyle.Bold),
                ForeColor = Color.DarkBlue,
                Location = new Point(20, 20),
                Size = new Size(350, 30),
                TextAlign = ContentAlignment.MiddleCenter,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            // Name label and textbox
            Label nameLabel = new Label
            {
                Text = "Character Name:",
                Location = new Point(20, 70),
                Size = new Size(100, 20),
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };

            nameTextBox = new TextBox
            {
                Location = new Point(130, 68),
                Size = new Size(200, 20),
                Text = "Hero",
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            // Class label and combobox
            Label classLabel = new Label
            {
                Text = "Character Class:",
                Location = new Point(20, 110),
                Size = new Size(100, 20),
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };

            classComboBox = new ComboBox
            {
                Location = new Point(130, 108),
                Size = new Size(200, 20),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            // Load character classes from JSON data
            var availableClasses = dataLoader.GetAvailableClasses();
            classComboBox.Items.AddRange(availableClasses.Select(c => c.Name).ToArray());
            classComboBox.SelectedIndex = 0;
            classComboBox.SelectedIndexChanged += ClassComboBox_SelectedIndexChanged;

            // Stats display
            statsLabel = new Label
            {
                Location = new Point(20, 150),
                Size = new Size(350, 150),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.LightGray,
                Font = new Font("Consolas", 9),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };

            // Buttons
            createButton = new Button
            {
                Text = "Create Character",
                Location = new Point(130, 320),
                Size = new Size(120, 30),
                BackColor = Color.Green,
                ForeColor = Color.White,
                DialogResult = DialogResult.OK,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right
            };
            createButton.Click += CreateButton_Click;

            cancelButton = new Button
            {
                Text = "Cancel",
                Location = new Point(260, 320),
                Size = new Size(80, 30),
                BackColor = Color.Gray,
                ForeColor = Color.White,
                DialogResult = DialogResult.Cancel,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right
            };

            // Add controls to form
            this.Controls.AddRange(new Control[] {
                titleLabel, nameLabel, nameTextBox, classLabel, classComboBox,
                statsLabel, createButton, cancelButton
            });

            // Update stats display initially
            UpdateStatsDisplay();
        }

        private void ClassComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateStatsDisplay();
        }

        private void UpdateStatsDisplay()
        {
            string selectedClassName = classComboBox.SelectedItem?.ToString() ?? "";
            var classInfo = dataLoader.GetClassInfo(selectedClassName);
            
            if (classInfo != null)
            {
                var startingItems = GetStartingItemsDisplay(classInfo);

                statsLabel.Text = $"Class: {classInfo.Name}\n\n" +
                                 $"{classInfo.Description}\n\n" +
                                 $"Base Stats:\n" +
                                 $"Health: {classInfo.BaseStats.MaxHealth}\n" +
                                 $"Attack: {classInfo.BaseStats.Attack}\n" +
                                 $"Defense: {classInfo.BaseStats.Defense}\n\n" +
                                 $"Starting Items:\n{startingItems}";
            }
        }

        private string GetStartingItemsDisplay(CharacterClassInfo classInfo)
        {
            var itemNames = classInfo.StartingItems.Select(itemId =>
            {
                var item = dataLoader.CreateItemFromId(itemId);
                return item?.Name ?? itemId;
            });

            return string.Join("\n", itemNames.Select(name => $"• {name}"));
        }

        private void CreateButton_Click(object sender, EventArgs e)
        {
            string characterName = nameTextBox.Text.Trim();
            
            if (string.IsNullOrEmpty(characterName))
            {
                MessageBox.Show("Please enter a character name.", "Invalid Name", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (characterName.Length > 20)
            {
                MessageBox.Show("Character name must be 20 characters or less.", "Name Too Long", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string selectedClassName = classComboBox.SelectedItem?.ToString() ?? "Warrior";
            CreatedCharacter = dataLoader.CreatePlayerFromClass(characterName, selectedClassName);
            
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
} 