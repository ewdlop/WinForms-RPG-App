using System;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using WinFormsApp1.Constants;

namespace WinFormsApp1.Events
{
    public class GameEventLogger : IPlayerEventHandler, ICombatEventHandler, IWorldEventHandler, IGameStateEventHandler
    {
        private readonly string logFilePath;
        private readonly List<GameEvent> eventLog;
        private readonly GameEventManager eventManager;
        private readonly HashSet<string> unlockedAchievements;

        public GameEventLogger(string logDirectory = "Logs")
        {
            // Create logs directory
            Directory.CreateDirectory(logDirectory);
            
            // Create log file with timestamp
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            logFilePath = Path.Combine(logDirectory, $"game_events_{timestamp}.json");
            
            eventLog = new List<GameEvent>();
            unlockedAchievements = new HashSet<string>();
            eventManager = GameEventManager.Instance;
            
            // Subscribe to all events
            SubscribeToEvents();
        }

        private void SubscribeToEvents()
        {
            eventManager.SubscribeToPlayerEvents(this);
            eventManager.SubscribeToCombatEvents(this);
            eventManager.SubscribeToWorldEvents(this);
            eventManager.SubscribeToGameStateEvents(this);
        }

        public void UnsubscribeFromEvents()
        {
            eventManager.UnsubscribeFromPlayerEvents(this);
            eventManager.UnsubscribeFromCombatEvents(this);
            eventManager.UnsubscribeFromWorldEvents(this);
            eventManager.UnsubscribeFromGameStateEvents(this);
        }

        // IPlayerEventHandler implementation
        public void OnPlayerLeveledUp(object? sender, LevelUpEventArgs e)
        {
            LogEvent("PlayerLevelUp", new
            {
                PlayerName = e.Player.Name,
                OldLevel = e.OldLevel,
                NewLevel = e.NewLevel,
                HealthGain = e.HealthGain,
                AttackGain = e.AttackGain,
                DefenseGain = e.DefenseGain,
                SkillPointsGained = e.SkillPointsGained
            });

            // Check for level-based achievements
            CheckLevelAchievements(e.NewLevel);
        }

        public void OnPlayerGainedExperience(object? sender, ExperienceGainEventArgs e)
        {
            LogEvent("ExperienceGain", new
            {
                PlayerName = e.Player.Name,
                ExperienceGained = e.ExperienceGained,
                TotalExperience = e.TotalExperience,
                LeveledUp = e.LeveledUp
            });
        }

        public void OnPlayerGoldChanged(object? sender, GoldChangeEventArgs e)
        {
            LogEvent("GoldChange", new
            {
                PlayerName = e.Player.Name,
                GoldChange = e.GoldChange,
                TotalGold = e.TotalGold,
                Reason = e.Reason
            });

            // Check for wealth-based achievements
            CheckWealthAchievements(e.TotalGold);
        }

        public void OnPlayerHealthChanged(object? sender, HealthChangeEventArgs e)
        {
            LogEvent("HealthChange", new
            {
                PlayerName = e.Player.Name,
                HealthChange = e.HealthChange,
                CurrentHealth = e.CurrentHealth,
                MaxHealth = e.MaxHealth,
                Reason = e.Reason
            });

            // Check for near-death experiences
            if (e.CurrentHealth <= e.MaxHealth * 0.1 && e.CurrentHealth > 0)
            {
                LogEvent("NearDeathExperience", new
                {
                    PlayerName = e.Player.Name,
                    HealthPercentage = (double)e.CurrentHealth / e.MaxHealth * 100
                });
                
                CheckAchievement("Survivor", "Survived with less than 10% health");
            }
        }

        public void OnPlayerLearnedSkill(object? sender, SkillEventArgs e)
        {
            LogEvent("SkillLearned", new
            {
                PlayerName = e.Player.Name,
                SkillName = e.SkillName,
                SkillPointsCost = e.SkillPointsCost,
                SkillDescription = e.SkillDescription
            });
        }

        // ICombatEventHandler implementation
        public void OnCombatStarted(object? sender, CombatEventArgs e)
        {
            LogEvent("CombatStarted", new
            {
                PlayerName = e.Player.Name,
                PlayerLevel = e.Player.Level,
                EnemyName = e.Enemy.Name,
                EnemyLevel = e.Enemy.Level
            });
        }

        public void OnCombatActionPerformed(object? sender, CombatActionEventArgs e)
        {
            LogEvent("CombatAction", new
            {
                PlayerName = e.Player.Name,
                EnemyName = e.Enemy.Name,
                Action = e.Action,
                Damage = e.Damage,
                IsPlayerAction = e.IsPlayerAction,
                IsCritical = e.IsCritical
            });

            // Track critical hits
            if (e.IsCritical && e.IsPlayerAction)
            {
                LogEvent("CriticalHit", new
                {
                    PlayerName = e.Player.Name,
                    EnemyName = e.Enemy.Name,
                    Damage = e.Damage
                });
                
                CheckCriticalHitAchievements();
            }
        }

