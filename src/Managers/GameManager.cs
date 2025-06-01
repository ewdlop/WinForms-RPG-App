using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using WinFormsApp1.Events;
using WinFormsApp1.Interfaces;

namespace WinFormsApp1.Managers
{
    /// <summary>
    /// Core game management and coordination
    /// </summary>
    public class GameManager : BaseManager, IGameManager
    {
        private readonly IPlayerManager _playerManager;
        private GameState _currentGameState;
        private GameStatistics _gameStatistics;
        private Dictionary<string, bool> _featureFlags;
        private DateTime _gameStartTime;
        private DateTime _sessionStartTime;
        private readonly Dictionary<string, Func<string[], CommandResult>> _commandHandlers;
        private bool _hasUnsavedChanges;

        public override string ManagerName => "GameManager";
        public GameState CurrentGameState => _currentGameState;
        public bool IsGameRunning => _currentGameState == GameState.Running || _currentGameState == GameState.InCombat;
        public bool HasUnsavedChanges => _hasUnsavedChanges;

        public GameManager(IEventManager eventManager, IPlayerManager playerManager) : base(eventManager)
        {
            _playerManager = playerManager ?? throw new ArgumentNullException(nameof(playerManager));
            _currentGameState = GameState.NotStarted;
            _gameStatistics = new GameStatistics();
            _featureFlags = new Dictionary<string, bool>();
            _commandHandlers = new Dictionary<string, Func<string[], CommandResult>>();
            
            InitializeCommandHandlers();
            InitializeDefaultFeatures();
        }

        protected override void SubscribeToEvents()
        {
            // Subscribe to player events to update statistics
            EventManager.Subscribe<PlayerLeveledUpEvent>(OnPlayerLeveledUp);
            EventManager.Subscribe<PlayerDiedEvent>(OnPlayerDied);
            EventManager.Subscribe<PlayerExperienceGainedEvent>(OnPlayerExperienceGained);
            EventManager.Subscribe<PlayerGoldChangedEvent>(OnPlayerGoldChanged);
            EventManager.Subscribe<EnemyDefeatedEvent>(OnEnemyDefeated);
            EventManager.Subscribe<ItemAddedEvent>(OnItemAdded);
        }

        protected override void UnsubscribeFromEvents()
        {
            EventManager.ClearSubscriptions<PlayerLeveledUpEvent>();
            EventManager.ClearSubscriptions<PlayerDiedEvent>();
            EventManager.ClearSubscriptions<PlayerExperienceGainedEvent>();
            EventManager.ClearSubscriptions<PlayerGoldChangedEvent>();
            EventManager.ClearSubscriptions<EnemyDefeatedEvent>();
            EventManager.ClearSubscriptions<ItemAddedEvent>();
        }

        public void StartNewGame(Player player)
        {
            if (player == null)
                throw new ArgumentNullException(nameof(player));

            LogMessage($"Starting new game with player: {player.Name}");

            // Set the current player
            _playerManager.SetCurrentPlayer(player);

            // Reset game statistics
            _gameStatistics = new GameStatistics();
            _gameStartTime = DateTime.UtcNow;
            _sessionStartTime = DateTime.UtcNow;
            _hasUnsavedChanges = true; // New game has unsaved changes

            // Change game state
            ChangeGameState(GameState.Running, "New game started");

            // Publish game started event
            EventManager.Publish(new GameStartedEvent(player, true));

            LogMessage("New game started successfully");
        }

