using System;
using System.Globalization;
using System.Windows.Forms;
using WinFormsApp1.Localization;
using WinFormsApp1.Dialogs;
using WinFormsApp1.Configuration;

namespace WinFormsApp1
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Initialize application
            ApplicationConfiguration.Initialize();

            // Initialize localization
            InitializeLocalization();

            // Show language selection dialog on first run or if requested
            if (ShouldShowLanguageDialog())
            {
                using (var languageDialog = new LanguageSelectionDialog())
                {
                    if (languageDialog.ShowDialog() == DialogResult.OK)
                    {
                        SaveLanguagePreference(languageDialog.SelectedCulture);
                    }
                }
            }

            // Set application title based on current culture
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Create and run the main form
            var mainForm = new Form1();
            mainForm.Text = LocalizationManager.GetString("AppTitle");
            
            Application.Run(mainForm);
        }

        private static void InitializeLocalization()
        {
            try
            {
                // Load saved language preference or use system default
                string savedLanguage = LoadLanguagePreference();
                
                if (!string.IsNullOrEmpty(savedLanguage))
                {
                    LocalizationManager.SetCulture(savedLanguage);
                }
                else
                {
                    // Set Chinese as default for demonstration
                    LocalizationManager.SetCulture("zh-CN");
                }
            }
            catch (Exception ex)
            {
                // Fallback to English if there's any issue
                LocalizationManager.SetCulture("en-US");
                MessageBox.Show($"Language initialization failed: {ex.Message}\nDefaulting to English.", 
                               "Language Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private static bool ShouldShowLanguageDialog()
        {
            // Show language dialog if no preference is saved
            // or if the user explicitly requested it
            string savedLanguage = LoadLanguagePreference();
            return string.IsNullOrEmpty(savedLanguage);
        }

        private static string LoadLanguagePreference()
        {
            try
            {
                // Use our custom configuration manager
                return ConfigurationManager.Language;
            }
            catch
            {
                return "";
            }
        }

        private static void SaveLanguagePreference(string cultureName)
        {
            try
            {
                ConfigurationManager.Language = cultureName;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to save language preference: {ex.Message}", 
                               "Settings Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}