        public void OnCombatEnded(object? sender, CombatEndEventArgs e)
        {
            LogEvent("CombatEnded", new
            {
                PlayerName = e.Player.Name,
                EnemyName = e.Enemy.Name,
                PlayerWon = e.PlayerWon,
                ExperienceGained = e.ExperienceGained,
                GoldGained = e.GoldGained,
                LootCount = e.LootGained.Count
            });

            // Track combat statistics
            if (e.PlayerWon)
            {
                CheckCombatAchievements(e.Enemy.Name);
            }
        }

        // IWorldEventHandler implementation
        public void OnPlayerMovedLocation(object? sender, LocationChangeEventArgs e)
        {
            LogEvent("LocationChange", new
            {
                PlayerName = e.Player.Name,
                OldLocation = e.OldLocation?.Name ?? "Unknown",
                NewLocation = e.NewLocation.Name,
                MovementDirection = e.MovementDirection
            });

            // Track exploration
            CheckExplorationAchievements(e.NewLocation.Name);
        }

        public void OnInventoryChanged(object? sender, InventoryChangeEventArgs e)
        {
            LogEvent("InventoryChange", new
            {
                PlayerName = e.Player.Name,
                ItemName = e.Item.Name,
                ItemAdded = e.ItemAdded,
                Quantity = e.Quantity,
                LocationName = e.Location?.Name
            });
        }

        public void OnItemUsed(object? sender, ItemUsedEventArgs e)
        {
            LogEvent("ItemUsed", new
            {
                PlayerName = e.Player.Name,
                ItemName = e.Item.Name,
                Effect = e.Effect,
                WasConsumed = e.WasConsumed
            });
        }

        // IGameStateEventHandler implementation
        public void OnGameStateChanged(object? sender, GameStateEventArgs e)
        {
            LogEvent("GameStateChange", new
            {
                StateName = e.StateName,
                Timestamp = e.Timestamp
            });
        }

        public void OnGameSaved(object? sender, SaveGameEventArgs e)
        {
            LogEvent("GameSaved", new
            {
                SaveName = e.SaveName,
                SavePath = e.SavePath,
                WasSuccessful = e.WasSuccessful,
                ErrorMessage = e.ErrorMessage
            });
        }

        public void OnGameLoaded(object? sender, LoadGameEventArgs e)
        {
            LogEvent("GameLoaded", new
            {
                SaveName = e.SaveName,
                SavePath = e.SavePath,
                WasSuccessful = e.WasSuccessful,
                PlayerName = e.LoadedPlayer?.Name,
                ErrorMessage = e.ErrorMessage
            });
        }

        public void OnCommandExecuted(object? sender, CommandEventArgs e)
        {
            LogEvent("CommandExecuted", new
            {
                Command = e.Command,
                Arguments = e.Arguments,
                PlayerName = e.Player?.Name,
                WasHandled = e.WasHandled
            });
        }

        public void OnCheatActivated(object? sender, CheatEventArgs e)
        {
            LogEvent("CheatActivated", new
            {
                CheatCommand = e.CheatCommand,
                Arguments = e.Arguments,
                PlayerName = e.Player?.Name,
                WasSuccessful = e.WasSuccessful
            });
        }

        // Achievement checking methods
        private void CheckLevelAchievements(int level)
        {
            var achievements = new Dictionary<int, string>
            {
                [5] = "Novice Adventurer",
                [10] = "Experienced Explorer",
                [20] = "Veteran Warrior",
                [50] = "Legendary Hero",
                [100] = "Mythical Champion"
            };

            if (achievements.TryGetValue(level, out string? achievement))
            {
                CheckAchievement(achievement, $"Reached level {level}");
            }
        }

        private void CheckWealthAchievements(int gold)
        {
            var achievements = new Dictionary<int, string>
            {
                [1000] = "Wealthy Merchant",
                [10000] = "Rich Noble",
                [100000] = "Treasure Hoarder",
                [1000000] = "Dragon's Fortune"
            };

            foreach (var kvp in achievements)
            {
                if (gold >= kvp.Key && !unlockedAchievements.Contains(kvp.Value))
                {
                    CheckAchievement(kvp.Value, $"Accumulated {kvp.Key:N0} gold");
                }
            }
        }

        private void CheckCombatAchievements(string enemyName)
        {
            // Count enemy defeats
            var combatEvents = eventLog.Where(e => e.EventType == "CombatEnded").Count();
            
            // Check for combat milestones
            var combatAchievements = new Dictionary<int, string>
            {
                [1] = "First Blood",
                [10] = "Warrior",
                [50] = "Veteran",
                [100] = "Slayer",
                [500] = "Destroyer"
            };

            foreach (var kvp in combatAchievements)
            {
                if (combatEvents >= kvp.Key && !unlockedAchievements.Contains(kvp.Value))
                {
                    CheckAchievement(kvp.Value, $"Won {kvp.Key} combats");
                }
            }

            // Check for specific enemy achievements
            var specificEnemyDefeats = eventLog.Where(e => 
                e.EventType == "CombatEnded" && 
                e.Data.ToString()?.Contains($"\"EnemyName\":\"{enemyName}\"") == true &&
                e.Data.ToString()?.Contains("\"PlayerWon\":true") == true).Count();

            var enemyAchievements = new Dictionary<string, Dictionary<int, string>>
            {
                ["Goblin"] = new() { [10] = "Goblin Slayer", [50] = "Goblin Bane" },
                ["Dragon"] = new() { [1] = "Dragon Slayer", [5] = "Dragon Lord" },
                ["Orc"] = new() { [25] = "Orc Hunter", [100] = "Orc Nemesis" }
            };

            if (enemyAchievements.TryGetValue(enemyName, out var enemySpecificAchievements))
            {
                foreach (var kvp in enemySpecificAchievements)
                {
                    if (specificEnemyDefeats >= kvp.Key && !unlockedAchievements.Contains(kvp.Value))
                    {
                        CheckAchievement(kvp.Value, $"Defeated {kvp.Key} {enemyName}s");
                    }
                }
            }
        }

