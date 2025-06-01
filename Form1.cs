using System;
using System.Drawing;
using System.Windows.Forms;
using WinFormsApp1.Events;

namespace WinFormsApp1
{
    public partial class Form1 : Form, IMessageEventHandler, IPlayerEventHandler, ICombatEventHandler, IGameStateEventHandler
    {
        private GameEventManager eventManager;
        private List<string> commandHistory = new List<string>();
        private int historyIndex = -1;
        private bool isInCombatMode = false;

        public Form1()
        {
            InitializeComponent();
            eventManager = GameEventManager.Instance;
            gameEngine = new GameEngine(this);
            
            // Subscribe to events
            eventManager.SubscribeToMessageEvents(this);
            eventManager.SubscribeToPlayerEvents(this);
            eventManager.SubscribeToCombatEvents(this);
            eventManager.SubscribeToGameStateEvents(this);
            
            SetupUI();
        }

        private void SetupUI()
        {
            // Initial setup
            EnableGameControls(false);
            DisplayText("Welcome to Realm of Aethermoor!", Color.Cyan);
            DisplayText("Click 'New Game' to begin your adventure.", Color.Yellow);
        }

        // IMessageEventHandler implementation
        public void OnMessageDisplayed(object? sender, MessageEventArgs e)
        {
            // Handle message display with proper color and formatting
            Color displayColor = e.Color ?? GetDefaultColorForMessageType(e.Type);
            DisplayText(e.Message, displayColor);
        }

        private Color GetDefaultColorForMessageType(MessageType type)
        {
            return type switch
            {
                MessageType.System => Color.Cyan,
                MessageType.Combat => Color.Red,
                MessageType.Error => Color.Red,
                MessageType.Success => Color.Green,
                MessageType.Warning => Color.Orange,
                MessageType.Cheat => Color.Magenta,
                MessageType.Debug => Color.Gray,
                _ => Color.White
            };
        }

        // IPlayerEventHandler implementation
        public void OnPlayerLeveledUp(object? sender, LevelUpEventArgs e)
        {
            // Update UI for level up
            UpdateCharacterDisplay();
            ShowLevelUpNotification(e.OldLevel, e.NewLevel);
        }

        public void OnPlayerGainedExperience(object? sender, ExperienceGainEventArgs e)
        {
            // Update character display and show experience gain
            UpdateCharacterDisplay();
            if (!e.LeveledUp)
            {
                ShowExperienceGain(e.ExperienceGained);
            }
        }

        public void OnPlayerGoldChanged(object? sender, GoldChangeEventArgs e)
        {
            // Update character display and show gold change animation
            UpdateCharacterDisplay();
            ShowGoldChange(e.GoldChange);
        }

        public void OnPlayerHealthChanged(object? sender, HealthChangeEventArgs e)
        {
            // Update character display and health bar
            UpdateCharacterDisplay();
            UpdateHealthBar(e.CurrentHealth, e.MaxHealth);
        }

        public void OnPlayerLearnedSkill(object? sender, SkillEventArgs e)
        {
            // Update character display
            UpdateCharacterDisplay();
        }

        // ICombatEventHandler implementation
        public void OnCombatStarted(object? sender, CombatEventArgs e)
        {
            SetCombatMode(true);
        }

        public void OnCombatActionPerformed(object? sender, CombatActionEventArgs e)
        {
            // Could add combat animations or effects here
            if (e.IsCritical)
            {
                // Flash screen or show special effect for critical hits
                FlashCriticalHit();
            }
        }

        public void OnCombatEnded(object? sender, CombatEndEventArgs e)
        {
            SetCombatMode(false);
            
            if (e.PlayerWon)
            {
                // Show victory effects
                ShowVictoryEffect();
            }
            else
            {
                // Show defeat effects
                ShowDefeatEffect();
            }
        }

        // IGameStateEventHandler implementation
        public void OnGameStateChanged(object? sender, GameStateEventArgs e)
        {
            // Handle game state changes
            switch (e.StateName)
            {
                case "GameStarted":
                    EnableGameControls(true);
                    break;
                case "GameReset":
                    // Handle game reset
                    break;
            }
        }

