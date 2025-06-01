using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Linq;
using WinFormsApp1.Controls;
using WinFormsApp1.Constants;

namespace WinFormsApp1
{
    public partial class CharacterCreationDialog : Form
    {
        private TextBox nameTextBox;
        private ComboBox classComboBox;
        private CharacterStatsDisplayControl statsDisplay;
        private Button createButton;
        private Button cancelButton;
        private DataLoader dataLoader;
        private Label descriptionLabel;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Player CreatedCharacter { get; private set; }

        // Add property to match what GameEngine expects
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Player CreatedPlayer => CreatedCharacter;

        public CharacterCreationDialog()
        {
            InitializeComponent();
            dataLoader = new DataLoader();
            LoadCharacterClasses();
        }

        private void InitializeComponent()
        {
            this.Text = "Create New Character";
            this.Size = new Size(400, 300);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Title label
            var titleLabel = new Label
            {
                Text = "Create Your Character",
                Font = new Font("Arial", 14, FontStyle.Bold),
                Location = new Point(20, 20),
                Size = new Size(350, 30),
                TextAlign = ContentAlignment.MiddleCenter
            };
            this.Controls.Add(titleLabel);

            // Name label and textbox
            var nameLabel = new Label
            {
                Text = "Character Name:",
                Location = new Point(20, 60),
                Size = new Size(100, 20)
            };
            this.Controls.Add(nameLabel);

            nameTextBox = new TextBox
            {
                Location = new Point(130, 58),
                Size = new Size(200, 20),
                MaxLength = 20
            };
            nameTextBox.TextChanged += NameTextBox_TextChanged;
            this.Controls.Add(nameTextBox);

            // Class label and combobox
            var classLabel = new Label
            {
                Text = "Character Class:",
                Location = new Point(20, 90),
                Size = new Size(100, 20)
            };
            this.Controls.Add(classLabel);

            classComboBox = new ComboBox
            {
                Location = new Point(130, 88),
                Size = new Size(200, 20),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            classComboBox.SelectedIndexChanged += ClassComboBox_SelectedIndexChanged;
            this.Controls.Add(classComboBox);

            // Description label
            descriptionLabel = new Label
            {
                Location = new Point(20, 120),
                Size = new Size(350, 80),
                Text = "Select a character class to see its description.",
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.LightGray
            };
            this.Controls.Add(descriptionLabel);

            // OK button
            createButton = new Button
            {
                Text = "Create Character",
                Location = new Point(130, 220),
                Size = new Size(120, 30),
                Enabled = false
            };
            createButton.Click += CreateButton_Click;
            this.Controls.Add(createButton);

            // Cancel button
            cancelButton = new Button
            {
                Text = "Cancel",
                Location = new Point(260, 220),
                Size = new Size(80, 30),
                DialogResult = DialogResult.Cancel
            };
            this.Controls.Add(cancelButton);

            this.AcceptButton = createButton;
            this.CancelButton = cancelButton;
        }

        private void LoadCharacterClasses()
        {
            var classes = dataLoader.LoadCharacterClasses();
            
            foreach (var classInfo in classes)
            {
                classComboBox.Items.Add(classInfo);
            }

            if (classComboBox.Items.Count > 0)
            {
                classComboBox.SelectedIndex = 0;
            }
        }

        private void NameTextBox_TextChanged(object sender, EventArgs e)
        {
            UpdateOkButton();
        }

        private void ClassComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (classComboBox.SelectedItem is CharacterClassInfo selectedClass)
            {
                descriptionLabel.Text = $"{selectedClass.Name}\n\n{selectedClass.Description}\n\n" +
                    $"Starting Stats:\n" +
                    $"Health: {selectedClass.StartingHealth}\n" +
                    $"Attack: {selectedClass.StartingAttack}\n" +
                    $"Defense: {selectedClass.StartingDefense}";
            }
            UpdateOkButton();
        }

        private void UpdateOkButton()
        {
            createButton.Enabled = !string.IsNullOrWhiteSpace(nameTextBox.Text) && 
                                  classComboBox.SelectedItem != null;
        }

        private void CreateButton_Click(object sender, EventArgs e)
        {
            if (classComboBox.SelectedItem is CharacterClassInfo selectedClass)
            {
                CreatedCharacter = CreatePlayerFromClass(nameTextBox.Text.Trim(), selectedClass);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private Player CreatePlayerFromClass(string name, CharacterClassInfo classInfo)
        {
            var player = new Player
            {
                Name = name,
                CharacterClass = classInfo.Class,
                Level = 1,
                MaxHealth = classInfo.StartingHealth,
                Health = classInfo.StartingHealth,
                Attack = classInfo.StartingAttack,
                Defense = classInfo.StartingDefense,
                Experience = 0,
                ExperienceToNextLevel = 100,
                Gold = 50,
                SkillPoints = 0
            };

            // Clear default inventory and add class-specific items
            player.Inventory.Clear();
            foreach (var item in classInfo.StartingItems)
            {
                player.Inventory.Add(new Item(item.Name, item.Description, item.Type, item.Value, item.Price));
            }

            // Add a basic health potion for all classes
            player.Inventory.Add(new Item(GameConstants.HEALTH_POTION, "Restores 20 health", ItemType.Potion, 20, 15));

            return player;
        }
    }
} 