using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;

namespace WinFormsApp1
{
    public partial class AddNodeDialog : Form
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string NodeName { get; private set; }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string NodeType { get; private set; }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string NodeValue { get; private set; }

        private TextBox nameTextBox;
        private ComboBox typeComboBox;
        private TextBox valueTextBox;
        private Button okButton;
        private Button cancelButton;

        public AddNodeDialog(string parentType)
        {
            InitializeComponent();
            SetupForParentType(parentType);
        }

        private void InitializeComponent()
        {
            this.Text = "Add New Node";
            this.Size = new Size(400, 300);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Main layout
            TableLayoutPanel mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 5,
                Padding = new Padding(15)
            };

            // Set column styles
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            // Name field
            Label nameLabel = new Label
            {
                Text = "Node Name:",
                Anchor = AnchorStyles.Left,
                AutoSize = true,
                Font = new Font("Arial", 9, FontStyle.Bold)
            };

            nameTextBox = new TextBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Arial", 9)
            };

            // Type field
            Label typeLabel = new Label
            {
                Text = "Node Type:",
                Anchor = AnchorStyles.Left,
                AutoSize = true,
                Font = new Font("Arial", 9, FontStyle.Bold)
            };

            typeComboBox = new ComboBox
            {
                Dock = DockStyle.Fill,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Arial", 9)
            };
            typeComboBox.Items.AddRange(new[] { "String", "Number", "Boolean", "Array", "Object", "Null" });
            typeComboBox.SelectedIndex = 0; // Default to String
            typeComboBox.SelectedIndexChanged += TypeComboBox_SelectedIndexChanged;

            // Value field
            Label valueLabel = new Label
            {
                Text = "Initial Value:",
                Anchor = AnchorStyles.Left,
                AutoSize = true,
                Font = new Font("Arial", 9, FontStyle.Bold)
            };

            valueTextBox = new TextBox
            {
                Dock = DockStyle.Fill,
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                Height = 60,
                Font = new Font("Consolas", 9)
            };

            // Help text
            Label helpLabel = new Label
            {
                Text = "💡 Tip: For arrays and objects, the initial value will be empty. You can add children after creation.",
                Dock = DockStyle.Fill,
                Font = new Font("Arial", 8),
                ForeColor = Color.DarkBlue,
                AutoSize = false
            };

            // Button panel
            Panel buttonPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Height = 40
            };

            okButton = new Button
            {
                Text = "Add Node",
                Size = new Size(100, 30),
                Location = new Point(0, 5),
                BackColor = Color.Green,
                ForeColor = Color.White,
                DialogResult = DialogResult.OK
            };
            okButton.Click += OkButton_Click;

            cancelButton = new Button
            {
                Text = "Cancel",
                Size = new Size(80, 30),
                Location = new Point(110, 5),
                BackColor = Color.Gray,
                ForeColor = Color.White,
                DialogResult = DialogResult.Cancel
            };

            buttonPanel.Controls.AddRange(new Control[] { okButton, cancelButton });

            // Add to layout
            mainLayout.Controls.Add(nameLabel, 0, 0);
            mainLayout.Controls.Add(nameTextBox, 1, 0);
            mainLayout.Controls.Add(typeLabel, 0, 1);
            mainLayout.Controls.Add(typeComboBox, 1, 1);
            mainLayout.Controls.Add(valueLabel, 0, 2);
            mainLayout.Controls.Add(valueTextBox, 1, 2);
            mainLayout.Controls.Add(helpLabel, 0, 3);
            mainLayout.SetColumnSpan(helpLabel, 2);
            mainLayout.Controls.Add(buttonPanel, 0, 4);
            mainLayout.SetColumnSpan(buttonPanel, 2);

            this.Controls.Add(mainLayout);

            // Focus the name textbox
            this.ActiveControl = nameTextBox;
        }

        private void SetupForParentType(string parentType)
        {
            switch (parentType)
            {
                case "Array":
                    // For arrays, the node name will be the index, so make it read-only
                    nameTextBox.Text = "[New Item]";
                    nameTextBox.ReadOnly = true;
                    nameTextBox.BackColor = Color.LightGray;
                    break;
                case "Object":
                    // For objects, user needs to specify property name
                    nameTextBox.PlaceholderText = "Enter property name...";
                    break;
                default:
                    // This shouldn't happen, but handle gracefully
                    nameTextBox.PlaceholderText = "Enter node name...";
                    break;
            }
        }

        private void TypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Update the value textbox based on selected type
            string selectedType = typeComboBox.SelectedItem?.ToString();
            if (!string.IsNullOrEmpty(selectedType))
            {
                switch (selectedType)
                {
                    case "String":
                        valueTextBox.Text = "";
                        valueTextBox.PlaceholderText = "Enter string value...";
                        break;
                    case "Number":
                        valueTextBox.Text = "0";
                        valueTextBox.PlaceholderText = "Enter numeric value...";
                        break;
                    case "Boolean":
                        valueTextBox.Text = "true";
                        valueTextBox.PlaceholderText = "true or false";
                        break;
                    case "Array":
                        valueTextBox.Text = "[]";
                        valueTextBox.ReadOnly = true;
                        valueTextBox.BackColor = Color.LightGray;
                        valueTextBox.PlaceholderText = "Array will be empty initially";
                        break;
                    case "Object":
                        valueTextBox.Text = "{}";
                        valueTextBox.ReadOnly = true;
                        valueTextBox.BackColor = Color.LightGray;
                        valueTextBox.PlaceholderText = "Object will be empty initially";
                        break;
                    case "Null":
                        valueTextBox.Text = "null";
                        valueTextBox.ReadOnly = true;
                        valueTextBox.BackColor = Color.LightGray;
                        valueTextBox.PlaceholderText = "Null value";
                        break;
                }
            }
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(nameTextBox.Text))
            {
                MessageBox.Show("Please enter a node name.", "Validation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                nameTextBox.Focus();
                return;
            }

            if (typeComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please select a node type.", "Validation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                typeComboBox.Focus();
                return;
            }

            // Validate value based on type
            string selectedType = typeComboBox.SelectedItem.ToString();
            string value = valueTextBox.Text;

            if (!ValidateValueForType(selectedType, value))
            {
                return;
            }

            // Set properties
            NodeName = nameTextBox.Text;
            NodeType = selectedType;
            NodeValue = value;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private bool ValidateValueForType(string type, string value)
        {
            switch (type)
            {
                case "Number":
                    if (!double.TryParse(value, out _))
                    {
                        MessageBox.Show("Please enter a valid number.", "Validation Error", 
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        valueTextBox.Focus();
                        return false;
                    }
                    break;

                case "Boolean":
                    if (value.ToLower() != "true" && value.ToLower() != "false")
                    {
                        MessageBox.Show("Boolean value must be 'true' or 'false'.", "Validation Error", 
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        valueTextBox.Focus();
                        return false;
                    }
                    break;

                case "String":
                    // Strings can be anything, including empty
                    break;

                case "Array":
                case "Object":
                case "Null":
                    // These are handled automatically
                    break;
            }

            return true;
        }
    }
} 