        public bool LoadGame(int saveSlot)
        {
            try
            {
                LogMessage($"Loading game from slot {saveSlot}");
                ChangeGameState(GameState.Loading, $"Loading from slot {saveSlot}");

                // TODO: Implement actual save/load logic
                // For now, simulate loading
                var success = SimulateLoadGame(saveSlot);

                if (success)
                {
                    _sessionStartTime = DateTime.UtcNow;
                    _hasUnsavedChanges = false; // Loaded game has no unsaved changes
                    ChangeGameState(GameState.Running, "Game loaded successfully");
                    EventManager.Publish(new GameLoadedEvent(saveSlot, _playerManager.CurrentPlayer, true));
                    _gameStatistics.LoadCount++;
                    LogMessage($"Game loaded successfully from slot {saveSlot}");
                }
                else
                {
                    ChangeGameState(GameState.NotStarted, "Failed to load game");
                    EventManager.Publish(new GameLoadedEvent(saveSlot, null, false, "Save file not found or corrupted"));
                    LogError($"Failed to load game from slot {saveSlot}");
                }

                return success;
            }
            catch (Exception ex)
            {
                LogError($"Error loading game from slot {saveSlot}", ex);
                ChangeGameState(GameState.NotStarted, "Load error");
                EventManager.Publish(new GameLoadedEvent(saveSlot, null, false, ex.Message));
                return false;
            }
        }

        public bool SaveGame(int saveSlot)
        {
            try
            {
                if (_playerManager.CurrentPlayer == null)
                {
                    LogError("Cannot save game: No current player");
                    return false;
                }

                LogMessage($"Saving game to slot {saveSlot}");
                var previousState = _currentGameState;
                ChangeGameState(GameState.Saving, $"Saving to slot {saveSlot}");

                // TODO: Implement actual save logic
                // For now, simulate saving
                var success = SimulateSaveGame(saveSlot);

                if (success)
                {
                    ChangeGameState(previousState, "Game saved successfully");
                    EventManager.Publish(new GameSavedEvent(saveSlot, _playerManager.CurrentPlayer, true, "", 1024));
                    _gameStatistics.SaveCount++;
                    _hasUnsavedChanges = false; // Clear unsaved changes flag
                    LogMessage($"Game saved successfully to slot {saveSlot}");
                }
                else
                {
                    ChangeGameState(previousState, "Failed to save game");
                    EventManager.Publish(new GameSavedEvent(saveSlot, _playerManager.CurrentPlayer, false, "Write error"));
                    LogError($"Failed to save game to slot {saveSlot}");
                }

                return success;
            }
            catch (Exception ex)
            {
                LogError($"Error saving game to slot {saveSlot}", ex);
                EventManager.Publish(new GameSavedEvent(saveSlot, _playerManager.CurrentPlayer, false, ex.Message));
                return false;
            }
        }

        public CommandResult ProcessCommand(string command)
        {
            if (string.IsNullOrWhiteSpace(command))
                return new CommandResult(false, "", "Command cannot be empty");

            var stopwatch = Stopwatch.StartNew();
            var parts = command.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var commandName = parts[0].ToLowerInvariant();
            var args = parts.Skip(1).ToArray();

            try
            {
                LogMessage($"Processing command: {command}");

                CommandResult result;
                if (_commandHandlers.ContainsKey(commandName))
                {
                    result = _commandHandlers[commandName](args);
                }
                else
                {
                    result = new CommandResult(false, "", $"Unknown command: {commandName}");
                }

                stopwatch.Stop();
                EventManager.Publish(new CommandProcessedEvent(command, result, _playerManager.CurrentPlayer, stopwatch.Elapsed.TotalMilliseconds));

                return result;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                LogError($"Error processing command: {command}", ex);
                var errorResult = new CommandResult(false, "", $"Command error: {ex.Message}");
                EventManager.Publish(new CommandProcessedEvent(command, errorResult, _playerManager.CurrentPlayer, stopwatch.Elapsed.TotalMilliseconds));
                return errorResult;
            }
        }

        public List<string> GetAvailableCommands()
        {
            var commands = new List<string>(_commandHandlers.Keys);
            
            // Add context-specific commands based on game state
            switch (_currentGameState)
            {
                case GameState.Running:
                    commands.AddRange(new[] { "look", "inventory", "stats", "map", "save", "quit" });
                    break;
                case GameState.InCombat:
                    commands.AddRange(new[] { "attack", "defend", "flee", "use" });
                    break;
                case GameState.InMenu:
                    commands.AddRange(new[] { "continue", "load", "settings", "quit" });
                    break;
            }

            return commands.Distinct().OrderBy(c => c).ToList();
        }

