using System;
using WinFormsApp1.Events;
using WinFormsApp1.Interfaces;

namespace WinFormsApp1.Managers
{
    /// <summary>
    /// Manages player state and progression
    /// </summary>
    public class PlayerManager : BaseManager, IPlayerManager
    {
        private Player _currentPlayer;

        public override string ManagerName => "PlayerManager";
        public Player CurrentPlayer => _currentPlayer;

        public PlayerManager(IEventManager eventManager) : base(eventManager)
        {
        }

        protected override void SubscribeToEvents()
        {
            // PlayerManager doesn't need to subscribe to events initially
            // It primarily publishes events when player state changes
        }

        protected override void UnsubscribeFromEvents()
        {
            // No subscriptions to clean up
        }

        public void SetCurrentPlayer(Player player)
        {
            if (player == null)
                throw new ArgumentNullException(nameof(player));

            var oldPlayer = _currentPlayer;
            _currentPlayer = player;

            LogMessage($"Current player set to: {player.Name} ({player.CharacterClass})");

            // Publish initial stats
            if (oldPlayer != player)
            {
                PublishAllStatsChanged();
            }
        }

        public void UpdateHealth(int newHealth, string reason = "")
        {
            if (_currentPlayer == null)
                throw new InvalidOperationException("No current player set");

            var oldHealth = _currentPlayer.Health;
            _currentPlayer.Health = Math.Max(0, Math.Min(newHealth, _currentPlayer.MaxHealth));

            if (oldHealth != _currentPlayer.Health)
            {
                EventManager.Publish(new PlayerHealthChangedEvent(_currentPlayer, oldHealth, _currentPlayer.Health, reason));
                EventManager.Publish(new PlayerStatsChangedEvent(_currentPlayer, StatType.Health, oldHealth, _currentPlayer.Health));

                if (_currentPlayer.Health <= 0 && oldHealth > 0)
                {
                    EventManager.Publish(new PlayerDiedEvent(_currentPlayer, reason));
                }
            }
        }

        public void AddExperience(int experience, string source = "")
        {
            if (_currentPlayer == null)
                throw new InvalidOperationException("No current player set");

            if (experience <= 0)
                return;

            var oldExperience = _currentPlayer.Experience;
            _currentPlayer.Experience += experience;

            EventManager.Publish(new PlayerExperienceGainedEvent(_currentPlayer, experience, source));
            EventManager.Publish(new PlayerStatsChangedEvent(_currentPlayer, StatType.Experience, oldExperience, _currentPlayer.Experience));

            // Check for level up
            while (CanLevelUp())
            {
                LevelUp();
            }
        }

        public void ModifyGold(int amount, string reason = "")
        {
            if (_currentPlayer == null)
                throw new InvalidOperationException("No current player set");

            var oldGold = _currentPlayer.Gold;
            _currentPlayer.Gold = Math.Max(0, _currentPlayer.Gold + amount);

            if (oldGold != _currentPlayer.Gold)
            {
                EventManager.Publish(new PlayerGoldChangedEvent(_currentPlayer, oldGold, _currentPlayer.Gold, reason));
                EventManager.Publish(new PlayerStatsChangedEvent(_currentPlayer, StatType.Gold, oldGold, _currentPlayer.Gold));
            }
        }

        public void LevelUp()
        {
            if (_currentPlayer == null)
                throw new InvalidOperationException("No current player set");

            if (!CanLevelUp())
                return;

            var oldLevel = _currentPlayer.Level;
            _currentPlayer.Level++;

            // Deduct experience for level up
            _currentPlayer.Experience -= _currentPlayer.ExperienceToNextLevel;

            // Calculate new experience requirement (increases by 50% each level)
            _currentPlayer.ExperienceToNextLevel = (int)(_currentPlayer.ExperienceToNextLevel * 1.5);

            // Increase stats based on character class
            IncreaseStatsOnLevelUp();

            // Award skill points
            var skillPointsGained = 3; // Base skill points per level
            AddSkillPoints(skillPointsGained);

            LogMessage($"Player leveled up! Level {oldLevel} -> {_currentPlayer.Level}");

            EventManager.Publish(new PlayerLeveledUpEvent(_currentPlayer, oldLevel, _currentPlayer.Level, skillPointsGained));
            EventManager.Publish(new PlayerStatsChangedEvent(_currentPlayer, StatType.Level, oldLevel, _currentPlayer.Level));
        }

        public bool CanLevelUp()
        {
            return _currentPlayer != null && _currentPlayer.Experience >= _currentPlayer.ExperienceToNextLevel;
        }

        public void Heal(int amount, string source = "")
        {
            if (_currentPlayer == null)
                throw new InvalidOperationException("No current player set");

            if (amount <= 0)
                return;

            var newHealth = Math.Min(_currentPlayer.Health + amount, _currentPlayer.MaxHealth);
            UpdateHealth(newHealth, $"Healed by {source}");
        }

