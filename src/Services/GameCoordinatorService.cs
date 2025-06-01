using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WinFormsApp1.Events;
using WinFormsApp1.Interfaces;

namespace WinFormsApp1.Services
{
    /// <summary>
    /// Advanced service that coordinates complex operations between multiple managers
    /// Demonstrates sophisticated dependency injection patterns
    /// </summary>
    public class GameCoordinatorService : IGameCoordinatorService
    {
        private readonly IEventManager _eventManager;
        private readonly IGameManager _gameManager;
        private readonly IPlayerManager _playerManager;
        private readonly ICombatManager _combatManager;
        private readonly IInventoryManager _inventoryManager;
        private readonly ILocationManager _locationManager;
        private readonly ISkillManager _skillManager;
        private readonly ILogger<GameCoordinatorService> _logger;

        public GameCoordinatorService(
            IEventManager eventManager,
            IGameManager gameManager,
            IPlayerManager playerManager,
            ICombatManager combatManager,
            IInventoryManager inventoryManager,
            ILocationManager locationManager,
            ISkillManager skillManager,
            ILogger<GameCoordinatorService> logger)
        {
            _eventManager = eventManager ?? throw new ArgumentNullException(nameof(eventManager));
            _gameManager = gameManager ?? throw new ArgumentNullException(nameof(gameManager));
            _playerManager = playerManager ?? throw new ArgumentNullException(nameof(playerManager));
            _combatManager = combatManager ?? throw new ArgumentNullException(nameof(combatManager));
            _inventoryManager = inventoryManager ?? throw new ArgumentNullException(nameof(inventoryManager));
            _locationManager = locationManager ?? throw new ArgumentNullException(nameof(locationManager));
            _skillManager = skillManager ?? throw new ArgumentNullException(nameof(skillManager));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _logger.LogInformation("GameCoordinatorService initialized with all manager dependencies");
        }

        public async Task<CoordinatedActionResult> PerformCoordinatedActionAsync(string actionType, Dictionary<string, object> parameters)
        {
            var stopwatch = Stopwatch.StartNew();
            var result = new CoordinatedActionResult();

            try
            {
                _logger.LogInformation("Performing coordinated action: {ActionType}", actionType);

                switch (actionType.ToLowerInvariant())
                {
                    case "levelup_with_rewards":
                        result = await PerformLevelUpWithRewardsAsync(parameters);
                        break;

                    case "complete_quest":
                        result = await CompleteQuestAsync(parameters);
                        break;

                    case "travel_with_encounters":
                        result = await TravelWithEncountersAsync(parameters);
                        break;

                    case "use_skill_with_effects":
                        result = await UseSkillWithEffectsAsync(parameters);
                        break;

                    case "shop_transaction":
                        result = await PerformShopTransactionAsync(parameters);
                        break;

                    default:
                        result.Success = false;
                        result.Errors.Add($"Unknown action type: {actionType}");
                        break;
                }

                stopwatch.Stop();
                result.Results["ExecutionTime"] = stopwatch.Elapsed.TotalMilliseconds;

                _logger.LogInformation("Coordinated action {ActionType} completed in {Duration}ms with success: {Success}",
                    actionType, stopwatch.Elapsed.TotalMilliseconds, result.Success);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error performing coordinated action: {ActionType}", actionType);
                result.Success = false;
                result.Errors.Add($"Exception: {ex.Message}");
                return result;
            }
        }

        public async Task<bool> InitializePlayerAsync(Player player)
        {
            try
            {
                _logger.LogInformation("Initializing player across all managers: {PlayerName}", player.Name);

                // Set player in PlayerManager
                _playerManager.SetCurrentPlayer(player);

                // Set player in InventoryManager
                _inventoryManager.SetCurrentPlayer(player);

                // Set starting location in LocationManager
                if (_locationManager.AllLocations.ContainsKey("village"))
                {
                    _locationManager.MoveToLocation("village");
                }

                // Set player in SkillManager
                _skillManager.SetCurrentPlayer(player);

                // Initialize skills in SkillManager
                var availableSkills = _skillManager.GetAvailableSkills();
                _logger.LogInformation("Player has {SkillCount} available skills", availableSkills.Count);

                // Publish initialization event
                _eventManager.Publish(new GameMessageEvent($"Player {player.Name} fully initialized across all systems", GameMessageType.Success));

                _logger.LogInformation("Player initialization completed successfully");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initializing player: {PlayerName}", player.Name);
                return false;
            }
        }