        public void PauseGame()
        {
            if (_currentGameState == GameState.Running)
            {
                ChangeGameState(GameState.Paused, "Game paused by user");
                LogMessage("Game paused");
            }
        }

        public void ResumeGame()
        {
            if (_currentGameState == GameState.Paused)
            {
                ChangeGameState(GameState.Running, "Game resumed by user");
                LogMessage("Game resumed");
            }
        }

        public void EndGame()
        {
            if (_currentGameState != GameState.NotStarted && _currentGameState != GameState.GameOver)
            {
                var finalStats = GetGameStatistics();
                ChangeGameState(GameState.GameOver, "Game ended by user");
                EventManager.Publish(new GameEndedEvent(_playerManager.CurrentPlayer, "User quit", finalStats));
                LogMessage("Game ended");
            }
        }

        public bool ProcessCheat(string cheatCode)
        {
            if (string.IsNullOrWhiteSpace(cheatCode))
                return false;

            var parts = cheatCode.ToLowerInvariant().Split(' ');
            var cheat = parts[0];
            var parameters = new Dictionary<string, object>();

            try
            {
                bool success = false;
                string effect = "";

                switch (cheat)
                {
                    case "godmode":
                        success = true;
                        effect = "God mode activated";
                        SetFeatureEnabled("GodMode", true);
                        break;

                    case "addgold":
                        if (parts.Length > 1 && int.TryParse(parts[1], out int gold))
                        {
                            _playerManager.ModifyGold(gold, "Cheat code");
                            success = true;
                            effect = $"Added {gold} gold";
                            parameters["amount"] = gold;
                        }
                        break;

                    case "addexp":
                        if (parts.Length > 1 && int.TryParse(parts[1], out int exp))
                        {
                            _playerManager.AddExperience(exp, "Cheat code");
                            success = true;
                            effect = $"Added {exp} experience";
                            parameters["amount"] = exp;
                        }
                        break;

                    case "heal":
                        _playerManager.Heal(_playerManager.CurrentPlayer.MaxHealth, "Cheat code");
                        success = true;
                        effect = "Fully healed";
                        break;

                    case "levelup":
                        if (_playerManager.CanLevelUp())
                        {
                            _playerManager.LevelUp();
                            success = true;
                            effect = "Leveled up";
                        }
                        break;
                }

                EventManager.Publish(new CheatUsedEvent(cheatCode, _playerManager.CurrentPlayer, success, effect, parameters));
                
                if (success)
                {
                    LogMessage($"Cheat activated: {cheatCode} - {effect}");
                }

                return success;
            }
            catch (Exception ex)
            {
                LogError($"Error processing cheat: {cheatCode}", ex);
                EventManager.Publish(new CheatUsedEvent(cheatCode, _playerManager.CurrentPlayer, false, $"Error: {ex.Message}"));
                return false;
            }
        }

        public GameStatistics GetGameStatistics()
        {
            // Update total play time
            if (_gameStartTime != default)
            {
                _gameStatistics.TotalPlayTime = (DateTime.UtcNow - _gameStartTime).TotalMinutes;
            }

            return _gameStatistics;
        }

        public void UpdateGameTime(double deltaTime)
        {
            var totalTime = GetGameStatistics().TotalPlayTime;
            var sessionTime = _sessionStartTime != default ? (DateTime.UtcNow - _sessionStartTime).TotalMinutes : 0;

            EventManager.Publish(new GameTimeUpdatedEvent(totalTime, deltaTime, sessionTime));
        }

        public bool IsFeatureEnabled(string featureName)
        {
            return _featureFlags.ContainsKey(featureName) && _featureFlags[featureName];
        }

        public void SetFeatureEnabled(string featureName, bool enabled)
        {
            var previousState = IsFeatureEnabled(featureName);
            _featureFlags[featureName] = enabled;

            if (previousState != enabled)
            {
                EventManager.Publish(new FeatureToggledEvent(featureName, enabled, previousState, "GameManager"));
                LogMessage($"Feature '{featureName}' {(enabled ? "enabled" : "disabled")}");
            }
        }

