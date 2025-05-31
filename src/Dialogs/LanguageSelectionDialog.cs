using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using WinFormsApp1.Localization;

namespace WinFormsApp1.Dialogs
{
    public partial class LanguageSelectionDialog : Form
    {
        private ComboBox languageComboBox;
        private Label instructionLabel;
        private Label previewLabel;
        private Button okButton;
        private Button cancelButton;
        private Button applyButton;
        private GroupBox previewGroupBox;
        private Label sampleTextLabel;

        public string SelectedCulture { get; private set; }

        public LanguageSelectionDialog()
        {
            InitializeComponent();
            LoadLanguages();
            UpdatePreview();
        }

        private void InitializeComponent()
        {
            this.Text = LocalizationManager.GetString("Menu_Language");
            this.Size = new Size(450, 350);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ShowIcon = false;

            // Main layout
            TableLayoutPanel mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4,
                Padding = new Padding(12)
            };

            // Set row styles
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Instruction
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Language selection
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); // Preview
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Buttons

            // Instruction label
            instructionLabel = new Label
            {
                Text = "Select your preferred language:",
                Dock = DockStyle.Fill,
                AutoSize = true,
                Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                Margin = new Padding(0, 0, 0, 8)
            };
            mainLayout.Controls.Add(instructionLabel, 0, 0);

            // Language selection
            var selectionPanel = CreateLanguageSelectionPanel();
            mainLayout.Controls.Add(selectionPanel, 0, 1);

            // Preview group
            CreatePreviewGroup();
            mainLayout.Controls.Add(previewGroupBox, 0, 2);

            // Button panel
            var buttonPanel = CreateButtonPanel();
            mainLayout.Controls.Add(buttonPanel, 0, 3);