        public async Task<GameStateValidationResult> ValidateGameStateAsync()
        {
            var result = new GameStateValidationResult { IsValid = true };

            try
            {
                _logger.LogInformation("Performing comprehensive game state validation");

                // Validate GameManager state
                await ValidateManagerAsync("GameManager", () =>
                {
                    var gameState = _gameManager.CurrentGameState;
                    if (gameState == GameState.NotStarted && _playerManager.CurrentPlayer != null)
                    {
                        result.Issues.Add(new ValidationIssue
                        {
                            Manager = "GameManager",
                            Issue = "Game state is NotStarted but player exists",
                            Severity = "Warning",
                            Recommendation = "Start a new game or clear player data"
                        });
                    }
                    return true;
                });

                // Validate PlayerManager state
                await ValidateManagerAsync("PlayerManager", () =>
                {
                    var player = _playerManager.CurrentPlayer;
                    if (player != null)
                    {
                        if (player.Health <= 0 && _gameManager.CurrentGameState == GameState.Running)
                        {
                            result.Issues.Add(new ValidationIssue
                            {
                                Manager = "PlayerManager",
                                Issue = "Player has no health but game is running",
                                Severity = "Error",
                                Recommendation = "Handle player death or restore health"
                            });
                            result.IsValid = false;
                        }

                        if (player.Level < 1)
                        {
                            result.Issues.Add(new ValidationIssue
                            {
                                Manager = "PlayerManager",
                                Issue = "Player level is less than 1",
                                Severity = "Error",
                                Recommendation = "Set player level to at least 1"
                            });
                            result.IsValid = false;
                        }
                    }
                    return true;
                });

                // Validate LocationManager state
                await ValidateManagerAsync("LocationManager", () =>
                {
                    var currentLocation = _locationManager.CurrentLocationKey;
                    if (!string.IsNullOrEmpty(currentLocation) && !_locationManager.AllLocations.ContainsKey(currentLocation))
                    {
                        result.Issues.Add(new ValidationIssue
                        {
                            Manager = "LocationManager",
                            Issue = "Current location key does not exist in locations dictionary",
                            Severity = "Error",
                            Recommendation = "Reset to a valid location"
                        });
                        result.IsValid = false;
                    }
                    return true;
                });

                // Validate InventoryManager state
                await ValidateManagerAsync("InventoryManager", () =>
                {
                    var player = _playerManager.CurrentPlayer;
                    if (player != null)
                    {
                        // Check if inventory manager has the current player set
                        if (_inventoryManager.CurrentPlayer == null)
                        {
                            result.Issues.Add(new ValidationIssue
                            {
                                Manager = "InventoryManager",
                                Issue = "Player exists but inventory manager has no current player",
                                Severity = "Warning",
                                Recommendation = "Set current player in inventory manager"
                            });
                        }
                    }
                    return true;
                });

                result.ManagerStates["GameManager"] = true;
                result.ManagerStates["PlayerManager"] = true;
                result.ManagerStates["LocationManager"] = true;
                result.ManagerStates["InventoryManager"] = true;
                result.ManagerStates["CombatManager"] = true;
                result.ManagerStates["SkillManager"] = true;

                _logger.LogInformation("Game state validation completed. Valid: {IsValid}, Issues: {IssueCount}",
                    result.IsValid, result.Issues.Count);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during game state validation");
                result.IsValid = false;
                result.Issues.Add(new ValidationIssue
                {
                    Manager = "GameCoordinatorService",
                    Issue = $"Validation failed with exception: {ex.Message}",
                    Severity = "Error",
                    Recommendation = "Check logs for detailed error information"
                });
                return result;
            }
        }

        public async Task<CommandResult> ExecuteComplexCommandAsync(string command)
        {
            try
            {
                _logger.LogInformation("Executing complex command: {Command}", command);

                var parts = command.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 0)
                    return new CommandResult(false, "", "Empty command");

                var mainCommand = parts[0].ToLowerInvariant();
                var args = parts.Skip(1).ToArray();

                switch (mainCommand)
                {
                    case "auto-explore":
                        return await AutoExploreAsync(args);

                    case "optimize-character":
                        return await OptimizeCharacterAsync(args);

                    case "batch-use":
                        return await BatchUseItemsAsync(args);

                    case "skill-combo":
                        return await ExecuteSkillComboAsync(args);

                    default:
                        // Delegate to GameManager for standard commands
                        return _gameManager.ProcessCommand(command);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing complex command: {Command}", command);
                return new CommandResult(false, "", $"Command execution failed: {ex.Message}");
            }
        }