        public void OnGameSaved(object? sender, SaveGameEventArgs e)
        {
            if (e.WasSuccessful)
            {
                // Show save success indicator
                ShowSaveIndicator(true);
            }
            else
            {
                // Show save failure indicator
                ShowSaveIndicator(false);
            }
        }

        public void OnGameLoaded(object? sender, LoadGameEventArgs e)
        {
            if (e.WasSuccessful && e.LoadedPlayer != null)
            {
                EnableGameControls(true);
                UpdateCharacterDisplay();
            }
        }

        public void OnCommandExecuted(object? sender, CommandEventArgs e)
        {
            // Add command to history
            AddToCommandHistory(e.Command + (e.Arguments.Length > 0 ? " " + string.Join(" ", e.Arguments) : ""));
        }

        public void OnCheatActivated(object? sender, CheatEventArgs e)
        {
            // Show cheat activation effect
            ShowCheatEffect(e.CheatCommand);
        }

        // UI Helper Methods
        private void ShowLevelUpNotification(int oldLevel, int newLevel)
        {
            // Create a temporary label for level up notification
            var levelUpLabel = new Label
            {
                Text = $"LEVEL UP! {oldLevel} → {newLevel}",
                Font = new Font("Arial", 16, FontStyle.Bold),
                ForeColor = Color.Gold,
                BackColor = Color.Transparent,
                AutoSize = true,
                Location = new Point(this.Width / 2 - 100, this.Height / 2 - 50)
            };

            this.Controls.Add(levelUpLabel);
            levelUpLabel.BringToFront();

            // Animate and remove after 3 seconds
            var timer = new System.Windows.Forms.Timer { Interval = 3000 };
            timer.Tick += (s, e) =>
            {
                this.Controls.Remove(levelUpLabel);
                timer.Stop();
                timer.Dispose();
            };
            timer.Start();
        }

        private void ShowExperienceGain(int experience)
        {
            // Update status bar or show floating text
            UpdateStatus($"Gained {experience} XP!");
        }

        private void ShowGoldChange(int goldChange)
        {
            // Update status bar or show floating text
            string message = goldChange > 0 ? $"+{goldChange} Gold!" : $"{goldChange} Gold";
            UpdateStatus(message);
        }

        private void UpdateHealthBar(int currentHealth, int maxHealth)
        {
            // Update health bar if you have one
            // This could be a progress bar or custom drawn health bar
        }

        private void FlashCriticalHit()
        {
            // Flash the screen red for critical hits
            var originalColor = this.BackColor;
            this.BackColor = Color.DarkRed;
            
            var timer = new System.Windows.Forms.Timer { Interval = 100 };
            timer.Tick += (s, e) =>
            {
                this.BackColor = originalColor;
                timer.Stop();
                timer.Dispose();
            };
            timer.Start();
        }

        private void ShowVictoryEffect()
        {
            // Show victory animation or effect
            DisplayText("🎉 VICTORY! 🎉", Color.Gold);
        }

        private void ShowDefeatEffect()
        {
            // Show defeat animation or effect
            DisplayText("💀 DEFEAT 💀", Color.Red);
        }

        private void ShowSaveIndicator(bool success)
        {
            string message = success ? "✓ Game Saved" : "✗ Save Failed";
            Color color = success ? Color.Green : Color.Red;
            
            // Show in status bar temporarily
            var originalStatus = toolStripStatusLabel?.Text ?? "";
            UpdateStatus(message);
            
            var timer = new System.Windows.Forms.Timer { Interval = 2000 };
            timer.Tick += (s, e) =>
            {
                UpdateStatus(originalStatus);
                timer.Stop();
                timer.Dispose();
            };
            timer.Start();
        }

        private void ShowCheatEffect(string cheatCommand)
        {
            // Show cheat activation effect
            DisplayText($"🎮 CHEAT: {cheatCommand.ToUpper()} 🎮", Color.Magenta);
        }

        private void AddToCommandHistory(string command)
        {
            commandHistory.Add(command);
            if (commandHistory.Count > 50) // Limit history size
            {
                commandHistory.RemoveAt(0);
            }
            historyIndex = commandHistory.Count;
        }