        private void ChangeGameState(GameState newState, string reason = "")
        {
            var oldState = _currentGameState;
            _currentGameState = newState;

            if (oldState != newState)
            {
                EventManager.Publish(new GameStateChangedEvent(oldState, newState, reason));
                LogMessage($"Game state changed: {oldState} -> {newState} ({reason})");
            }
        }

        private void InitializeCommandHandlers()
        {
            // Basic commands
            _commandHandlers["help"] = args => new CommandResult(true, "Available commands: " + string.Join(", ", GetAvailableCommands()));
            _commandHandlers["quit"] = args => { EndGame(); return new CommandResult(true, "Game ended"); };
            
            // Save/Load commands
            _commandHandlers["save"] = args => 
            {
                var slot = args.Length > 0 && int.TryParse(args[0], out int s) ? s : 1;
                var success = SaveGame(slot);
                return new CommandResult(success, success ? $"Game saved to slot {slot}" : "Failed to save game");
            };
            _commandHandlers["load"] = args =>
            {
                var slot = args.Length > 0 && int.TryParse(args[0], out int s) ? s : 1;
                var success = LoadGame(slot);
                return new CommandResult(success, success ? $"Game loaded from slot {slot}" : "Failed to load game");
            };

            // Player-related commands that delegate to PlayerManager
            _commandHandlers["stats"] = args =>
            {
                var player = _playerManager.CurrentPlayer;
                if (player == null)
                    return new CommandResult(false, "", "No character loaded");

                var stats = $"=== Character Stats ===\n" +
                           $"Name: {player.Name}\n" +
                           $"Class: {player.CharacterClass}\n" +
                           $"Level: {player.Level}\n" +
                           $"Health: {player.Health}/{player.MaxHealth}\n" +
                           $"Attack: {player.Attack}\n" +
                           $"Defense: {player.Defense}\n" +
                           $"Experience: {player.Experience}/{player.ExperienceToNextLevel}\n" +
                           $"Gold: {player.Gold}\n" +
                           $"Skill Points: {player.SkillPoints}";
                
                return new CommandResult(true, stats);
            };

            _commandHandlers["heal"] = args =>
            {
                var player = _playerManager.CurrentPlayer;
                if (player == null)
                    return new CommandResult(false, "", "No character loaded");

                if (args.Length > 0 && int.TryParse(args[0], out int amount))
                {
                    _playerManager.Heal(amount, "Command");
                    return new CommandResult(true, $"Healed for {amount} health");
                }
                else
                {
                    _playerManager.Heal(player.MaxHealth, "Command");
                    return new CommandResult(true, "Fully healed");
                }
            };

            // Game state commands
            _commandHandlers["pause"] = args =>
            {
                PauseGame();
                return new CommandResult(true, "Game paused");
            };

            _commandHandlers["resume"] = args =>
            {
                ResumeGame();
                return new CommandResult(true, "Game resumed");
            };

            _commandHandlers["status"] = args =>
            {
                var status = $"Game State: {_currentGameState}\n" +
                           $"Running: {IsGameRunning}\n" +
                           $"Unsaved Changes: {HasUnsavedChanges}\n" +
                           $"Session Time: {(DateTime.UtcNow - _sessionStartTime).TotalMinutes:F1} minutes";
                return new CommandResult(true, status);
            };

            // Feature toggle commands
            _commandHandlers["features"] = args =>
            {
                if (args.Length == 0)
                {
                    var features = string.Join("\n", _featureFlags.Select(f => $"{f.Key}: {(f.Value ? "Enabled" : "Disabled")}"));
                    return new CommandResult(true, $"=== Features ===\n{features}");
                }
                else if (args.Length == 2)
                {
                    var featureName = args[0];
                    var enabled = args[1].ToLowerInvariant() == "on" || args[1].ToLowerInvariant() == "true";
                    SetFeatureEnabled(featureName, enabled);
                    return new CommandResult(true, $"Feature '{featureName}' {(enabled ? "enabled" : "disabled")}");
                }
                return new CommandResult(false, "", "Usage: features [feature_name on/off]");
            };

            // Statistics command
            _commandHandlers["statistics"] = args =>
            {
                var stats = GetGameStatistics();
                var statsText = $"=== Game Statistics ===\n" +
                              $"Total Play Time: {stats.TotalPlayTime:F1} minutes\n" +
                              $"Enemies Defeated: {stats.EnemiesDefeated}\n" +
                              $"Items Collected: {stats.ItemsCollected}\n" +
                              $"Locations Visited: {stats.LocationsVisited}\n" +
                              $"Times Leveled Up: {stats.TimesLeveledUp}\n" +
                              $"Gold Earned: {stats.GoldEarned}\n" +
                              $"Experience Gained: {stats.ExperienceGained}\n" +
                              $"Death Count: {stats.DeathCount}\n" +
                              $"Save Count: {stats.SaveCount}\n" +
                              $"Load Count: {stats.LoadCount}";
                return new CommandResult(true, statsText);
            };

            // Cheat commands (if enabled)
            _commandHandlers["cheat"] = args =>
            {
                if (!IsFeatureEnabled("CheatCodes"))
                    return new CommandResult(false, "", "Cheat codes are disabled");

                if (args.Length == 0)
                    return new CommandResult(false, "", "Usage: cheat <cheat_code> [parameters]");

                var cheatCode = string.Join(" ", args);
                var success = ProcessCheat(cheatCode);
                return new CommandResult(success, success ? "Cheat activated" : "Invalid cheat code");
            };

            // Clear command
            _commandHandlers["clear"] = args =>
            {
                EventManager.Publish(new GameMessageEvent("Screen cleared", GameMessageType.System));
                return new CommandResult(true, "");
            };
        }

