using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using WinFormsApp1.Interfaces;
using Microsoft.Extensions.Logging;

namespace WinFormsApp1
{
    public partial class InventoryForm : Form
    {
        private readonly IPlayerManager _playerManager;
        private readonly IInventoryManager _inventoryManager;
        private readonly ILogger<InventoryForm> _logger;
        
        // UI Controls
        private ListView itemListView;
        private Panel detailsPanel;
        private Label itemNameLabel;
        private Label itemDescriptionLabel;
        private Label itemStatsLabel;
        private Button useButton;
        private Button equipButton;
        private Button dropButton;
        private Button closeButton;

        public InventoryForm(IPlayerManager playerManager, IInventoryManager inventoryManager, ILogger<InventoryForm> logger)
        {
            _playerManager = playerManager;
            _inventoryManager = inventoryManager;
            _logger = logger;
            
            InitializeComponent();
            LoadInventory();
        }

        private void InitializeComponent()
        {
            this.Text = "Inventory Management";
            this.Size = new Size(600, 450);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MinimumSize = new Size(500, 400);

            // Create main layout
            TableLayoutPanel mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 2,
                Padding = new Padding(10)
            };

            // Set column and row styles
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60F));
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 85F));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 15F));

            // Create inventory list
            CreateInventoryList();
            mainLayout.Controls.Add(itemListView, 0, 0);

            // Create details panel
            CreateDetailsPanel();
            mainLayout.Controls.Add(detailsPanel, 1, 0);

            // Create button panel
            Panel buttonPanel = CreateButtonPanel();
            mainLayout.SetColumnSpan(buttonPanel, 2);
            mainLayout.Controls.Add(buttonPanel, 0, 1);

            this.Controls.Add(mainLayout);
        }

        private void CreateInventoryList()
        {
            GroupBox inventoryGroup = new GroupBox
            {
                Text = "Inventory Items",
                Dock = DockStyle.Fill,
                Padding = new Padding(5)
            };

            itemListView = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                MultiSelect = false
            };

            // Add columns
            itemListView.Columns.Add("Name", 150);
            itemListView.Columns.Add("Type", 80);
            itemListView.Columns.Add("Value", 60);
            itemListView.Columns.Add("Qty", 40);

            itemListView.SelectedIndexChanged += ItemListView_SelectedIndexChanged;
            itemListView.DoubleClick += ItemListView_DoubleClick;

            inventoryGroup.Controls.Add(itemListView);
        }

        private void CreateDetailsPanel()
        {
            detailsPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10)
            };

            TableLayoutPanel detailsLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4
            };

            detailsLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            detailsLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            detailsLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            detailsLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            // Item name
            itemNameLabel = new Label
            {
                Text = "Select an item",
                Font = new Font("Arial", 12, FontStyle.Bold),
                ForeColor = Color.DarkBlue,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };

            // Item description
            itemDescriptionLabel = new Label
            {
                Text = "",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.TopLeft,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.LightGray,
                Padding = new Padding(5)
            };

            // Item stats
            itemStatsLabel = new Label
            {
                Text = "",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.TopLeft,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.LightYellow,
                Padding = new Padding(5),
                Font = new Font("Consolas", 9)
            };

            // Action buttons panel
            Panel actionPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Height = 40
            };

            useButton = new Button
            {
                Text = "Use",
                Size = new Size(60, 30),
                Location = new Point(0, 5),
                BackColor = Color.Green,
                ForeColor = Color.White,
                Enabled = false
            };
            useButton.Click += UseButton_Click;

            equipButton = new Button
            {
                Text = "Equip",
                Size = new Size(60, 30),
                Location = new Point(70, 5),
                BackColor = Color.Blue,
                ForeColor = Color.White,
                Enabled = false
            };
            equipButton.Click += EquipButton_Click;

            dropButton = new Button
            {
                Text = "Drop",
                Size = new Size(60, 30),
                Location = new Point(140, 5),
                BackColor = Color.Red,
                ForeColor = Color.White,
                Enabled = false
            };
            dropButton.Click += DropButton_Click;

            actionPanel.Controls.AddRange(new Control[] { useButton, equipButton, dropButton });

            detailsLayout.Controls.Add(itemNameLabel, 0, 0);
            detailsLayout.Controls.Add(itemDescriptionLabel, 0, 1);
            detailsLayout.Controls.Add(itemStatsLabel, 0, 2);
            detailsLayout.Controls.Add(actionPanel, 0, 3);

            detailsPanel.Controls.Add(detailsLayout);
        }

        private Panel CreateButtonPanel()
        {
            Panel buttonPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Height = 50
            };

            closeButton = new Button
            {
                Text = "Close",
                Size = new Size(80, 35),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                BackColor = Color.Gray,
                ForeColor = Color.White,
                DialogResult = DialogResult.OK
            };
            closeButton.Location = new Point(buttonPanel.Width - 90, 10);
            closeButton.Click += (s, e) => this.Close();

            Label goldLabel = new Label
            {
                Text = $"Gold: {_playerManager.CurrentPlayer.Gold}",
                Font = new Font("Arial", 10, FontStyle.Bold),
                ForeColor = Color.Goldenrod,
                AutoSize = true,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left
            };
            goldLabel.Location = new Point(10, 15);

            buttonPanel.Controls.AddRange(new Control[] { goldLabel, closeButton });
            return buttonPanel;
        }

        private void LoadInventory()
        {
            itemListView.Items.Clear();

            var groupedItems = _playerManager.CurrentPlayer.Inventory.GroupBy(item => item.Name).ToList();

            foreach (var group in groupedItems)
            {
                var item = group.First();
                var quantity = group.Count();

                ListViewItem listItem = new ListViewItem(item.Name);
                listItem.SubItems.Add(item.Type.ToString());
                listItem.SubItems.Add(item.Value.ToString());
                listItem.SubItems.Add(quantity.ToString());
                listItem.Tag = item;

                // Color code by item type
                switch (item.Type)
                {
                    case ItemType.Weapon:
                        listItem.BackColor = Color.LightCoral;
                        break;
                    case ItemType.Armor:
                        listItem.BackColor = Color.LightBlue;
                        break;
                    case ItemType.Potion:
                        listItem.BackColor = Color.LightGreen;
                        break;
                    case ItemType.Key:
                        listItem.BackColor = Color.Gold;
                        break;
                    default:
                        listItem.BackColor = Color.LightGray;
                        break;
                }

                itemListView.Items.Add(listItem);
            }
        }

        private void ItemListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (itemListView.SelectedItems.Count > 0)
            {
                var selectedItem = (Item)itemListView.SelectedItems[0].Tag;
                DisplayItemDetails(selectedItem);
                EnableActionButtons(selectedItem);
            }
            else
            {
                ClearItemDetails();
                DisableActionButtons();
            }
        }

        private void ItemListView_DoubleClick(object sender, EventArgs e)
        {
            if (itemListView.SelectedItems.Count > 0)
            {
                var selectedItem = (Item)itemListView.SelectedItems[0].Tag;
                if (selectedItem.Type == ItemType.Potion)
                {
                    UseItem(selectedItem);
                }
                else if (selectedItem.Type == ItemType.Weapon || selectedItem.Type == ItemType.Armor)
                {
                    EquipItem(selectedItem);
                }
            }
        }

        private void DisplayItemDetails(Item item)
        {
            itemNameLabel.Text = item.Name;
            itemDescriptionLabel.Text = item.Description;

            string stats = $"Type: {item.Type}\n";
            stats += $"Value: {item.Value}\n";
            if (item.Price > 0)
                stats += $"Price: {item.Price} gold\n";
            stats += $"Stackable: {(item.IsStackable ? "Yes" : "No")}\n";

            if (item.Type == ItemType.Weapon)
                stats += $"Attack Power: +{item.Value}";
            else if (item.Type == ItemType.Armor)
                stats += $"Defense: +{item.Value}";
            else if (item.Type == ItemType.Potion)
                stats += $"Healing: {item.Value} HP";

            itemStatsLabel.Text = stats;
        }

        private void ClearItemDetails()
        {
            itemNameLabel.Text = "Select an item";
            itemDescriptionLabel.Text = "";
            itemStatsLabel.Text = "";
        }

        private void EnableActionButtons(Item item)
        {
            useButton.Enabled = item.Type == ItemType.Potion;
            equipButton.Enabled = item.Type == ItemType.Weapon || item.Type == ItemType.Armor;
            dropButton.Enabled = true;
        }

        private void DisableActionButtons()
        {
            useButton.Enabled = false;
            equipButton.Enabled = false;
            dropButton.Enabled = false;
        }

        private void UseButton_Click(object sender, EventArgs e)
        {
            if (itemListView.SelectedItems.Count > 0)
            {
                var selectedItem = (Item)itemListView.SelectedItems[0].Tag;
                UseItem(selectedItem);
            }
        }

        private void EquipButton_Click(object sender, EventArgs e)
        {
            if (itemListView.SelectedItems.Count > 0)
            {
                var selectedItem = (Item)itemListView.SelectedItems[0].Tag;
                EquipItem(selectedItem);
            }
        }

        private void DropButton_Click(object sender, EventArgs e)
        {
            if (itemListView.SelectedItems.Count > 0)
            {
                var selectedItem = (Item)itemListView.SelectedItems[0].Tag;
                DropItem(selectedItem);
            }
        }

        private void UseItem(Item item)
        {
            if (item.Type == ItemType.Potion)
            {
                int oldHealth = _playerManager.CurrentPlayer.Health;
                _playerManager.CurrentPlayer.Health = Math.Min(_playerManager.CurrentPlayer.MaxHealth, _playerManager.CurrentPlayer.Health + item.Value);
                int healedAmount = _playerManager.CurrentPlayer.Health - oldHealth;

                _playerManager.CurrentPlayer.Inventory.Remove(item);
                LoadInventory();
                ClearItemDetails();
                DisableActionButtons();

                MessageBox.Show($"You used {item.Name} and restored {healedAmount} health!", 
                    "Item Used", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void EquipItem(Item item)
        {
            if (item.Type == ItemType.Weapon)
            {
                if (_playerManager.CurrentPlayer.EquippedWeapon != null)
                {
                    _playerManager.CurrentPlayer.Inventory.Add(_playerManager.CurrentPlayer.EquippedWeapon);
                }
                _playerManager.CurrentPlayer.EquippedWeapon = item;
                _playerManager.CurrentPlayer.Inventory.Remove(item);
                
                MessageBox.Show($"You equipped {item.Name}!", "Item Equipped", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (item.Type == ItemType.Armor)
            {
                if (_playerManager.CurrentPlayer.EquippedArmor != null)
                {
                    _playerManager.CurrentPlayer.Inventory.Add(_playerManager.CurrentPlayer.EquippedArmor);
                }
                _playerManager.CurrentPlayer.EquippedArmor = item;
                _playerManager.CurrentPlayer.Inventory.Remove(item);
                
                MessageBox.Show($"You equipped {item.Name}!", "Item Equipped", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            LoadInventory();
            ClearItemDetails();
            DisableActionButtons();
        }

        private void DropItem(Item item)
        {
            var result = MessageBox.Show($"Are you sure you want to drop {item.Name}?", 
                "Drop Item", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                _playerManager.CurrentPlayer.Inventory.Remove(item);
                LoadInventory();
                ClearItemDetails();
                DisableActionButtons();

                MessageBox.Show($"You dropped {item.Name}.", "Item Dropped", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
} 