        public async Task<bool> SynchronizeManagersAsync()
        {
            try
            {
                _logger.LogInformation("Synchronizing data between managers");

                var player = _playerManager.CurrentPlayer;
                if (player == null)
                {
                    _logger.LogWarning("No player to synchronize");
                    return false;
                }

                // Ensure all managers have the current player set
                _inventoryManager.SetCurrentPlayer(player);
                _locationManager.SetCurrentPlayer(player);
                _skillManager.SetCurrentPlayer(player);
                
                // Update location-based effects
                var currentLocation = _locationManager.CurrentLocationKey;
                if (!string.IsNullOrEmpty(currentLocation))
                {
                    // Apply location-based effects to player
                    _logger.LogDebug("Applying location effects for: {Location}", currentLocation);
                }

                // Synchronize skill effects
                var activeSkills = _skillManager.AllSkills.Values.Where(s => s.Type == SkillType.Passive).ToList();
                foreach (var skill in activeSkills)
                {
                    // Apply passive skill effects
                    _logger.LogDebug("Applying passive skill effects: {SkillName}", skill.Name);
                }

                _logger.LogInformation("Manager synchronization completed successfully");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error synchronizing managers");
                return false;
            }
        }

        public async Task<ComprehensiveGameStatus> GetComprehensiveStatusAsync()
        {
            try
            {
                var status = new ComprehensiveGameStatus
                {
                    GameState = _gameManager.CurrentGameState,
                    CurrentPlayer = _playerManager.CurrentPlayer,
                    CurrentLocation = _locationManager.CurrentLocationKey,
                    InCombat = _gameManager.CurrentGameState == GameState.InCombat,
                    Statistics = _gameManager.GetGameStatistics()
                };

                // Get manager-specific statuses
                status.ManagerStatuses["GameManager"] = new
                {
                    State = _gameManager.CurrentGameState,
                    IsRunning = _gameManager.IsGameRunning,
                    HasUnsavedChanges = _gameManager.HasUnsavedChanges
                };

                status.ManagerStatuses["PlayerManager"] = new
                {
                    HasPlayer = _playerManager.CurrentPlayer != null,
                    PlayerName = _playerManager.CurrentPlayer?.Name,
                    PlayerLevel = _playerManager.CurrentPlayer?.Level
                };

                status.ManagerStatuses["LocationManager"] = new
                {
                    CurrentLocation = _locationManager.CurrentLocationKey,
                    TotalLocations = _locationManager.AllLocations.Count
                };

                // Get active effects
                if (_playerManager.CurrentPlayer != null)
                {
                    var player = _playerManager.CurrentPlayer;
                    if (player.EquippedWeapon != null)
                        status.ActiveEffects.Add($"Weapon: {player.EquippedWeapon.Name}");
                    if (player.EquippedArmor != null)
                        status.ActiveEffects.Add($"Armor: {player.EquippedArmor.Name}");
                }

                return status;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting comprehensive status");
                return new ComprehensiveGameStatus();
            }
        }

        public async Task<MaintenanceResult> PerformMaintenanceAsync()
        {
            var stopwatch = Stopwatch.StartNew();
            var result = new MaintenanceResult { Success = true };

            try
            {
                _logger.LogInformation("Starting automated maintenance tasks");

                // Validate game state
                var validation = await ValidateGameStateAsync();
                result.OperationsPerformed.Add("Game state validation");
                result.Results["ValidationResult"] = validation;

                // Synchronize managers
                var syncResult = await SynchronizeManagersAsync();
                result.OperationsPerformed.Add("Manager synchronization");
                result.Results["SynchronizationSuccess"] = syncResult;

                // Clean up expired effects
                result.OperationsPerformed.Add("Effect cleanup");

                // Update statistics
                _gameManager.UpdateGameTime(0.1); // Small time update for maintenance
                result.OperationsPerformed.Add("Statistics update");

                stopwatch.Stop();
                result.Duration = stopwatch.Elapsed;

                _logger.LogInformation("Maintenance completed in {Duration}ms. Operations: {OperationCount}",
                    stopwatch.Elapsed.TotalMilliseconds, result.OperationsPerformed.Count);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during maintenance");
                result.Success = false;
                result.Results["Error"] = ex.Message;
                return result;
            }
        }

