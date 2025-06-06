using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Linq;
using WinFormsApp1.Controls;

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

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Player CreatedCharacter { get; private set; }

        // Add property to match what GameEngine expects
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Player CreatedPlayer => CreatedCharacter;

        public CharacterCreationDialog() : this(new DataLoader())
        {
            // Default constructor that creates its own DataLoader
        }

        public CharacterCreationDialog(DataLoader dataLoader)
        {
            this.dataLoader = dataLoader;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Create Your Character";
            this.Size = new Size(400, 520);
            this.MinimumSize = new Size(380, 500);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MaximizeBox = true;
            this.MinimizeBox = false;

            // Load data if not already loaded
            try
            {
                dataLoader.LoadAllData();
            }
            catch
            {
                // If loading fails, we'll use fallback classes
            }

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

            // Load character classes from JSON data with fallback
            try
            {
                var availableClasses = dataLoader.GetAvailableClasses();
                classComboBox.Items.AddRange(availableClasses.Select(c => c.Name).ToArray());
            }
            catch
            {
                // Fallback to hardcoded classes if JSON loading fails
                classComboBox.Items.AddRange(new string[] { "Warrior", "Mage", "Rogue", "Cleric" });
            }

            if (classComboBox.Items.Count > 0)
            {
                classComboBox.SelectedIndex = 0;
            }
            classComboBox.SelectedIndexChanged += ClassComboBox_SelectedIndexChanged;

            // Stats display using custom control
            statsDisplay = new CharacterStatsDisplayControl
            {
                Location = new Point(20, 150),
                Size = new Size(350, 230),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };

            // Buttons - positioned lower with more space
            createButton = new Button
            {
                Text = "Create Character",
                Location = new Point(130, 400),
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
                Location = new Point(260, 400),
                Size = new Size(80, 30),
                BackColor = Color.Gray,
                ForeColor = Color.White,
                DialogResult = DialogResult.Cancel,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right
            };

            // Add controls to form
            this.Controls.AddRange(new Control[] {
                titleLabel, nameLabel, nameTextBox, classLabel, classComboBox,
                statsDisplay, createButton, cancelButton
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
            
            try
            {
                var classInfo = dataLoader.GetClassInfo(selectedClassName);
                statsDisplay.UpdateDisplay(classInfo, dataLoader);
            }
            catch
            {
                // Fallback display if data loading fails
                statsDisplay.UpdateDisplay(CreateFallbackClassInfo(selectedClassName), dataLoader);
            }
        }

        private CharacterClassInfo CreateFallbackClassInfo(string className)
        {
            // Create fallback class info for basic functionality
            var fallbackClass = new CharacterClassInfo
            {
                Name = className,
                Description = $"A brave {className.ToLower()}",
                BaseStats = new BaseStats
                {
                    MaxHealth = 100,
                    Attack = 10,
                    Defense = 5
                },
                StartingItems = new System.Collections.Generic.List<string>()
            };

            switch (className.ToLower())
            {
                case "warrior":
                    fallbackClass.BaseStats.Attack = 15;
                    fallbackClass.BaseStats.Defense = 10;
                    fallbackClass.StartingItems.Add("Iron Sword");
                    fallbackClass.StartingItems.Add("Leather Armor");
                    break;
                case "mage":
                    fallbackClass.BaseStats.Attack = 8;
                    fallbackClass.BaseStats.Defense = 3;
                    fallbackClass.StartingItems.Add("Magic Staff");
                    fallbackClass.StartingItems.Add("Mana Potion");
                    break;
                case "rogue":
                    fallbackClass.BaseStats.Attack = 12;
                    fallbackClass.BaseStats.Defense = 6;
                    fallbackClass.StartingItems.Add("Dagger");
                    fallbackClass.StartingItems.Add("Lockpicks");
                    break;
                case "cleric":
                    fallbackClass.BaseStats.Attack = 9;
                    fallbackClass.BaseStats.Defense = 8;
                    fallbackClass.StartingItems.Add("Holy Symbol");
                    fallbackClass.StartingItems.Add("Health Potion");
                    break;
            }

            return fallbackClass;
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
            
            try
            {
                CreatedCharacter = dataLoader.CreatePlayerFromClass(characterName, selectedClassName);
            }
            catch
            {
                // Fallback character creation if data loading fails
                CreatedCharacter = CreateFallbackPlayer(characterName, selectedClassName);
            }
            
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private Player CreateFallbackPlayer(string name, string className)
        {
            var player = new Player(name, (CharacterClass)Enum.Parse(typeof(CharacterClass), className, true));
            
            // Set basic stats based on class
            switch (className.ToLower())
            {
                case "warrior":
                    player.Attack = 15;
                    player.Defense = 10;
                    player.MaxHealth = 120;
                    break;
                case "mage":
                    player.Attack = 8;
                    player.Defense = 3;
                    player.MaxHealth = 80;
                    break;
                case "rogue":
                    player.Attack = 12;
                    player.Defense = 6;
                    player.MaxHealth = 100;
                    break;
                case "cleric":
                    player.Attack = 9;
                    player.Defense = 8;
                    player.MaxHealth = 110;
                    break;
                default:
                    player.Attack = 10;
                    player.Defense = 5;
                    player.MaxHealth = 100;
                    break;
            }

            player.Health = player.MaxHealth;
            return player;
        }
    }
} 