        private void InitializeDefaultFeatures()
        {
            _featureFlags["GodMode"] = false;
            _featureFlags["DebugMode"] = false;
            _featureFlags["CheatCodes"] = true;
            _featureFlags["AutoSave"] = true;
        }

        private bool SimulateLoadGame(int saveSlot)
        {
            // Simulate loading - in real implementation, this would load from file
            return saveSlot >= 1 && saveSlot <= 10;
        }

        private bool SimulateSaveGame(int saveSlot)
        {
            // Simulate saving - in real implementation, this would save to file
            return saveSlot >= 1 && saveSlot <= 10;
        }

        // Event handlers for statistics tracking
        private void OnPlayerLeveledUp(PlayerLeveledUpEvent e)
        {
            _gameStatistics.TimesLeveledUp++;
            MarkUnsavedChanges();
        }

        private void OnPlayerDied(PlayerDiedEvent e)
        {
            _gameStatistics.DeathCount++;
            MarkUnsavedChanges();
        }

        private void OnPlayerExperienceGained(PlayerExperienceGainedEvent e)
        {
            _gameStatistics.ExperienceGained += e.ExperienceGained;
            MarkUnsavedChanges();
        }

        private void OnPlayerGoldChanged(PlayerGoldChangedEvent e)
        {
            if (e.NewGold > e.OldGold)
            {
                _gameStatistics.GoldEarned += (e.NewGold - e.OldGold);
            }
            MarkUnsavedChanges();
        }

        private void OnEnemyDefeated(EnemyDefeatedEvent e)
        {
            _gameStatistics.EnemiesDefeated++;
            MarkUnsavedChanges();
        }

        private void OnItemAdded(ItemAddedEvent e)
        {
            _gameStatistics.ItemsCollected++;
            MarkUnsavedChanges();
        }

        /// <summary>
        /// Mark that the game has unsaved changes
        /// </summary>
        private void MarkUnsavedChanges()
        {
            if (_currentGameState == GameState.Running || _currentGameState == GameState.InCombat)
            {
                _hasUnsavedChanges = true;
            }
        }
    }
} 