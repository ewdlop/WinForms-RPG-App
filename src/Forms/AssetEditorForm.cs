using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using WinFormsApp1.Controls;

namespace WinFormsApp1
{
    public partial class AssetEditorForm : Form
    {
        private JsonAssetEditorControl editorControl;
        private ComboBox fileComboBox;
        private Button loadButton;
        private Button saveButton;
        private Button refreshButton;
        private StatusStrip statusStrip;
        private ToolStripStatusLabel statusLabel;
        private string assetsDirectory;

        public AssetEditorForm()
        {
            assetsDirectory = Path.Combine(Application.StartupPath, "Assets", "Data");
            InitializeComponent();
            LoadAvailableFiles();
        }

        private void InitializeComponent()
        {
            this.Text = "Game Asset Editor - JSON Data Management";
            this.Size = new Size(1000, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimumSize = new Size(800, 600);
            this.Icon = SystemIcons.Application;

            // Create main layout
            TableLayoutPanel mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                Padding = new Padding(5)
            };

            // Set row styles
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Toolbar
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); // Editor
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Status

            // Create toolbar
            CreateToolbar(mainLayout);

            // Create editor control
            CreateEditorControl(mainLayout);

            // Create status bar
            CreateStatusBar(mainLayout);

            this.Controls.Add(mainLayout);
        }

        private void CreateToolbar(TableLayoutPanel mainLayout)
        {
            Panel toolbarPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Height = 50,
                BackColor = Color.LightSteelBlue,
                Padding = new Padding(10, 5, 10, 5)
            };

            // File selection
            Label fileLabel = new Label
            {
                Text = "Asset File:",
                Location = new Point(10, 15),
                Size = new Size(70, 20),
                Font = new Font("Arial", 9, FontStyle.Bold),
                ForeColor = Color.DarkBlue
            };

            fileComboBox = new ComboBox
            {
                Location = new Point(85, 12),
                Size = new Size(250, 25),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Arial", 9)
            };

            // Action buttons
            loadButton = new Button
            {
                Text = "Load File",
                Location = new Point(345, 10),
                Size = new Size(80, 30),
                BackColor = Color.DarkBlue,
                ForeColor = Color.White,
                Font = new Font("Arial", 9, FontStyle.Bold)
            };
            loadButton.Click += LoadButton_Click;

            saveButton = new Button
            {
                Text = "Save File",
                Location = new Point(435, 10),
                Size = new Size(80, 30),
                BackColor = Color.DarkGreen,
                ForeColor = Color.White,
                Font = new Font("Arial", 9, FontStyle.Bold),
                Enabled = false
            };
            saveButton.Click += SaveButton_Click;

            refreshButton = new Button
            {
                Text = "Refresh",
                Location = new Point(525, 10),
                Size = new Size(80, 30),
                BackColor = Color.DarkOrange,
                ForeColor = Color.White,
                Font = new Font("Arial", 9, FontStyle.Bold)
            };
            refreshButton.Click += RefreshButton_Click;

            // Help label
            Label helpLabel = new Label
            {
                Text = "💡 Edit game assets like items, enemies, and locations using the tree view below",
                Location = new Point(620, 15),
                Size = new Size(350, 20),
                Font = new Font("Arial", 8),
                ForeColor = Color.DarkBlue
            };

            toolbarPanel.Controls.AddRange(new Control[] {
                fileLabel, fileComboBox, loadButton, saveButton, refreshButton, helpLabel
            });

            mainLayout.Controls.Add(toolbarPanel, 0, 0);
        }

        private void CreateEditorControl(TableLayoutPanel mainLayout)
        {
            editorControl = new JsonAssetEditorControl
            {
                Dock = DockStyle.Fill
            };
            editorControl.AssetModified += EditorControl_AssetModified;

            mainLayout.Controls.Add(editorControl, 0, 1);
        }

        private void CreateStatusBar(TableLayoutPanel mainLayout)
        {
            statusStrip = new StatusStrip
            {
                Dock = DockStyle.Fill
            };

            statusLabel = new ToolStripStatusLabel
            {
                Text = "Ready - Select a file to edit",
                Spring = true,
                TextAlign = ContentAlignment.MiddleLeft
            };

            statusStrip.Items.Add(statusLabel);
            statusStrip.Items.Add(new ToolStripStatusLabel("Asset Editor v1.0"));

            mainLayout.Controls.Add(statusStrip, 0, 2);
        }

        private void LoadAvailableFiles()
        {
            try
            {
                if (!Directory.Exists(assetsDirectory))
                {
                    statusLabel.Text = $"Assets directory not found: {assetsDirectory}";
                    return;
                }

                fileComboBox.Items.Clear();

                var jsonFiles = Directory.GetFiles(assetsDirectory, "*.json")
                    .Select(Path.GetFileName)
                    .OrderBy(f => f)
                    .ToArray();

                if (jsonFiles.Any())
                {
                    fileComboBox.Items.AddRange(jsonFiles);
                    statusLabel.Text = $"Found {jsonFiles.Length} asset files";
                }
                else
                {
                    statusLabel.Text = "No JSON files found in assets directory";
                }
            }
            catch (Exception ex)
            {
                statusLabel.Text = $"Error loading file list: {ex.Message}";
                MessageBox.Show($"Error accessing assets directory:\n{ex.Message}", "Directory Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void LoadButton_Click(object sender, EventArgs e)
        {
            if (fileComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please select a file to load.", "No File Selected",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                string fileName = fileComboBox.SelectedItem.ToString();
                string filePath = Path.Combine(assetsDirectory, fileName);

                if (!File.Exists(filePath))
                {
                    MessageBox.Show($"File not found: {filePath}", "File Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                editorControl.LoadJsonFile(filePath);
                saveButton.Enabled = true;
                statusLabel.Text = $"Loaded: {fileName}";

                // Show file info
                var fileInfo = new FileInfo(filePath);
                this.Text = $"Game Asset Editor - {fileName} ({fileInfo.Length:N0} bytes)";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading file:\n{ex.Message}", "Load Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                statusLabel.Text = "Failed to load file";
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            try
            {
                editorControl.SaveToFile();
                statusLabel.Text = "Asset file saved successfully";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving file:\n{ex.Message}", "Save Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                statusLabel.Text = "Failed to save file";
            }
        }

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            LoadAvailableFiles();
        }

        private void EditorControl_AssetModified(object sender, AssetModifiedEventArgs e)
        {
            // Update status when assets are modified
            statusLabel.Text = $"Modified: {e.ModificationDescription}";
            
            // Add asterisk to title to indicate unsaved changes
            if (!this.Text.EndsWith("*"))
            {
                this.Text += "*";
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Check for unsaved changes
            if (this.Text.EndsWith("*"))
            {
                var result = MessageBox.Show(
                    "You have unsaved changes. Do you want to save before closing?",
                    "Unsaved Changes",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question);

                switch (result)
                {
                    case DialogResult.Yes:
                        SaveButton_Click(this, EventArgs.Empty);
                        break;
                    case DialogResult.Cancel:
                        e.Cancel = true;
                        return;
                }
            }

            base.OnFormClosing(e);
        }

        public static void ShowAssetEditor()
        {
            try
            {
                var editorForm = new AssetEditorForm();
                editorForm.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening Asset Editor:\n{ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
} 