        private void CheckExplorationAchievements(string locationName)
        {
            var visitedLocations = eventLog.Where(e => e.EventType == "LocationChange")
                .Select(e => GetLocationFromEventData(e.Data))
                .Where(loc => !string.IsNullOrEmpty(loc))
                .Distinct()
                .Count();

            var achievements = new Dictionary<int, string>
            {
                [3] = "Local Explorer",
                [5] = "World Traveler",
                [10] = "Master Explorer"
            };

            foreach (var kvp in achievements)
            {
                if (visitedLocations >= kvp.Key && !unlockedAchievements.Contains(kvp.Value))
                {
                    CheckAchievement(kvp.Value, $"Visited {kvp.Key} different locations");
                }
            }
        }

        private void CheckCriticalHitAchievements()
        {
            var criticalHits = eventLog.Where(e => e.EventType == "CriticalHit").Count();
            
            var achievements = new Dictionary<int, string>
            {
                [1] = "Lucky Strike",
                [10] = "Critical Master",
                [50] = "Precision Fighter"
            };

            foreach (var kvp in achievements)
            {
                if (criticalHits >= kvp.Key && !unlockedAchievements.Contains(kvp.Value))
                {
                    CheckAchievement(kvp.Value, $"Landed {kvp.Key} critical hits");
                }
            }
        }

        private string GetLocationFromEventData(object data)
        {
            try
            {
                var jsonString = data.ToString();
                if (string.IsNullOrEmpty(jsonString)) return "";
                
                using var doc = JsonDocument.Parse(jsonString);
                if (doc.RootElement.TryGetProperty("NewLocation", out var locationElement))
                {
                    return locationElement.GetString() ?? "";
                }
            }
            catch
            {
                // Ignore parsing errors
            }
            return "";
        }

        private void CheckAchievement(string achievementName, string description)
        {
            if (unlockedAchievements.Add(achievementName))
            {
                LogAchievement(achievementName, description);
            }
        }

        private void LogEvent(string eventType, object data)
        {
            var gameEvent = new GameEvent
            {
                EventType = eventType,
                Timestamp = DateTime.Now,
                Data = data
            };

            eventLog.Add(gameEvent);
            
            // Optionally write to file immediately for real-time logging
            WriteEventToFile(gameEvent);
        }

        private void LogAchievement(string achievementName, string description)
        {
            LogEvent("Achievement", new
            {
                AchievementName = achievementName,
                Description = description,
                UnlockedAt = DateTime.Now
            });

            // Publish achievement event
            eventManager.PublishMessage($"🏆 ACHIEVEMENT UNLOCKED: {achievementName}!", Color.Gold, MessageType.Success);
            eventManager.PublishMessage($"   {description}", Color.Yellow, MessageType.Success);
        }

        private void WriteEventToFile(GameEvent gameEvent)
        {
            try
            {
                string jsonLine = JsonSerializer.Serialize(gameEvent, new JsonSerializerOptions { WriteIndented = false });
                File.AppendAllText(logFilePath, jsonLine + Environment.NewLine);
            }
            catch (Exception ex)
            {
                // Handle logging errors silently to avoid disrupting gameplay
                Console.WriteLine($"Failed to write event to log: {ex.Message}");
            }
        }

        public void SaveEventLog()
        {
            try
            {
                string json = JsonSerializer.Serialize(eventLog, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(logFilePath.Replace(".json", "_complete.json"), json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to save complete event log: {ex.Message}");
            }
        }

        public List<GameEvent> GetEventLog() => new List<GameEvent>(eventLog);

        public Dictionary<string, int> GetEventStatistics()
        {
            return eventLog.GroupBy(e => e.EventType)
                          .ToDictionary(g => g.Key, g => g.Count());
        }

        public List<GameEvent> GetAchievements()
        {
            return eventLog.Where(e => e.EventType == "Achievement").ToList();
        }

        public HashSet<string> GetUnlockedAchievements()
        {
            return new HashSet<string>(unlockedAchievements);
        }

        public void Dispose()
        {
            SaveEventLog();
            UnsubscribeFromEvents();
        }
    }

    public class GameEvent
    {
        public string EventType { get; set; } = "";
        public DateTime Timestamp { get; set; }
        public object Data { get; set; } = new();
    }
} 