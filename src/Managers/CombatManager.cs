using System;
using System.Collections.Generic;
using System.Linq;
using WinFormsApp1.Events;
using WinFormsApp1.Interfaces;

namespace WinFormsApp1.Managers
{
    /// <summary>
    /// Manages combat system and battle mechanics
    /// </summary>
    public class CombatManager : BaseManager, ICombatManager
    {
        private readonly IPlayerManager _playerManager;
        private readonly Random _random;
        private bool _isInCombat;
        private Enemy _currentEnemy;
        private Player _currentPlayer;
        private Location _combatLocation;
        private int _currentTurn;
        private bool _isPlayerTurn;
        private bool _playerIsDefending;
        private int _defenseBonus;

        public override string ManagerName => "CombatManager";
        public bool IsInCombat => _isInCombat;
        public Enemy CurrentEnemy => _currentEnemy;
        public int CurrentTurn => _currentTurn;
        public bool IsPlayerTurn => _isPlayerTurn;

        public CombatManager(IEventManager eventManager, IPlayerManager playerManager) : base(eventManager)
        {
            _playerManager = playerManager ?? throw new ArgumentNullException(nameof(playerManager));
            _random = new Random();
            _isInCombat = false;
            _currentTurn = 0;
            _isPlayerTurn = true;
            _playerIsDefending = false;
            _defenseBonus = 0;
        }

        protected override void SubscribeToEvents()
        {
            // Subscribe to player death to end combat
            EventManager.Subscribe<PlayerDiedEvent>(OnPlayerDied);
            EventManager.Subscribe<GameStateChangedEvent>(OnGameStateChanged);
        }

        protected override void UnsubscribeFromEvents()
        {
            EventManager.ClearSubscriptions<PlayerDiedEvent>();
            EventManager.ClearSubscriptions<GameStateChangedEvent>();
        }

        public void StartCombat(Player player, Enemy enemy, Location location, bool isRandomEncounter = false)
        {
            if (player == null)
                throw new ArgumentNullException(nameof(player));
            if (enemy == null)
                throw new ArgumentNullException(nameof(enemy));

            LogMessage($"Starting combat: {player.Name} vs {enemy.Name}");

            _currentPlayer = player;
            _currentEnemy = enemy;
            _combatLocation = location;
            _isInCombat = true;
            _currentTurn = 1;
            _isPlayerTurn = true;
            _playerIsDefending = false;
            _defenseBonus = 0;

            // Reset enemy health if needed
            if (_currentEnemy.Health <= 0)
            {
                _currentEnemy.Health = _currentEnemy.MaxHealth;
            }

            EventManager.Publish(new CombatStartedEvent(player, enemy, location, isRandomEncounter));
            LogMessage($"Combat started - Turn {_currentTurn}");
        }

        public void EndCombat(CombatResult result)
        {
            if (!_isInCombat)
                return;

            LogMessage($"Ending combat with result: {result}");

            var experienceGained = 0;
            var goldGained = 0;
            var lootGained = new List<Item>();

            // Calculate rewards for victory
            if (result == CombatResult.Victory && _currentEnemy != null)
            {
                experienceGained = CalculateExperienceReward(_currentEnemy, _currentPlayer);
                goldGained = CalculateGoldReward(_currentEnemy);
                lootGained = GenerateLootDrops(_currentEnemy);

                // Award experience and gold
                if (experienceGained > 0)
                {
                    _playerManager.AddExperience(experienceGained, $"Defeated {_currentEnemy.Name}");
                }

                if (goldGained > 0)
                {
                    _playerManager.ModifyGold(goldGained, $"Defeated {_currentEnemy.Name}");
                }

                // Publish enemy defeated event
                EventManager.Publish(new EnemyDefeatedEvent(_currentPlayer, _currentEnemy, experienceGained, goldGained, lootGained));
            }

            // Publish combat ended event
            EventManager.Publish(new CombatEndedEvent(_currentPlayer, _currentEnemy, result, experienceGained, goldGained, lootGained));

            // Reset combat state
            _isInCombat = false;
            _currentEnemy = null;
            _currentPlayer = null;
            _combatLocation = null;
            _currentTurn = 0;
            _isPlayerTurn = true;
            _playerIsDefending = false;
            _defenseBonus = 0;

            LogMessage("Combat ended");
        }

