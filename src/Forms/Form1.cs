using System;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using System.Collections.Generic;
using WinFormsApp1.Controls;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {

        private void ShowEquipment(object sender, EventArgs e)
        {
            if (gameEngine != null)
            {
                try
                {
                    var player = gameEngine.GetPlayer();
                    string equipment = "=== Equipment ===\n";
                    equipment += $"Weapon: {(player.EquippedWeapon?.Name ?? "None")}\n";
                    equipment += $"Armor: {(player.EquippedArmor?.Name ?? "None")}\n\n";
                    equipment += $"Total Attack: {player.GetTotalAttack()}\n";
                    equipment += $"Total Defense: {player.GetTotalDefense()}";
                    
                    MessageBox.Show(equipment, "Equipment", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch
                {
                    DisplayText("Start a new game first!", Color.Red);
                }
            }
        }

        private void ShowThemeSelection(object sender, EventArgs e)
        {
            using (var themeDialog = new ThemeSelectionDialog())
            {
                if (themeDialog.ShowDialog() == DialogResult.OK)
                {
                    ApplyTheme(themeDialog.SelectedTheme);
                }
            }
        }

        private void ShowFontSettings(object sender, EventArgs e)
        {
            using (FontDialog fontDialog = new FontDialog())
            {
                fontDialog.Font = gameTextDisplayControl.GetFont();
                if (fontDialog.ShowDialog() == DialogResult.OK)
                {
                    gameTextDisplayControl.SetFont(fontDialog.Font);
                }
            }
        }

        private void ShowGameSettings(object sender, EventArgs e)
        {
            var currentSettings = new GameSettings(); // In a real app, load from config
            using (var settingsForm = new SettingsForm(currentSettings))
            {
                if (settingsForm.ShowDialog() == DialogResult.OK)
                {
                    // Apply the new settings
                    var newSettings = settingsForm.Settings;
                    
                    // Apply theme if changed
                    if (newSettings.Theme != currentSettings.Theme)
                    {
                        ApplyTheme(newSettings.Theme);
                    }
                    
                    // Apply font size if changed
                    if (newSettings.FontSize != currentSettings.FontSize)
                    {
                        var newFont = new Font(gameTextDisplayControl.GetFont().FontFamily, newSettings.FontSize);
                        gameTextDisplayControl.SetFont(newFont);
                    }
                    
                    DisplayText("Settings applied successfully!", Color.Green);
                }
            }
        }

        private void ShowControlsHelp(object sender, EventArgs e)
        {
            string controls = "=== Keyboard Controls ===\n\n";
            controls += "Tab - Open inventory\n";
            controls += "M - Open world map\n";
            controls += "Ctrl+S - Quick save\n";
            controls += "Ctrl+L - Quick load\n";
            controls += "F1 - Show help\n";
            controls += "Enter - Submit command\n";
            controls += "↑↓ - Command history\n\n";
            controls += "=== Mouse Controls ===\n\n";
            controls += "Click buttons for quick actions\n";
            controls += "Right-click for context menus\n";
            controls += "Double-click items to use them";

            MessageBox.Show(controls, "Controls Help", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ShowAbout(object sender, EventArgs e)
        {
            MessageBox.Show("Realm of Aethermoor\nVersion 2.0\n\n" +
                "A classic text-based RPG adventure built with .NET WinForms\n\n" +
                "Features:\n" +
                "• Rich character creation system\n" +
                "• Turn-based combat\n" +
                "• Inventory management\n" +
                "• World exploration\n" +
                "• Save/Load functionality\n" +
                "• Multiple themes\n\n" +
                "Created with ❤️ for RPG enthusiasts",
                "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ShowAssetEditor(object sender, EventArgs e)
        {
            try
            {
                AssetEditorForm.ShowAssetEditor();
                DisplayText("Asset Editor opened. You can now edit game data files.", Color.Cyan);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening Asset Editor:\n{ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ShowDataValidation(object sender, EventArgs e)
        {
            MessageBox.Show("Data Validation Tool\n\n" +
                "This feature validates the integrity of your game data files:\n\n" +
                "• Checks JSON syntax\n" +
                "• Validates data relationships\n" +
                "• Reports missing or invalid entries\n" +
                "• Suggests fixes for common issues\n\n" +
                "Feature coming soon in future update!",
                "Data Validation", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // Update the UpdateStatus method to work with new UI elements
        public void UpdateStatus(string status)
        {
            gameStatusBarControl?.UpdateStatus(status);
            
            // Update progress bars and stats if we have access to player data
            try
            {
                var player = gameEngine.GetPlayer();
                if (player != null)
                {
                    // Update status bar with player stats
                    gameStatusBarControl?.UpdatePlayerStats(player);

                    // Update progress display using custom control
                    progressDisplayControl?.UpdateProgress(player);

                    // Update character stats using custom control
                    characterStatsControl?.UpdateStats(player);

                    // Update mini-map with current location
                    string currentLocationKey = gameEngine.GetCurrentLocationKey();
                    if (!string.IsNullOrEmpty(currentLocationKey))
                    {
                        miniMapControl?.UpdateCurrentLocation(currentLocationKey);
                        
                        // Update status bar location
                        var locations = gameEngine.GetLocations();
                        if (locations.ContainsKey(currentLocationKey))
                        {
                            gameStatusBarControl?.UpdateLocation(locations[currentLocationKey].Name);
                        }
                    }
                }
                else
                {
                    // Clear stats when no player is available
                    gameStatusBarControl?.ClearPlayerStats();
                    progressDisplayControl?.ClearProgress();
                    characterStatsControl?.ClearStats();
                    miniMapControl?.ClearVisitedLocations();
                }
            }
            catch
            {
                // Player not available yet - clear the displays
                gameStatusBarControl?.ClearPlayerStats();
                progressDisplayControl?.ClearProgress();
                characterStatsControl?.ClearStats();
                miniMapControl?.ClearVisitedLocations();
            }
        }

        public void ClearScreen()
        {
            gameTextDisplayControl?.ClearText();
        }

        public void DisplayText(string text, Color? color = null)
        {
            gameTextDisplayControl?.DisplayText(text, color);
        }

        private void ApplyTheme(string themeName)
        {
            // Apply theme to custom controls
            gameTextDisplayControl?.ApplyTheme(themeName);
            gameInputControl?.ApplyTheme(themeName);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (gameEngine?.HasUnsavedChanges == true)
            {
                var result = MessageBox.Show(
                    "You have unsaved progress. Do you want to save before exiting?",
                    "Unsaved Changes",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    gameEngine.SaveGame();
                }
                else if (result == DialogResult.Cancel)
                {
                    e.Cancel = true;
                    return;
                }
            }
            base.OnFormClosing(e);
        }

        public void EnableGameControls(bool enabled)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => EnableGameControls(enabled)));
                return;
            }
            SetGameControlsEnabled(enabled);
        }

        private void QuickActionsControl_ActionClicked(object sender, string action)
        {
            switch (action)
            {
                case "look":
                    gameEngine.ProcessCommand("look");
                    break;
                case "stats":
                    gameEngine.ShowCharacterStats();
                    break;
                case "save":
                    gameEngine.SaveGame();
                    break;
                case "load":
                    gameEngine.LoadGame();
                    break;
                case "help":
                    gameEngine.ShowHelp();
                    break;
                case "exit":
                    this.Close();
                    break;
            }
        }

        public void SetCombatMode(bool inCombat)
        {
            gameStatusBarControl?.SetCombatMode(inCombat);
        }

        public void ShowExperienceGain(int expGained)
        {
            gameStatusBarControl?.ShowExperienceGain(expGained);
        }

        public void ShowLevelUp(int oldLevel, int newLevel)
        {
            gameStatusBarControl?.ShowExperienceGain(0, newLevel);
        }

        public void ShowGoldChange(int goldChange)
        {
            gameStatusBarControl?.ShowGoldChange(goldChange);
        }
    }
}