        public void TakeDamage(int damage, string source = "")
        {
            if (_currentPlayer == null)
                throw new InvalidOperationException("No current player set");

            if (damage <= 0)
                return;

            var newHealth = Math.Max(0, _currentPlayer.Health - damage);
            UpdateHealth(newHealth, $"Damage from {source}");
        }

        public bool IsDead()
        {
            return _currentPlayer?.Health <= 0;
        }

        public void Revive(int health)
        {
            if (_currentPlayer == null)
                throw new InvalidOperationException("No current player set");

            if (!IsDead())
                return;

            UpdateHealth(Math.Min(health, _currentPlayer.MaxHealth), "Revived");
            LogMessage("Player has been revived!");
        }

        public void AddSkillPoints(int points)
        {
            if (_currentPlayer == null)
                throw new InvalidOperationException("No current player set");

            if (points <= 0)
                return;

            var oldSkillPoints = _currentPlayer.SkillPoints;
            _currentPlayer.SkillPoints += points;

            EventManager.Publish(new PlayerStatsChangedEvent(_currentPlayer, StatType.SkillPoints, oldSkillPoints, _currentPlayer.SkillPoints));
        }

        public bool SpendSkillPoints(int points)
        {
            if (_currentPlayer == null)
                throw new InvalidOperationException("No current player set");

            if (points <= 0 || _currentPlayer.SkillPoints < points)
                return false;

            var oldSkillPoints = _currentPlayer.SkillPoints;
            _currentPlayer.SkillPoints -= points;

            EventManager.Publish(new PlayerStatsChangedEvent(_currentPlayer, StatType.SkillPoints, oldSkillPoints, _currentPlayer.SkillPoints));
            return true;
        }

        private void IncreaseStatsOnLevelUp()
        {
            var oldMaxHealth = _currentPlayer.MaxHealth;
            var oldAttack = _currentPlayer.Attack;
            var oldDefense = _currentPlayer.Defense;

            // Increase stats based on character class
            switch (_currentPlayer.CharacterClass)
            {
                case CharacterClass.Warrior:
                    _currentPlayer.MaxHealth += 15;
                    _currentPlayer.Attack += 3;
                    _currentPlayer.Defense += 2;
                    break;
                case CharacterClass.Mage:
                    _currentPlayer.MaxHealth += 8;
                    _currentPlayer.Attack += 4;
                    _currentPlayer.Defense += 1;
                    break;
                case CharacterClass.Rogue:
                    _currentPlayer.MaxHealth += 10;
                    _currentPlayer.Attack += 4;
                    _currentPlayer.Defense += 1;
                    break;
                case CharacterClass.Cleric:
                    _currentPlayer.MaxHealth += 12;
                    _currentPlayer.Attack += 2;
                    _currentPlayer.Defense += 3;
                    break;
            }

            // Heal to full health on level up
            _currentPlayer.Health = _currentPlayer.MaxHealth;

            // Publish stat change events
            EventManager.Publish(new PlayerStatsChangedEvent(_currentPlayer, StatType.MaxHealth, oldMaxHealth, _currentPlayer.MaxHealth));
            EventManager.Publish(new PlayerStatsChangedEvent(_currentPlayer, StatType.Attack, oldAttack, _currentPlayer.Attack));
            EventManager.Publish(new PlayerStatsChangedEvent(_currentPlayer, StatType.Defense, oldDefense, _currentPlayer.Defense));
            EventManager.Publish(new PlayerHealthChangedEvent(_currentPlayer, _currentPlayer.Health, _currentPlayer.MaxHealth, "Level up heal"));
        }

        private void PublishAllStatsChanged()
        {
            if (_currentPlayer == null)
                return;

            // Publish events for all current stats (useful for UI initialization)
            EventManager.Publish(new PlayerStatsChangedEvent(_currentPlayer, StatType.Health, _currentPlayer.Health, _currentPlayer.Health));
            EventManager.Publish(new PlayerStatsChangedEvent(_currentPlayer, StatType.MaxHealth, _currentPlayer.MaxHealth, _currentPlayer.MaxHealth));
            EventManager.Publish(new PlayerStatsChangedEvent(_currentPlayer, StatType.Attack, _currentPlayer.Attack, _currentPlayer.Attack));
            EventManager.Publish(new PlayerStatsChangedEvent(_currentPlayer, StatType.Defense, _currentPlayer.Defense, _currentPlayer.Defense));
            EventManager.Publish(new PlayerStatsChangedEvent(_currentPlayer, StatType.Experience, _currentPlayer.Experience, _currentPlayer.Experience));
            EventManager.Publish(new PlayerStatsChangedEvent(_currentPlayer, StatType.Level, _currentPlayer.Level, _currentPlayer.Level));
            EventManager.Publish(new PlayerStatsChangedEvent(_currentPlayer, StatType.Gold, _currentPlayer.Gold, _currentPlayer.Gold));
            EventManager.Publish(new PlayerStatsChangedEvent(_currentPlayer, StatType.SkillPoints, _currentPlayer.SkillPoints, _currentPlayer.SkillPoints));
        }
    }
} 