        public AttackResult PlayerAttack(Player player)
        {
            if (!_isInCombat || !_isPlayerTurn || _currentEnemy == null)
                throw new InvalidOperationException("Cannot attack: not in combat or not player's turn");

            LogMessage($"{player.Name} attacks {_currentEnemy.Name}");

            var isCritical = IsCriticalHit(player);
            var isMissed = IsAttackMissed(player, _currentEnemy);
            var damage = 0;

            if (!isMissed)
            {
                damage = CalculateDamage(player, _currentEnemy, player.Attack);
                if (isCritical)
                {
                    damage = (int)(damage * 1.5); // 50% bonus for critical hits
                }

                // Apply damage to enemy
                _currentEnemy.Health = Math.Max(0, _currentEnemy.Health - damage);
            }

            var result = new AttackResult(damage, isCritical, isMissed)
            {
                Description = GenerateAttackDescription(player.Name, _currentEnemy.Name, damage, isCritical, isMissed)
            };

            EventManager.Publish(new PlayerAttackedEvent(player, _currentEnemy, damage, isCritical, isMissed));

            // Check if enemy is defeated
            if (_currentEnemy.Health <= 0)
            {
                EndCombat(CombatResult.Victory);
            }
            else
            {
                // End player turn
                _isPlayerTurn = false;
                ProcessTurnEnd();
            }

            return result;
        }

        public AttackResult EnemyAttack(Enemy enemy, Player player)
        {
            if (!_isInCombat || _isPlayerTurn || enemy == null)
                throw new InvalidOperationException("Cannot attack: not in combat or not enemy's turn");

            LogMessage($"{enemy.Name} attacks {player.Name}");

            var isCritical = IsCriticalHit(enemy);
            var isMissed = IsAttackMissed(enemy, player);
            var wasBlocked = false;
            var damage = 0;

            if (!isMissed)
            {
                damage = CalculateDamage(enemy, player, enemy.Attack);
                if (isCritical)
                {
                    damage = (int)(damage * 1.5);
                }

                // Apply defense bonus if player is defending
                if (_playerIsDefending)
                {
                    var reducedDamage = Math.Max(1, damage - _defenseBonus);
                    if (reducedDamage < damage)
                    {
                        wasBlocked = true;
                        damage = reducedDamage;
                    }
                }

                // Apply damage to player
                _playerManager.TakeDamage(damage, $"Attack from {enemy.Name}");
            }

            var result = new AttackResult(damage, isCritical, isMissed, wasBlocked)
            {
                Description = GenerateAttackDescription(enemy.Name, player.Name, damage, isCritical, isMissed, wasBlocked)
            };

            EventManager.Publish(new EnemyAttackedEvent(enemy, player, damage, isCritical, isMissed, wasBlocked));

            // Check if player died
            if (_playerManager.IsDead())
            {
                EndCombat(CombatResult.Defeat);
            }
            else
            {
                // End enemy turn
                _isPlayerTurn = true;
                _playerIsDefending = false; // Reset defense
                _defenseBonus = 0;
                ProcessTurnEnd();
            }

            return result;
        }