            this.Controls.Add(mainLayout);
        }

        private Panel CreateLanguageSelectionPanel()
        {
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                Height = 60,
                Margin = new Padding(0, 0, 0, 12)
            };

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            var label = new Label
            {
                Text = "Language:",
                AutoSize = true,
                Anchor = AnchorStyles.Left,
                Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                Margin = new Padding(0, 6, 12, 0)
            };
            layout.Controls.Add(label, 0, 0);

            languageComboBox = new ComboBox
            {
                Dock = DockStyle.Fill,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                Margin = new Padding(0, 3, 0, 0)
            };
            languageComboBox.SelectedIndexChanged += LanguageComboBox_SelectedIndexChanged;
            layout.Controls.Add(languageComboBox, 1, 0);

            panel.Controls.Add(layout);
            return panel;
        }

        private void CreatePreviewGroup()
        {
            previewGroupBox = new GroupBox
            {
                Text = "Preview",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                Margin = new Padding(0, 0, 0, 12)
            };

            var previewLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                Padding = new Padding(8)
            };
            previewLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            previewLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            previewLabel = new Label
            {
                Text = "Sample interface elements in selected language:",
                Dock = DockStyle.Fill,
                AutoSize = true,
                Font = new Font("Segoe UI", 8.25F, FontStyle.Italic),
                Margin = new Padding(0, 0, 0, 8)
            };
            previewLayout.Controls.Add(previewLabel, 0, 0);

            // Sample text panel
            var samplePanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            sampleTextLabel = new Label
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                Padding = new Padding(8),
                Text = GetSampleText()
            };
            samplePanel.Controls.Add(sampleTextLabel);

            previewLayout.Controls.Add(samplePanel, 0, 1);
            previewGroupBox.Controls.Add(previewLayout);
        }

        private Panel CreateButtonPanel()
        {
            var buttonPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Height = 35
            };

            var buttonLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 4,
                RowCount = 1
            };
            buttonLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F)); // Spacer
            buttonLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize)); // Apply
            buttonLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize)); // OK
            buttonLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize)); // Cancel

            applyButton = new Button
            {
                Text = "Apply",
                Size = new Size(75, 23),
                UseVisualStyleBackColor = true,
                Margin = new Padding(0, 0, 6, 0)
            };
            applyButton.Click += ApplyButton_Click;
            buttonLayout.Controls.Add(applyButton, 1, 0);

            okButton = new Button
            {
                Text = LocalizationManager.GetString("Common_OK"),
                Size = new Size(75, 23),
                UseVisualStyleBackColor = true,
                Margin = new Padding(0, 0, 6, 0)
            };
            okButton.Click += OkButton_Click;
            buttonLayout.Controls.Add(okButton, 2, 0);

            cancelButton = new Button
            {
                Text = LocalizationManager.GetString("Common_Cancel"),
                Size = new Size(75, 23),
                UseVisualStyleBackColor = true,
                DialogResult = DialogResult.Cancel
            };
            cancelButton.Click += CancelButton_Click;
            buttonLayout.Controls.Add(cancelButton, 3, 0);

            this.AcceptButton = okButton;
            this.CancelButton = cancelButton;

            buttonPanel.Controls.Add(buttonLayout);
            return buttonPanel;
        }

        private void LoadLanguages()
        {
            var supportedCultures = LocalizationManager.GetSupportedCultures();
            var currentCulture = LocalizationManager.GetCurrentCulture().Name;

            languageComboBox.Items.Clear();

            foreach (var cultureName in supportedCultures)
            {
                var item = new LanguageItem
                {
                    CultureName = cultureName,
                    DisplayName = LocalizationManager.GetCultureDisplayName(cultureName)
                };
                languageComboBox.Items.Add(item);

                if (cultureName == currentCulture)
                {
                    languageComboBox.SelectedItem = item;
                }
            }

            // Default to first item if nothing selected
            if (languageComboBox.SelectedItem == null && languageComboBox.Items.Count > 0)
            {
                languageComboBox.SelectedIndex = 0;
            }
        }

        private void LanguageComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdatePreview();
        }

        private void UpdatePreview()
        {
            if (languageComboBox.SelectedItem is LanguageItem selectedItem)
            {
                // Temporarily set culture for preview
                var originalCulture = LocalizationManager.GetCurrentCulture();
                LocalizationManager.SetCulture(selectedItem.CultureName);

                // Update preview text
                sampleTextLabel.Text = GetSampleText();
                previewLabel.Text = GetPreviewDescription();

                // Restore original culture
                LocalizationManager.SetCulture(originalCulture);
            }
        }

        private string GetSampleText()
        {
            return $"{LocalizationManager.GetString("Menu_Game")}\n" +
                   $"  {LocalizationManager.GetString("Menu_NewGame")}\n" +
                   $"  {LocalizationManager.GetString("Menu_SaveGame")}\n" +
                   $"  {LocalizationManager.GetString("Menu_LoadGame")}\n\n" +
                   $"{LocalizationManager.GetString("Menu_Character")}\n" +
                   $"  {LocalizationManager.GetString("Menu_Inventory")}\n" +
                   $"  {LocalizationManager.GetString("Menu_Stats")}\n" +
                   $"  {LocalizationManager.GetString("Menu_Skills")}\n\n" +
                   $"{LocalizationManager.GetString("Status_Health", "100", "100")}\n" +
                   $"{LocalizationManager.GetString("Status_Level", "5")}\n" +
                   $"{LocalizationManager.GetString("Status_Gold", "250")}";
        }

        private string GetPreviewDescription()
        {
            if (languageComboBox.SelectedItem is LanguageItem selectedItem)
            {
                return selectedItem.CultureName switch
                {
                    "zh-CN" => "所选语言的界面元素示例：",
                    "zh-TW" => "所選語言的介面元素範例：",
                    "ja-JP" => "選択した言語のインターフェース要素のサンプル：",
                    "ko-KR" => "선택한 언어의 인터페이스 요소 샘플:",
                    "es-ES" => "Elementos de interfaz de muestra en el idioma seleccionado:",
                    "fr-FR" => "Éléments d'interface d'exemple dans la langue sélectionnée :",
                    "de-DE" => "Beispiel-Benutzeroberflächenelemente in der ausgewählten Sprache:",
                    "ru-RU" => "Образцы элементов интерфейса на выбранном языке:",
                    _ => "Sample interface elements in selected language:"
                };
            }
            return "Sample interface elements in selected language:";
        }

        private void ApplyButton_Click(object sender, EventArgs e)
        {
            ApplyLanguageChange();
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            ApplyLanguageChange();
            DialogResult = DialogResult.OK;
            Close();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void ApplyLanguageChange()
        {
            if (languageComboBox.SelectedItem is LanguageItem selectedItem)
            {
                SelectedCulture = selectedItem.CultureName;
                LocalizationManager.SetCulture(selectedItem.CultureName);

                // Update dialog text immediately
                UpdateDialogText();

                // Show confirmation message
                var message = LocalizationManager.GetString("Language_Changed") ?? 
                             "Language changed successfully. Some changes may require restarting the application.";
                var title = LocalizationManager.GetString("Common_Information") ?? "Information";
                
                MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void UpdateDialogText()
        {
            this.Text = LocalizationManager.GetString("Menu_Language");
            instructionLabel.Text = GetInstructionText();
            previewGroupBox.Text = GetPreviewGroupText();
            okButton.Text = LocalizationManager.GetString("Common_OK");
            cancelButton.Text = LocalizationManager.GetString("Common_Cancel");
            applyButton.Text = GetApplyText();
            UpdatePreview();
        }

        private string GetInstructionText()
        {
            return LocalizationManager.GetCurrentCulture().Name switch
            {
                "zh-CN" => "选择您首选的语言：",
                "zh-TW" => "選擇您偏好的語言：",
                "ja-JP" => "お好みの言語を選択してください：",
                "ko-KR" => "원하는 언어를 선택하세요:",
                "es-ES" => "Seleccione su idioma preferido:",
                "fr-FR" => "Sélectionnez votre langue préférée :",
                "de-DE" => "Wählen Sie Ihre bevorzugte Sprache:",
                "ru-RU" => "Выберите предпочитаемый язык:",
                _ => "Select your preferred language:"
            };
        }

        private string GetPreviewGroupText()
        {
            return LocalizationManager.GetCurrentCulture().Name switch
            {
                "zh-CN" => "预览",
                "zh-TW" => "預覽",
                "ja-JP" => "プレビュー",
                "ko-KR" => "미리보기",
                "es-ES" => "Vista previa",
                "fr-FR" => "Aperçu",
                "de-DE" => "Vorschau",
                "ru-RU" => "Предварительный просмотр",
                _ => "Preview"
            };
        }

        private string GetApplyText()
        {
            return LocalizationManager.GetCurrentCulture().Name switch
            {
                "zh-CN" => "应用",
                "zh-TW" => "套用",
                "ja-JP" => "適用",
                "ko-KR" => "적용",
                "es-ES" => "Aplicar",
                "fr-FR" => "Appliquer",
                "de-DE" => "Anwenden",
                "ru-RU" => "Применить",
                _ => "Apply"
            };
        }

        private class LanguageItem
        {
            public string CultureName { get; set; }
            public string DisplayName { get; set; }

            public override string ToString()
            {
                return DisplayName;
            }
        }
    }
} 