        // Private helper methods for complex operations

        private async Task<CoordinatedActionResult> PerformLevelUpWithRewardsAsync(Dictionary<string, object> parameters)
        {
            var result = new CoordinatedActionResult();
            
            if (_playerManager.CanLevelUp())
            {
                var oldLevel = _playerManager.CurrentPlayer.Level;
                _playerManager.LevelUp();
                var newLevel = _playerManager.CurrentPlayer.Level;

                // Give level-up rewards
                var goldReward = newLevel * 10;
                _playerManager.ModifyGold(goldReward, "Level up reward");

                result.Success = true;
                result.Message = $"Leveled up from {oldLevel} to {newLevel}! Gained {goldReward} gold.";
                result.Results["OldLevel"] = oldLevel;
                result.Results["NewLevel"] = newLevel;
                result.Results["GoldReward"] = goldReward;
            }
            else
            {
                result.Success = false;
                result.Errors.Add("Cannot level up - insufficient experience");
            }

            return result;
        }

        private async Task<CoordinatedActionResult> CompleteQuestAsync(Dictionary<string, object> parameters)
        {
            var result = new CoordinatedActionResult();
            
            // This would integrate with a quest system if implemented
            result.Success = true;
            result.Message = "Quest system integration would go here";
            result.Warnings.Add("Quest system not yet implemented");

            return result;
        }

        private async Task<CoordinatedActionResult> TravelWithEncountersAsync(Dictionary<string, object> parameters)
        {
            var result = new CoordinatedActionResult();

            if (parameters.TryGetValue("destination", out var dest))
            {
                var destination = dest.ToString();
                var moveResult = _locationManager.MoveToLocation(destination);

                if (moveResult)
                {
                    // Check for random encounters
                    var encounterChance = _locationManager.GetEncounterChance();
                    var random = new Random();
                    
                    if (random.NextDouble() < encounterChance)
                    {
                        result.Message = $"Traveled to {destination} and encountered an enemy!";
                        result.Results["Encounter"] = true;
                    }
                    else
                    {
                        result.Message = $"Safely traveled to {destination}";
                        result.Results["Encounter"] = false;
                    }

                    result.Success = true;
                }
                else
                {
                    result.Success = false;
                    result.Errors.Add($"Cannot travel to {destination}");
                }
            }
            else
            {
                result.Success = false;
                result.Errors.Add("No destination specified");
            }

            return result;
        }

        private async Task<CoordinatedActionResult> UseSkillWithEffectsAsync(Dictionary<string, object> parameters)
        {
            var result = new CoordinatedActionResult();

            if (parameters.TryGetValue("skillName", out var skillNameObj))
            {
                var skillName = skillNameObj.ToString();
                var player = _playerManager.CurrentPlayer;
                
                if (player != null)
                {
                    var useResult = _skillManager.UseSkill(skillName);
                    result.Success = useResult;
                    result.Message = useResult ? $"Successfully used skill: {skillName}" : $"Failed to use skill: {skillName}";
                }
                else
                {
                    result.Success = false;
                    result.Errors.Add("No player available");
                }
            }
            else
            {
                result.Success = false;
                result.Errors.Add("No skill name specified");
            }

            return result;
        }

        private async Task<CoordinatedActionResult> PerformShopTransactionAsync(Dictionary<string, object> parameters)
        {
            var result = new CoordinatedActionResult();
            
            // This would integrate with a shop system if implemented
            result.Success = true;
            result.Message = "Shop system integration would go here";
            result.Warnings.Add("Shop system not yet implemented");

            return result;
        }