        public DefenseResult PlayerDefend(Player player)
        {
            if (!_isInCombat || !_isPlayerTurn)
                throw new InvalidOperationException("Cannot defend: not in combat or not player's turn");

            LogMessage($"{player.Name} defends");

            _playerIsDefending = true;
            _defenseBonus = player.Defense + _random.Next(1, 6); // Defense + 1-5 bonus

            var result = new DefenseResult(_defenseBonus)
            {
                Description = $"{player.Name} takes a defensive stance, gaining +{_defenseBonus} defense"
            };

            EventManager.Publish(new PlayerDefendedEvent(player, _defenseBonus, true));

            // End player turn
            _isPlayerTurn = false;
            ProcessTurnEnd();

            return result;
        }

        public bool PlayerFlee(Player player)
        {
            if (!_isInCombat || !_isPlayerTurn)
                throw new InvalidOperationException("Cannot flee: not in combat or not player's turn");

            // Calculate flee chance based on player level vs enemy level
            var playerLevel = player.Level;
            var enemyLevel = _currentEnemy?.Level ?? 1;
            var baseFleeChance = 0.5; // 50% base chance
            var levelDifference = playerLevel - enemyLevel;
            var fleeChance = Math.Max(0.1, Math.Min(0.9, baseFleeChance + (levelDifference * 0.1)));

            var success = _random.NextDouble() < fleeChance;

            LogMessage($"{player.Name} attempts to flee (chance: {fleeChance:P0}) - {(success ? "Success" : "Failed")}");

            EventManager.Publish(new PlayerFleeAttemptEvent(player, _currentEnemy, success, fleeChance));

            if (success)
            {
                EndCombat(CombatResult.Fled);
            }
            else
            {
                // Failed to flee, end player turn
                _isPlayerTurn = false;
                ProcessTurnEnd();
            }

            return success;
        }

        public bool UseItem(Player player, Item item)
        {
            if (!_isInCombat || !_isPlayerTurn)
                throw new InvalidOperationException("Cannot use item: not in combat or not player's turn");

            if (item == null || !player.Inventory.Contains(item))
                return false;

            LogMessage($"{player.Name} uses {item.Name}");

            var success = false;
            var effect = "";
            var effectValue = 0;

            // Handle different item types
            switch (item.Type)
            {
                case ItemType.Consumable:
                    if (item.Name.ToLower().Contains("potion") && item.Name.ToLower().Contains("health"))
                    {
                        var healAmount = item.Value;
                        _playerManager.Heal(healAmount, $"Used {item.Name}");
                        success = true;
                        effect = "Healing";
                        effectValue = healAmount;
                    }
                    break;
            }

            if (success)
            {
                // Remove item from inventory
                player.Inventory.Remove(item);
                EventManager.Publish(new ItemUsedEvent(player, item, success, effect, effectValue));

                // End player turn
                _isPlayerTurn = false;
                ProcessTurnEnd();
            }

            return success;
        }

        public int CalculateDamage(object attacker, object defender, int baseAttack)
        {
            var damage = baseAttack;
            var defenseValue = 0;

            // Get defender's defense
            if (defender is Player player)
            {
                defenseValue = player.Defense;
            }
            else if (defender is Enemy enemy)
            {
                defenseValue = enemy.Defense;
            }

            // Add randomness (±20%)
            var randomFactor = 0.8 + (_random.NextDouble() * 0.4);
            damage = (int)(damage * randomFactor);

            // Apply defense reduction
            damage = Math.Max(1, damage - (defenseValue / 2));

            return damage;
        }

        public bool IsCriticalHit(object attacker)
        {
            // 10% base critical hit chance
            var critChance = 0.1;

            // Increase crit chance based on attacker level
            if (attacker is Player player)
            {
                critChance += player.Level * 0.01; // +1% per level
            }
            else if (attacker is Enemy enemy)
            {
                critChance += enemy.Level * 0.005; // +0.5% per level for enemies
            }

            return _random.NextDouble() < Math.Min(0.5, critChance); // Cap at 50%
        }