        // Core UI Methods
        public void DisplayText(string text, Color? color = null)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string, Color?>((t, c) => DisplayText(t, c)), text, color);
                return;
            }

            if (gameTextBox != null)
            {
                gameTextBox.SelectionStart = gameTextBox.TextLength;
                gameTextBox.SelectionLength = 0;
                gameTextBox.SelectionColor = color ?? Color.White;
                gameTextBox.AppendText(text + Environment.NewLine);
                gameTextBox.ScrollToCaret();
            }
        }

        public void SetCombatMode(bool inCombat)
        {
            isInCombatMode = inCombat;
            
            if (InvokeRequired)
            {
                Invoke(new Action<bool>(SetCombatMode), inCombat);
                return;
            }

            // Update UI to reflect combat state
            if (toolStripStatusLabel != null)
            {
                toolStripStatusLabel.BackColor = inCombat ? Color.DarkRed : SystemColors.Control;
                toolStripStatusLabel.ForeColor = inCombat ? Color.White : SystemColors.ControlText;
            }
        }

        public void EnableGameControls(bool enabled)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<bool>(EnableGameControls), enabled);
                return;
            }

            // Enable/disable game-related controls
            if (commandTextBox != null)
                commandTextBox.Enabled = enabled;
        }

        public void UpdateStatus(string status)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string>(UpdateStatus), status);
                return;
            }

            if (toolStripStatusLabel != null)
            {
                toolStripStatusLabel.Text = status;
            }
        }

        private void UpdateCharacterDisplay()
        {
            var player = gameEngine?.GetPlayer();
            if (player != null)
            {
                string statusText = $"Health: {player.Health}/{player.MaxHealth} | Level: {player.Level} | Gold: {player.Gold}";
                UpdateStatus(statusText);
            }
        }

        // Event handlers for UI controls
        private void newGameButton_Click(object sender, EventArgs e)
        {
            gameEngine?.StartNewGame();
        }

        private void loadGameButton_Click(object sender, EventArgs e)
        {
            gameEngine?.LoadGame();
        }

        private void saveGameButton_Click(object sender, EventArgs e)
        {
            gameEngine?.SaveGame();
        }

        private void commandTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string command = commandTextBox.Text.Trim();
                if (!string.IsNullOrEmpty(command))
                {
                    // Echo the command
                    DisplayText($"> {command}", Color.LightGray);
                    
                    // Process the command
                    gameEngine?.ProcessCommand(command);
                    
                    // Clear the input
                    commandTextBox.Clear();
                }
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Up)
            {
                // Navigate command history up
                if (historyIndex > 0)
                {
                    historyIndex--;
                    commandTextBox.Text = commandHistory[historyIndex];
                    commandTextBox.SelectionStart = commandTextBox.Text.Length;
                }
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Down)
            {
                // Navigate command history down
                if (historyIndex < commandHistory.Count - 1)
                {
                    historyIndex++;
                    commandTextBox.Text = commandHistory[historyIndex];
                    commandTextBox.SelectionStart = commandTextBox.Text.Length;
                }
                else if (historyIndex == commandHistory.Count - 1)
                {
                    historyIndex = commandHistory.Count;
                    commandTextBox.Clear();
                }
                e.Handled = true;
            }
        }

        private void inventoryButton_Click(object sender, EventArgs e)
        {
            gameEngine?.ShowInventory();
        }

        private void statsButton_Click(object sender, EventArgs e)
        {
            gameEngine?.ShowCharacterStats();
        }

        private void skillsButton_Click(object sender, EventArgs e)
        {
            gameEngine?.ShowSkillTree();
        }

        private void helpButton_Click(object sender, EventArgs e)
        {
            gameEngine?.ShowHelp();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Unsubscribe from events to prevent memory leaks
            eventManager.UnsubscribeFromMessageEvents(this);
            eventManager.UnsubscribeFromPlayerEvents(this);
            eventManager.UnsubscribeFromCombatEvents(this);
            eventManager.UnsubscribeFromGameStateEvents(this);
            
            base.OnFormClosing(e);
        }
    }
} 