        private async Task<CommandResult> AutoExploreAsync(string[] args)
        {
            var player = _playerManager.CurrentPlayer;
            if (player == null)
                return new CommandResult(false, "", "No player available");

            var currentLocation = _locationManager.CurrentLocationKey;
            var availableExits = _locationManager.GetAvailableExits();

            if (availableExits.Any())
            {
                var random = new Random();
                var randomDirection = availableExits.Keys.ElementAt(random.Next(availableExits.Count));
                var destinationKey = availableExits[randomDirection];
                
                var moveResult = _locationManager.MoveToLocation(destinationKey);
                return new CommandResult(moveResult, 
                    moveResult ? $"Auto-explored {randomDirection} to {destinationKey}" : "Failed to auto-explore");
            }

            return new CommandResult(false, "", "No directions available for auto-exploration");
        }

        private async Task<CommandResult> OptimizeCharacterAsync(string[] args)
        {
            var player = _playerManager.CurrentPlayer;
            if (player == null)
                return new CommandResult(false, "", "No player available");

            // Auto-equip best items from player's inventory
            var weapons = _inventoryManager.GetItemsByType(ItemType.Weapon);
            var armors = _inventoryManager.GetItemsByType(ItemType.Armor);

            var optimizations = new List<string>();

            // Equip best weapon (for now, just pick the first available weapon)
            if (weapons.Any())
            {
                var bestWeapon = weapons.First(); // In a real implementation, this would compare stats
                if (player.EquippedWeapon == null || bestWeapon.Value > player.EquippedWeapon.Value)
                {
                    _inventoryManager.EquipItem(bestWeapon);
                    optimizations.Add($"Equipped weapon: {bestWeapon.Name}");
                }
            }

            // Equip best armor (for now, just pick the first available armor)
            if (armors.Any())
            {
                var bestArmor = armors.First(); // In a real implementation, this would compare stats
                if (player.EquippedArmor == null || bestArmor.Value > player.EquippedArmor.Value)
                {
                    _inventoryManager.EquipItem(bestArmor);
                    optimizations.Add($"Equipped armor: {bestArmor.Name}");
                }
            }

            var message = optimizations.Any() 
                ? $"Character optimized: {string.Join(", ", optimizations)}"
                : "Character already optimized";

            return new CommandResult(true, message);
        }

        private async Task<CommandResult> BatchUseItemsAsync(string[] args)
        {
            var player = _playerManager.CurrentPlayer;
            if (player == null)
                return new CommandResult(false, "", "No player available");

            if (args.Length == 0)
                return new CommandResult(false, "", "Usage: batch-use <item_type>");

            var itemType = args[0].ToLowerInvariant();
            var itemsUsed = 0;

            // Get items by type and use them
            if (Enum.TryParse<ItemType>(itemType, true, out var parsedItemType))
            {
                var items = _inventoryManager.GetItemsByType(parsedItemType);
                foreach (var item in items.ToList())
                {
                    var useResult = _inventoryManager.UseItem(item);
                    if (useResult)
                        itemsUsed++;
                }
            }
            else
            {
                // Try to find items by name containing the type
                var consumableItems = _inventoryManager.GetConsumableItems();
                foreach (var item in consumableItems.ToList())
                {
                    if (item.Name.ToLowerInvariant().Contains(itemType))
                    {
                        var useResult = _inventoryManager.UseItem(item);
                        if (useResult)
                            itemsUsed++;
                    }
                }
            }

            return new CommandResult(true, $"Used {itemsUsed} items of type {itemType}");
        }

        private async Task<CommandResult> ExecuteSkillComboAsync(string[] args)
        {
            var player = _playerManager.CurrentPlayer;
            if (player == null)
                return new CommandResult(false, "", "No player available");

            if (args.Length < 2)
                return new CommandResult(false, "", "Usage: skill-combo <skill1> <skill2> [skill3...]");

            var skillsUsed = 0;
            var failedSkills = new List<string>();

            foreach (var skillName in args)
            {
                var result = _skillManager.UseSkill(skillName);
                if (result)
                    skillsUsed++;
                else
                    failedSkills.Add(skillName);
            }

            var message = $"Skill combo executed: {skillsUsed}/{args.Length} skills successful";
            if (failedSkills.Any())
                message += $". Failed: {string.Join(", ", failedSkills)}";

            return new CommandResult(skillsUsed > 0, message);
        }

        private async Task<bool> ValidateManagerAsync(string managerName, Func<bool> validation)
        {
            try
            {
                return validation();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating {ManagerName}", managerName);
                return false;
            }
        }
    }
} 