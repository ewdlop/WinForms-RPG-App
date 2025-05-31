using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows.Forms;

namespace WinFormsApp1.Controls
{
    public partial class JsonAssetEditorControl : UserControl
    {
        private TreeView assetTreeView;
        private Panel editorPanel;
        private Label nodeNameLabel;
        private TextBox nodeValueTextBox;
        private Button saveNodeButton;
        private Button addNodeButton;
        private Button deleteNodeButton;
        private ComboBox nodeTypeComboBox;
        private Label nodePathLabel;
        private Button expandAllButton;
        private Button collapseAllButton;

        private string currentFilePath;
        private JsonDocument currentDocument;
        private Dictionary<TreeNode, JsonNodeInfo> nodeInfoMap;

        public event EventHandler<AssetModifiedEventArgs> AssetModified;

        public JsonAssetEditorControl()
        {
            nodeInfoMap = new Dictionary<TreeNode, JsonNodeInfo>();
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Size = new Size(800, 600);
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

            // Toolbar panel
            CreateToolbarPanel(mainLayout);

            // Tree view for JSON structure
            CreateTreeView(mainLayout);

            // Editor panel
            CreateEditorPanel(mainLayout);

            this.Controls.Add(mainLayout);
        }

        private void CreateToolbarPanel(TableLayoutPanel mainLayout)
        {
            Panel toolbarPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Height = 40,
                BackColor = Color.LightGray
            };

            expandAllButton = new Button
            {
                Text = "Expand All",
                Location = new Point(5, 5),
                Size = new Size(80, 30),
                BackColor = Color.DarkBlue,
                ForeColor = Color.White
            };
            expandAllButton.Click += (s, e) => assetTreeView.ExpandAll();

            collapseAllButton = new Button
            {
                Text = "Collapse All",
                Location = new Point(90, 5),
                Size = new Size(80, 30),
                BackColor = Color.DarkBlue,
                ForeColor = Color.White
            };
            collapseAllButton.Click += (s, e) => assetTreeView.CollapseAll();

            Label fileLabel = new Label
            {
                Text = "File: None loaded",
                Location = new Point(180, 10),
                Size = new Size(300, 20),
                Font = new Font("Arial", 9, FontStyle.Bold),
                ForeColor = Color.DarkGreen
            };
            fileLabel.Name = "fileLabel";

            toolbarPanel.Controls.AddRange(new Control[] {
                expandAllButton, collapseAllButton, fileLabel
            });

            mainLayout.Controls.Add(toolbarPanel, 0, 0);
            mainLayout.SetColumnSpan(toolbarPanel, 2);
        }

        private void CreateTreeView(TableLayoutPanel mainLayout)
        {
            assetTreeView = new TreeView
            {
                Dock = DockStyle.Fill,
                ShowLines = true,
                ShowPlusMinus = true,
                ShowRootLines = true,
                FullRowSelect = true,
                HideSelection = false,
                ImageList = CreateAssetImageList()
            };
            assetTreeView.AfterSelect += AssetTreeView_AfterSelect;
            assetTreeView.NodeMouseClick += AssetTreeView_NodeMouseClick;

            mainLayout.Controls.Add(assetTreeView, 0, 1);
        }

        private void CreateEditorPanel(TableLayoutPanel mainLayout)
        {
            editorPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.LightGray,
                Padding = new Padding(10)
            };

            // Node path label
            nodePathLabel = new Label
            {
                Text = "Path: Select a node",
                Location = new Point(10, 10),
                Size = new Size(300, 20),
                Font = new Font("Arial", 8),
                ForeColor = Color.DarkBlue
            };

            // Node name label and textbox
            Label nameLabel = new Label
            {
                Text = "Node Name:",
                Location = new Point(10, 40),
                Size = new Size(80, 20),
                Font = new Font("Arial", 9, FontStyle.Bold)
            };

            nodeNameLabel = new Label
            {
                Text = "None",
                Location = new Point(100, 40),
                Size = new Size(200, 20),
                Font = new Font("Arial", 9),
                ForeColor = Color.DarkGreen
            };

            // Node type
            Label typeLabel = new Label
            {
                Text = "Type:",
                Location = new Point(10, 70),
                Size = new Size(80, 20),
                Font = new Font("Arial", 9, FontStyle.Bold)
            };