        public bool IsAttackMissed(object attacker, object defender)
        {
            // 10% base miss chance
            var missChance = 0.1;

            // Adjust based on level difference
            if (attacker is Player player && defender is Enemy enemy)
            {
                var levelDiff = enemy.Level - player.Level;
                missChance += levelDiff * 0.02; // +2% miss per level difference
            }
            else if (attacker is Enemy enemy2 && defender is Player player2)
            {
                var levelDiff = player2.Level - enemy2.Level;
                missChance += levelDiff * 0.01; // +1% miss per level difference
            }

            return _random.NextDouble() < Math.Max(0.05, Math.Min(0.3, missChance)); // 5-30% range
        }

        public int CalculateExperienceReward(Enemy enemy, Player player)
        {
            var baseExp = enemy.Level * 10;
            var levelDiff = enemy.Level - player.Level;
            
            // Bonus/penalty based on level difference
            var multiplier = 1.0 + (levelDiff * 0.1);
            multiplier = Math.Max(0.1, Math.Min(2.0, multiplier)); // 10%-200% range

            return (int)(baseExp * multiplier);
        }

        public int CalculateGoldReward(Enemy enemy)
        {
            var baseGold = enemy.Level * 5;
            var randomBonus = _random.Next(0, enemy.Level + 1);
            return baseGold + randomBonus;
        }

        public List<Item> GenerateLootDrops(Enemy enemy)
        {
            var loot = new List<Item>();
            
            // 30% chance for item drop
            if (_random.NextDouble() < 0.3)
            {
                // Simple loot generation - in a real game this would be more sophisticated
                var itemTypes = new[] { "Health Potion", "Weapon Fragment", "Armor Piece" };
                var itemName = itemTypes[_random.Next(itemTypes.Length)];
                
                var item = new Item
                {
                    Name = itemName,
                    Type = ItemType.Consumable,
                    Value = enemy.Level * 2,
                    Description = $"Dropped by {enemy.Name}"
                };
                
                loot.Add(item);
            }

            return loot;
        }

        public void ProcessTurnEnd()
        {
            if (!_isInCombat)
                return;

            if (!_isPlayerTurn)
            {
                // Enemy's turn
                _currentTurn++;
                LogMessage($"Turn {_currentTurn} - Enemy's turn");
                
                // Enemy AI - simple attack for now
                if (_currentEnemy != null && _currentEnemy.Health > 0)
                {
                    EnemyAttack(_currentEnemy, _currentPlayer);
                }
            }
            else
            {
                LogMessage($"Turn {_currentTurn} - Player's turn");
            }
        }

        public List<string> GetAvailableCombatActions()
        {
            if (!_isInCombat || !_isPlayerTurn)
                return new List<string>();

            var actions = new List<string> { "attack", "defend", "flee" };

            // Add item actions if player has usable items
            if (_currentPlayer?.Inventory?.Any(item => item.Type == ItemType.Consumable) == true)
            {
                actions.Add("use");
            }

            return actions;
        }

        private string GenerateAttackDescription(string attackerName, string defenderName, int damage, bool isCritical, bool isMissed, bool wasBlocked = false)
        {
            if (isMissed)
            {
                return $"{attackerName} attacks {defenderName} but misses!";
            }

            var description = $"{attackerName} attacks {defenderName}";
            
            if (isCritical)
            {
                description += " with a critical hit";
            }

            if (wasBlocked)
            {
                description += $" for {damage} damage (reduced by defense)";
            }
            else
            {
                description += $" for {damage} damage";
            }

            return description + "!";
        }

        private void OnPlayerDied(PlayerDiedEvent e)
        {
            if (_isInCombat)
            {
                LogMessage("Player died in combat");
                EndCombat(CombatResult.Defeat);
            }
        }

        private void OnGameStateChanged(GameStateChangedEvent e)
        {
            // End combat if game state changes to something incompatible
            if (_isInCombat && e.NewState != GameState.InCombat && e.NewState != GameState.Running)
            {
                LogMessage("Game state changed, ending combat");
                EndCombat(CombatResult.Draw);
            }
        }
    }
} 