            nodeTypeComboBox = new ComboBox
            {
                Location = new Point(100, 68),
                Size = new Size(150, 20),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            nodeTypeComboBox.Items.AddRange(new[] { "String", "Number", "Boolean", "Array", "Object", "Null" });
            nodeTypeComboBox.SelectedIndexChanged += NodeTypeComboBox_SelectedIndexChanged;

            // Node value
            Label valueLabel = new Label
            {
                Text = "Value:",
                Location = new Point(10, 100),
                Size = new Size(80, 20),
                Font = new Font("Arial", 9, FontStyle.Bold)
            };

            nodeValueTextBox = new TextBox
            {
                Location = new Point(10, 125),
                Size = new Size(300, 100),
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                Font = new Font("Consolas", 9)
            };

            // Action buttons
            saveNodeButton = new Button
            {
                Text = "Save Changes",
                Location = new Point(10, 240),
                Size = new Size(100, 30),
                BackColor = Color.Green,
                ForeColor = Color.White,
                Enabled = false
            };
            saveNodeButton.Click += SaveNodeButton_Click;

            addNodeButton = new Button
            {
                Text = "Add Child",
                Location = new Point(120, 240),
                Size = new Size(80, 30),
                BackColor = Color.Blue,
                ForeColor = Color.White,
                Enabled = false
            };
            addNodeButton.Click += AddNodeButton_Click;

            deleteNodeButton = new Button
            {
                Text = "Delete",
                Location = new Point(210, 240),
                Size = new Size(80, 30),
                BackColor = Color.Red,
                ForeColor = Color.White,
                Enabled = false
            };
            deleteNodeButton.Click += DeleteNodeButton_Click;

            // Help text
            Label helpLabel = new Label
            {
                Text = "💡 Tips:\n• Click nodes to edit values\n• Right-click for context menu\n• Arrays and Objects can have children\n• Changes are saved to JSON file",
                Location = new Point(10, 280),
                Size = new Size(300, 80),
                Font = new Font("Arial", 8),
                ForeColor = Color.DarkBlue
            };

            editorPanel.Controls.AddRange(new Control[] {
                nodePathLabel, nameLabel, nodeNameLabel, typeLabel, nodeTypeComboBox,
                valueLabel, nodeValueTextBox, saveNodeButton, addNodeButton, deleteNodeButton, helpLabel
            });

            mainLayout.Controls.Add(editorPanel, 1, 1);
        }

        private ImageList CreateAssetImageList()
        {
            ImageList imageList = new ImageList
            {
                ImageSize = new Size(16, 16),
                ColorDepth = ColorDepth.Depth32Bit
            };

            // Create icons for different JSON types
            var objectIcon = CreateIcon(Color.Blue);
            var arrayIcon = CreateIcon(Color.Purple);
            var stringIcon = CreateIcon(Color.Green);
            var numberIcon = CreateIcon(Color.Orange);
            var booleanIcon = CreateIcon(Color.Red);
            var nullIcon = CreateIcon(Color.Gray);

            imageList.Images.Add("object", objectIcon);
            imageList.Images.Add("array", arrayIcon);
            imageList.Images.Add("string", stringIcon);
            imageList.Images.Add("number", numberIcon);
            imageList.Images.Add("boolean", booleanIcon);
            imageList.Images.Add("null", nullIcon);

            return imageList;
        }

        private Bitmap CreateIcon(Color color)
        {
            var icon = new Bitmap(16, 16);
            using (var g = Graphics.FromImage(icon))
            {
                g.FillEllipse(new SolidBrush(color), 2, 2, 12, 12);
                g.DrawEllipse(Pens.Black, 2, 2, 12, 12);
            }
            return icon;
        }

        public void LoadJsonFile(string filePath)
        {
            try
            {
                currentFilePath = filePath;
                string jsonContent = File.ReadAllText(filePath);
                currentDocument = JsonDocument.Parse(jsonContent);

                // Update file label
                var fileLabel = this.Controls.Find("fileLabel", true).FirstOrDefault() as Label;
                if (fileLabel != null)
                {
                    fileLabel.Text = $"File: {Path.GetFileName(filePath)}";
                }

                // Clear and rebuild tree
                assetTreeView.Nodes.Clear();
                nodeInfoMap.Clear();

                // Build tree from JSON
                var rootNode = new TreeNode(Path.GetFileNameWithoutExtension(filePath))
                {
                    ImageKey = "object",
                    SelectedImageKey = "object"
                };

                var rootInfo = new JsonNodeInfo
                {
                    JsonPath = "$",
                    JsonElement = currentDocument.RootElement,
                    NodeType = GetJsonNodeType(currentDocument.RootElement)
                };

                nodeInfoMap[rootNode] = rootInfo;
                BuildTreeFromJson(rootNode, currentDocument.RootElement, "$");

                assetTreeView.Nodes.Add(rootNode);
                rootNode.Expand();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading JSON file: {ex.Message}", "Load Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BuildTreeFromJson(TreeNode parentNode, JsonElement element, string basePath)
        {
            switch (element.ValueKind)
            {
                case JsonValueKind.Object:
                    foreach (var property in element.EnumerateObject())
                    {
                        var childNode = new TreeNode(property.Name);
                        var childPath = $"{basePath}.{property.Name}";
                        
                        var childInfo = new JsonNodeInfo
                        {
                            JsonPath = childPath,
                            JsonElement = property.Value,
                            NodeType = GetJsonNodeType(property.Value),
                            PropertyName = property.Name
                        };

                        nodeInfoMap[childNode] = childInfo;
                        SetNodeIcon(childNode, property.Value);
                        parentNode.Nodes.Add(childNode);

                        BuildTreeFromJson(childNode, property.Value, childPath);
                    }
                    break;

                case JsonValueKind.Array:
                    int index = 0;
                    foreach (var item in element.EnumerateArray())
                    {
                        var childNode = new TreeNode($"[{index}]");
                        var childPath = $"{basePath}[{index}]";
                        
                        var childInfo = new JsonNodeInfo
                        {
                            JsonPath = childPath,
                            JsonElement = item,
                            NodeType = GetJsonNodeType(item),
                            ArrayIndex = index
                        };

                        nodeInfoMap[childNode] = childInfo;
                        SetNodeIcon(childNode, item);
                        parentNode.Nodes.Add(childNode);

                        BuildTreeFromJson(childNode, item, childPath);
                        index++;
                    }
                    break;
            }
        }

        private void SetNodeIcon(TreeNode node, JsonElement element)
        {
            string iconKey = GetJsonNodeType(element).ToLower();
            node.ImageKey = iconKey;
            node.SelectedImageKey = iconKey;
        }

        private string GetJsonNodeType(JsonElement element)
        {
            return element.ValueKind switch
            {
                JsonValueKind.Object => "Object",
                JsonValueKind.Array => "Array",
                JsonValueKind.String => "String",
                JsonValueKind.Number => "Number",
                JsonValueKind.True or JsonValueKind.False => "Boolean",
                JsonValueKind.Null => "Null",
                _ => "Unknown"
            };
        }

        private void AssetTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (nodeInfoMap.TryGetValue(e.Node, out var nodeInfo))
            {
                DisplayNodeInfo(nodeInfo, e.Node);
            }
        }

        private void AssetTreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                // Show context menu for right-click
                ShowContextMenu(e.Node, e.Location);
            }
        }

        private void DisplayNodeInfo(JsonNodeInfo nodeInfo, TreeNode node)
        {
            nodePathLabel.Text = $"Path: {nodeInfo.JsonPath}";
            nodeNameLabel.Text = nodeInfo.PropertyName ?? node.Text;
            nodeTypeComboBox.SelectedItem = nodeInfo.NodeType;

            // Display value based on type
            string displayValue = "";
            switch (nodeInfo.JsonElement.ValueKind)
            {
                case JsonValueKind.String:
                    displayValue = nodeInfo.JsonElement.GetString() ?? "";
                    break;
                case JsonValueKind.Number:
                    displayValue = nodeInfo.JsonElement.ToString();
                    break;
                case JsonValueKind.True:
                    displayValue = "true";
                    break;
                case JsonValueKind.False:
                    displayValue = "false";
                    break;
                case JsonValueKind.Null:
                    displayValue = "null";
                    break;
                case JsonValueKind.Object:
                    displayValue = "{ ... } (Object with properties)";
                    break;
                case JsonValueKind.Array:
                    displayValue = $"[ ... ] (Array with {nodeInfo.JsonElement.GetArrayLength()} items)";
                    break;
            }

            nodeValueTextBox.Text = displayValue;

            // Enable/disable buttons based on node type
            bool canEdit = nodeInfo.JsonElement.ValueKind != JsonValueKind.Object && 
                          nodeInfo.JsonElement.ValueKind != JsonValueKind.Array;
            
            saveNodeButton.Enabled = canEdit;
            addNodeButton.Enabled = nodeInfo.JsonElement.ValueKind == JsonValueKind.Object || 
                                   nodeInfo.JsonElement.ValueKind == JsonValueKind.Array;
            deleteNodeButton.Enabled = node.Parent != null; // Can't delete root
        }

        private void ShowContextMenu(TreeNode node, Point location)
        {
            var contextMenu = new ContextMenuStrip();
            
            contextMenu.Items.Add("Add Child", null, (s, e) => AddChildNode(node));
            contextMenu.Items.Add("Delete Node", null, (s, e) => DeleteNode(node));
            contextMenu.Items.Add("-"); // Separator
            contextMenu.Items.Add("Expand All Children", null, (s, e) => node.ExpandAll());
            contextMenu.Items.Add("Collapse All Children", null, (s, e) => node.Collapse());

            contextMenu.Show(assetTreeView, location);
        }

        private void NodeTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Update the node value textbox based on selected type
            string selectedType = nodeTypeComboBox.SelectedItem?.ToString();
            if (!string.IsNullOrEmpty(selectedType))
            {
                switch (selectedType)
                {
                    case "Boolean":
                        nodeValueTextBox.Text = "true";
                        break;
                    case "Number":
                        nodeValueTextBox.Text = "0";
                        break;
                    case "String":
                        nodeValueTextBox.Text = "";
                        break;
                    case "Null":
                        nodeValueTextBox.Text = "null";
                        break;
                    case "Array":
                        nodeValueTextBox.Text = "[ ... ] (Array)";
                        break;
                    case "Object":
                        nodeValueTextBox.Text = "{ ... } (Object)";
                        break;
                }
            }
        }

        private void SaveNodeButton_Click(object sender, EventArgs e)
        {
            SaveCurrentNode();
        }

        private void AddNodeButton_Click(object sender, EventArgs e)
        {
            if (assetTreeView.SelectedNode != null)
            {
                AddChildNode(assetTreeView.SelectedNode);
            }
        }

        private void DeleteNodeButton_Click(object sender, EventArgs e)
        {
            if (assetTreeView.SelectedNode != null)
            {
                DeleteNode(assetTreeView.SelectedNode);
            }
        }

        private void SaveCurrentNode()
        {
            // This would implement saving the current node's value
            // For now, we'll just show a message
            MessageBox.Show("Node value saved!\n\nNote: This is a demo. Full implementation would modify the JSON and save to file.", 
                "Save", MessageBoxButtons.OK, MessageBoxIcon.Information);
            
            AssetModified?.Invoke(this, new AssetModifiedEventArgs(currentFilePath, "Node value updated"));
        }

        private void AddChildNode(TreeNode parentNode)
        {
            if (nodeInfoMap.TryGetValue(parentNode, out var parentInfo))
            {
                var dialog = new AddNodeDialog(parentInfo.NodeType);
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    var newNode = new TreeNode(dialog.NodeName);
                    parentNode.Nodes.Add(newNode);
                    parentNode.Expand();
                    
                    MessageBox.Show($"Added new {dialog.NodeType} node: {dialog.NodeName}\n\nNote: This is a demo. Full implementation would modify the JSON.", 
                        "Node Added", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    AssetModified?.Invoke(this, new AssetModifiedEventArgs(currentFilePath, $"Added node: {dialog.NodeName}"));
                }
            }
        }

        private void DeleteNode(TreeNode node)
        {
            if (node.Parent == null)
            {
                MessageBox.Show("Cannot delete the root node.", "Delete Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var result = MessageBox.Show($"Are you sure you want to delete '{node.Text}' and all its children?", 
                "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            
            if (result == DialogResult.Yes)
            {
                string nodeName = node.Text;
                node.Remove();
                nodeInfoMap.Remove(node);
                
                MessageBox.Show($"Deleted node: {nodeName}\n\nNote: This is a demo. Full implementation would modify the JSON and save to file.", 
                    "Node Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                AssetModified?.Invoke(this, new AssetModifiedEventArgs(currentFilePath, $"Deleted node: {nodeName}"));
            }
        }

        public void SaveToFile()
        {
            if (string.IsNullOrEmpty(currentFilePath))
            {
                MessageBox.Show("No file loaded to save.", "Save Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // This would implement the actual JSON modification and file saving
            MessageBox.Show($"Asset file would be saved to:\n{currentFilePath}\n\nNote: This is a demo implementation.", 
                "Save Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }

    public class JsonNodeInfo
    {
        public string JsonPath { get; set; }
        public JsonElement JsonElement { get; set; }
        public string NodeType { get; set; }
        public string PropertyName { get; set; }
        public int? ArrayIndex { get; set; }
    }

    public class AssetModifiedEventArgs : EventArgs
    {
        public string FilePath { get; }
        public string ModificationDescription { get; }

        public AssetModifiedEventArgs(string filePath, string description)
        {
            FilePath = filePath;
            ModificationDescription = description;
        }
    }
} 