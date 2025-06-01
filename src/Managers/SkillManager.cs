using System;
using System.Collections.Generic;
using System.Linq;
using WinFormsApp1.Events;
using WinFormsApp1.Interfaces;

namespace WinFormsApp1.Managers
{
    /// <summary>
    /// Manages skill trees and skill learning for players
    /// </summary>
    public class SkillManager : BaseManager, ISkillManager
    {
        private readonly IPlayerManager _playerManager;
        private Dictionary<string, Skill> _allSkills;
        private Player _currentPlayer;
        private readonly Dictionary<string, DateTime> _skillCooldowns;
        private readonly Dictionary<string, int> _appliedPassiveBonuses;

        public override string ManagerName => "SkillManager";
        public Player CurrentPlayer => _currentPlayer;
        public Dictionary<string, Skill> AllSkills => new Dictionary<string, Skill>(_allSkills);

        public SkillManager(IEventManager eventManager, IPlayerManager playerManager) : base(eventManager)
        {
            _playerManager = playerManager ?? throw new ArgumentNullException(nameof(playerManager));
            _allSkills = new Dictionary<string, Skill>();
            _skillCooldowns = new Dictionary<string, DateTime>();
            _appliedPassiveBonuses = new Dictionary<string, int>();
        }

        protected override void SubscribeToEvents()
        {
            // Subscribe to game events to track player
            EventManager.Subscribe<GameStartedEvent>(OnGameStarted);
            EventManager.Subscribe<PlayerLeveledUpEvent>(OnPlayerLevelUp);
        }

        protected override void UnsubscribeFromEvents()
        {
            EventManager.ClearSubscriptions<GameStartedEvent>();
            EventManager.ClearSubscriptions<PlayerLeveledUpEvent>();
        }

        public void SetCurrentPlayer(Player player)
        {
            if (player == null)
                throw new ArgumentNullException(nameof(player));

            // Remove passive effects from previous player
            if (_currentPlayer != null)
            {
                RemovePassiveSkillEffects();
            }

            _currentPlayer = player;
            
            // Apply passive effects to new player
            ApplyPassiveSkillEffects();
            
            LogMessage($"Current player set to: {player.Name}");
        }

        public bool LoadSkills()
        {
            try
            {
                LogMessage("Loading skills from data");

                // Create default skills since we don't have a data file yet
                CreateDefaultSkills();

                var skillsByClass = new Dictionary<CharacterClass, int>();
                foreach (var characterClass in Enum.GetValues<CharacterClass>())
                {
                    skillsByClass[characterClass] = _allSkills.Values.Count(s => s.RequiredClass == characterClass);
                }

                var skillIds = _allSkills.Keys.ToList();
                EventManager.Publish(new SkillsLoadedEvent(_allSkills.Count, true, "", skillIds, skillsByClass));
                LogMessage($"Successfully loaded {_allSkills.Count} skills");
                return true;
            }
            catch (Exception ex)
            {
                LogError("Failed to load skills", ex);
                EventManager.Publish(new SkillsLoadedEvent(0, false, ex.Message));
                return false;
            }
        }

        public bool LearnSkill(string skillId)
        {
            if (_currentPlayer == null)
            {
                LogError("Cannot learn skill: No current player set");
                return false;
            }

            if (!SkillExists(skillId))
            {
                var failureReason = $"Skill '{skillId}' does not exist";
                EventManager.Publish(new SkillLearnedEvent(_currentPlayer, null, 0, GetAvailableSkillPoints(), false, failureReason));
                return false;
            }

            var skill = _allSkills[skillId];

            if (!CanLearnSkill(skillId))
            {
                var failureReason = GetSkillLearnFailureReason(skillId);
                EventManager.Publish(new SkillLearnedEvent(_currentPlayer, skill, 0, GetAvailableSkillPoints(), false, failureReason));
                return false;
            }

            // Spend skill points
            _currentPlayer.SkillPoints -= skill.SkillPointCost;
            
            // Add skill to learned skills
            if (!_currentPlayer.LearnedSkills.Contains(skillId))
            {
                _currentPlayer.LearnedSkills.Add(skillId);
            }

            // Mark skill as learned
            skill.IsLearned = true;

            // Apply passive effects if it's a passive skill
            if (skill.Type == SkillType.Passive)
            {
                ApplySkillPassiveEffects(skill);
            }

            EventManager.Publish(new SkillLearnedEvent(_currentPlayer, skill, skill.SkillPointCost, GetAvailableSkillPoints(), true));
            PublishSkillTreeUpdate();
            
            LogMessage($"Player {_currentPlayer.Name} learned skill: {skill.Name}");
            return true;
        }

        public bool CanLearnSkill(string skillId)
        {
            if (_currentPlayer == null || !SkillExists(skillId))
                return false;

            var skill = _allSkills[skillId];

            // Check if already learned
            if (HasSkill(skillId))
                return false;

            // Check class requirement
            if (skill.RequiredClass != _currentPlayer.CharacterClass)
                return false;

            // Check level requirement
            if (_currentPlayer.Level < skill.RequiredLevel)
                return false;

            // Check skill points
            if (GetAvailableSkillPoints() < skill.SkillPointCost)
                return false;

            // Check prerequisites
            foreach (var prerequisite in skill.Prerequisites)
            {
                if (!HasSkill(prerequisite))
                    return false;
            }

            return true;
        }

        public string GetSkillLearnFailureReason(string skillId)
        {
            if (_currentPlayer == null)
                return "No player set";

            if (!SkillExists(skillId))
                return "Skill does not exist";

            var skill = _allSkills[skillId];

            if (HasSkill(skillId))
                return "Skill already learned";

            if (skill.RequiredClass != _currentPlayer.CharacterClass)
                return $"Requires {skill.RequiredClass} class";

            if (_currentPlayer.Level < skill.RequiredLevel)
                return $"Requires level {skill.RequiredLevel}";

            if (GetAvailableSkillPoints() < skill.SkillPointCost)
                return $"Requires {skill.SkillPointCost} skill points";

            var missingPrerequisites = skill.Prerequisites.Where(p => !HasSkill(p)).ToList();
            if (missingPrerequisites.Count > 0)
            {
                var prerequisiteNames = missingPrerequisites.Select(p => _allSkills.ContainsKey(p) ? _allSkills[p].Name : p);
                return $"Missing prerequisites: {string.Join(", ", prerequisiteNames)}";
            }

            return "";
        }

        public bool HasSkill(string skillId)
        {
            return _currentPlayer?.LearnedSkills.Contains(skillId) ?? false;
        }

        public List<string> GetLearnedSkills()
        {
            return _currentPlayer?.LearnedSkills ?? new List<string>();
        }

        public List<string> GetAvailableSkills()
        {
            if (_currentPlayer == null)
                return new List<string>();

            return _allSkills.Values
                .Where(s => s.RequiredClass == _currentPlayer.CharacterClass)
                .Select(s => s.Id)
                .ToList();
        }

        public List<string> GetLearnableSkills()
        {
            return GetAvailableSkills().Where(CanLearnSkill).ToList();
        }

        public Skill GetSkill(string skillId)
        {
            return SkillExists(skillId) ? _allSkills[skillId] : null;
        }

        public bool AddSkill(Skill skill)
        {
            if (skill == null || string.IsNullOrWhiteSpace(skill.Id))
            {
                EventManager.Publish(new SkillAddedEvent(skill, false, "Invalid skill or missing ID"));
                return false;
            }

            if (_allSkills.ContainsKey(skill.Id))
            {
                EventManager.Publish(new SkillAddedEvent(skill, false, "Skill ID already exists"));
                return false;
            }

            _allSkills[skill.Id] = skill;
            EventManager.Publish(new SkillAddedEvent(skill, true));
            LogMessage($"Added new skill: {skill.Name} ({skill.Id})");
            return true;
        }

        public bool RemoveSkill(string skillId)
        {
            if (!SkillExists(skillId))
            {
                EventManager.Publish(new SkillRemovedEvent(skillId, null, false, "Skill does not exist"));
                return false;
            }

            var skill = _allSkills[skillId];
            var wasLearnedByPlayer = HasSkill(skillId);

            // Remove from player's learned skills if they had it
            if (wasLearnedByPlayer && _currentPlayer != null)
            {
                _currentPlayer.LearnedSkills.Remove(skillId);
                
                // Remove passive effects if it was a passive skill
                if (skill.Type == SkillType.Passive)
                {
                    RemoveSkillPassiveEffects(skill);
                }
            }

            _allSkills.Remove(skillId);
            EventManager.Publish(new SkillRemovedEvent(skillId, skill, true, "", wasLearnedByPlayer));
            LogMessage($"Removed skill: {skill.Name} ({skillId})");
            return true;
        }

        public bool UseSkill(string skillId, object target = null)
        {
            if (_currentPlayer == null)
            {
                LogError("Cannot use skill: No current player set");
                return false;
            }

            if (!SkillExists(skillId))
            {
                EventManager.Publish(new SkillUsedEvent(_currentPlayer, null, target, false, "Skill does not exist"));
                return false;
            }

            var skill = _allSkills[skillId];

            if (!HasSkill(skillId))
            {
                EventManager.Publish(new SkillUsedEvent(_currentPlayer, skill, target, false, "Skill not learned"));
                return false;
            }

            if (skill.Type != SkillType.Active)
            {
                EventManager.Publish(new SkillUsedEvent(_currentPlayer, skill, target, false, "Skill is not active"));
                return false;
            }

            if (IsSkillOnCooldown(skillId))
            {
                var remainingCooldown = GetSkillCooldownRemaining(skillId);
                EventManager.Publish(new SkillUsedEvent(_currentPlayer, skill, target, false, $"Skill on cooldown ({remainingCooldown:F1}s remaining)"));
                return false;
            }

            // TODO: Check mana cost when mana system is implemented
            // if (_currentPlayer.Mana < skill.ManaCost)
            // {
            //     EventManager.Publish(new SkillUsedEvent(_currentPlayer, skill, target, false, "Not enough mana"));
            //     return false;
            // }

            // Use the skill
            var skillEffects = ExecuteSkillEffect(skill, target);
            
            // Set cooldown
            if (skill.Cooldown > 0)
            {
                _skillCooldowns[skillId] = DateTime.UtcNow.AddSeconds(skill.Cooldown);
            }

            EventManager.Publish(new SkillUsedEvent(_currentPlayer, skill, target, true, "", skillEffects, skill.ManaCost, skill.Cooldown));
            LogMessage($"Player {_currentPlayer.Name} used skill: {skill.Name}");
            return true;
        }

        public bool IsSkillOnCooldown(string skillId)
        {
            if (!_skillCooldowns.ContainsKey(skillId))
                return false;

            return DateTime.UtcNow < _skillCooldowns[skillId];
        }

        public double GetSkillCooldownRemaining(string skillId)
        {
            if (!IsSkillOnCooldown(skillId))
                return 0.0;

            var timeRemaining = _skillCooldowns[skillId] - DateTime.UtcNow;
            return Math.Max(0.0, timeRemaining.TotalSeconds);
        }

        public List<string> GetSkillsByCategory(SkillCategory category)
        {
            return _allSkills.Values
                .Where(s => s.Category == category)
                .Select(s => s.Id)
                .ToList();
        }

        public List<string> GetSkillsByClass(CharacterClass characterClass)
        {
            return _allSkills.Values
                .Where(s => s.RequiredClass == characterClass)
                .Select(s => s.Id)
                .ToList();
        }

        public Dictionary<int, List<string>> GetSkillTree(CharacterClass characterClass)
        {
            var skillTree = new Dictionary<int, List<string>>();
            
            var classSkills = _allSkills.Values
                .Where(s => s.RequiredClass == characterClass)
                .GroupBy(s => s.Tier)
                .OrderBy(g => g.Key);

            foreach (var tierGroup in classSkills)
            {
                skillTree[tierGroup.Key] = tierGroup.Select(s => s.Id).ToList();
            }

            return skillTree;
        }

        public int GetTotalSkillPointsSpent()
        {
            if (_currentPlayer == null)
                return 0;

            return _currentPlayer.LearnedSkills
                .Where(SkillExists)
                .Sum(skillId => _allSkills[skillId].SkillPointCost);
        }

        public int GetAvailableSkillPoints()
        {
            return _currentPlayer?.SkillPoints ?? 0;
        }

        public bool ResetSkills(double refundPercentage = 1.0)
        {
            if (_currentPlayer == null)
            {
                EventManager.Publish(new SkillsResetEvent(null, new List<string>(), 0, refundPercentage, false, "No current player"));
                return false;
            }

            var resetSkills = new List<string>(_currentPlayer.LearnedSkills);
            var totalSpent = GetTotalSkillPointsSpent();
            var refundAmount = (int)(totalSpent * Math.Max(0.0, Math.Min(1.0, refundPercentage)));

            // Remove passive effects
            RemovePassiveSkillEffects();

            // Clear learned skills
            _currentPlayer.LearnedSkills.Clear();

            // Mark all skills as not learned
            foreach (var skill in _allSkills.Values)
            {
                skill.IsLearned = false;
            }

            // Refund skill points
            _currentPlayer.SkillPoints += refundAmount;

            // Clear cooldowns
            _skillCooldowns.Clear();

            EventManager.Publish(new SkillsResetEvent(_currentPlayer, resetSkills, refundAmount, refundPercentage, true));
            PublishSkillTreeUpdate();
            
            LogMessage($"Reset skills for {_currentPlayer.Name}, refunded {refundAmount} skill points");
            return true;
        }

        public string GetSkillDescription(string skillId)
        {
            var skill = GetSkill(skillId);
            if (skill == null)
                return "Skill not found";

            var description = $"**{skill.Name}**\n{skill.Description}\n\n";
            description += $"Type: {skill.Type}\n";
            description += $"Category: {skill.Category}\n";
            description += $"Cost: {skill.SkillPointCost} skill points\n";

            if (skill.RequiredLevel > 1)
                description += $"Required Level: {skill.RequiredLevel}\n";

            if (skill.Prerequisites.Count > 0)
            {
                var prereqNames = skill.Prerequisites.Select(p => _allSkills.ContainsKey(p) ? _allSkills[p].Name : p);
                description += $"Prerequisites: {string.Join(", ", prereqNames)}\n";
            }

            if (skill.Type == SkillType.Active)
            {
                if (skill.Cooldown > 0)
                    description += $"Cooldown: {skill.Cooldown} seconds\n";
                if (skill.ManaCost > 0)
                    description += $"Mana Cost: {skill.ManaCost}\n";
            }

            if (skill.StatBonuses.Count > 0)
            {
                description += "\nStat Bonuses:\n";
                foreach (var bonus in skill.StatBonuses)
                {
                    description += $"  {bonus.Key}: +{bonus.Value}\n";
                }
            }

            return description;
        }

        public string GetSkillTooltip(string skillId)
        {
            var skill = GetSkill(skillId);
            if (skill == null)
                return "Skill not found";

            var tooltip = $"{skill.Name}\n{skill.Description}";
            
            if (HasSkill(skillId))
            {
                tooltip += "\n[LEARNED]";
                if (skill.Type == SkillType.Active && IsSkillOnCooldown(skillId))
                {
                    var remaining = GetSkillCooldownRemaining(skillId);
                    tooltip += $"\n[COOLDOWN: {remaining:F1}s]";
                }
            }
            else if (CanLearnSkill(skillId))
            {
                tooltip += "\n[CAN LEARN]";
            }
            else
            {
                var reason = GetSkillLearnFailureReason(skillId);
                tooltip += $"\n[CANNOT LEARN: {reason}]";
            }

            return tooltip;
        }

        public bool SkillExists(string skillId)
        {
            return !string.IsNullOrWhiteSpace(skillId) && _allSkills.ContainsKey(skillId);
        }

        public Dictionary<string, int> GetPassiveSkillBonuses()
        {
            var bonuses = new Dictionary<string, int>();

            if (_currentPlayer == null)
                return bonuses;

            foreach (var skillId in _currentPlayer.LearnedSkills)
            {
                if (SkillExists(skillId))
                {
                    var skill = _allSkills[skillId];
                    if (skill.Type == SkillType.Passive)
                    {
                        foreach (var bonus in skill.StatBonuses)
                        {
                            bonuses[bonus.Key] = bonuses.GetValueOrDefault(bonus.Key, 0) + bonus.Value;
                        }
                    }
                }
            }

            return bonuses;
        }

        public void ApplyPassiveSkillEffects()
        {
            if (_currentPlayer == null)
                return;

            var bonuses = GetPassiveSkillBonuses();
            var affectedSkills = new List<string>();

            foreach (var skillId in _currentPlayer.LearnedSkills)
            {
                if (SkillExists(skillId) && _allSkills[skillId].Type == SkillType.Passive)
                {
                    affectedSkills.Add(skillId);
                    ApplySkillPassiveEffects(_allSkills[skillId]);
                }
            }

            if (bonuses.Count > 0)
            {
                EventManager.Publish(new PassiveSkillEffectsChangedEvent(_currentPlayer, bonuses, true, affectedSkills));
                LogMessage($"Applied passive skill effects to {_currentPlayer.Name}");
            }
        }

        public void RemovePassiveSkillEffects()
        {
            if (_currentPlayer == null)
                return;

            var bonuses = GetPassiveSkillBonuses();
            var affectedSkills = new List<string>();

            foreach (var skillId in _currentPlayer.LearnedSkills)
            {
                if (SkillExists(skillId) && _allSkills[skillId].Type == SkillType.Passive)
                {
                    affectedSkills.Add(skillId);
                    RemoveSkillPassiveEffects(_allSkills[skillId]);
                }
            }

            if (bonuses.Count > 0)
            {
                EventManager.Publish(new PassiveSkillEffectsChangedEvent(_currentPlayer, bonuses, false, affectedSkills));
                LogMessage($"Removed passive skill effects from {_currentPlayer.Name}");
            }
        }

        public string GetSkillPrerequisitesText(string skillId)
        {
            var skill = GetSkill(skillId);
            if (skill == null || skill.Prerequisites.Count == 0)
                return "None";

            var prereqNames = skill.Prerequisites.Select(p => 
            {
                if (_allSkills.ContainsKey(p))
                {
                    var prereqSkill = _allSkills[p];
                    var status = HasSkill(p) ? "✓" : "✗";
                    return $"{status} {prereqSkill.Name}";
                }
                return p;
            });

            return string.Join(", ", prereqNames);
        }

        private void CreateDefaultSkills()
        {
            // Warrior Skills
            var warriorSkills = new[]
            {
                new Skill
                {
                    Id = "warrior_power_strike",
                    Name = "Power Strike",
                    Description = "A devastating attack that deals extra damage.",
                    Category = SkillCategory.Combat,
                    Type = SkillType.Active,
                    RequiredClass = CharacterClass.Warrior,
                    RequiredLevel = 1,
                    SkillPointCost = 1,
                    Tier = 1,
                    Cooldown = 5,
                    ManaCost = 0
                },
                new Skill
                {
                    Id = "warrior_defense_boost",
                    Name = "Defense Mastery",
                    Description = "Permanently increases defense.",
                    Category = SkillCategory.Passive,
                    Type = SkillType.Passive,
                    RequiredClass = CharacterClass.Warrior,
                    RequiredLevel = 2,
                    SkillPointCost = 1,
                    Tier = 1,
                    StatBonuses = new Dictionary<string, int> { { "Defense", 3 } }
                },
                new Skill
                {
                    Id = "warrior_berserker_rage",
                    Name = "Berserker Rage",
                    Description = "Temporarily increases attack but reduces defense.",
                    Category = SkillCategory.Ultimate,
                    Type = SkillType.Active,
                    RequiredClass = CharacterClass.Warrior,
                    RequiredLevel = 5,
                    SkillPointCost = 3,
                    Tier = 2,
                    Prerequisites = new List<string> { "warrior_power_strike" },
                    Cooldown = 30,
                    ManaCost = 0
                }
            };

            // Mage Skills
            var mageSkills = new[]
            {
                new Skill
                {
                    Id = "mage_fireball",
                    Name = "Fireball",
                    Description = "Launches a fireball at the enemy.",
                    Category = SkillCategory.Magic,
                    Type = SkillType.Active,
                    RequiredClass = CharacterClass.Mage,
                    RequiredLevel = 1,
                    SkillPointCost = 1,
                    Tier = 1,
                    Cooldown = 3,
                    ManaCost = 10
                },
                new Skill
                {
                    Id = "mage_mana_boost",
                    Name = "Mana Mastery",
                    Description = "Permanently increases maximum mana.",
                    Category = SkillCategory.Passive,
                    Type = SkillType.Passive,
                    RequiredClass = CharacterClass.Mage,
                    RequiredLevel = 2,
                    SkillPointCost = 1,
                    Tier = 1,
                    StatBonuses = new Dictionary<string, int> { { "MaxMana", 20 } }
                },
                new Skill
                {
                    Id = "mage_meteor",
                    Name = "Meteor",
                    Description = "Calls down a devastating meteor strike.",
                    Category = SkillCategory.Ultimate,
                    Type = SkillType.Active,
                    RequiredClass = CharacterClass.Mage,
                    RequiredLevel = 6,
                    SkillPointCost = 3,
                    Tier = 2,
                    Prerequisites = new List<string> { "mage_fireball" },
                    Cooldown = 45,
                    ManaCost = 50
                }
            };

            // Rogue Skills
            var rogueSkills = new[]
            {
                new Skill
                {
                    Id = "rogue_stealth",
                    Name = "Stealth",
                    Description = "Become invisible for a short time.",
                    Category = SkillCategory.Utility,
                    Type = SkillType.Active,
                    RequiredClass = CharacterClass.Rogue,
                    RequiredLevel = 1,
                    SkillPointCost = 1,
                    Tier = 1,
                    Cooldown = 15,
                    ManaCost = 0
                },
                new Skill
                {
                    Id = "rogue_agility",
                    Name = "Agility Training",
                    Description = "Permanently increases attack speed.",
                    Category = SkillCategory.Passive,
                    Type = SkillType.Passive,
                    RequiredClass = CharacterClass.Rogue,
                    RequiredLevel = 2,
                    SkillPointCost = 1,
                    Tier = 1,
                    StatBonuses = new Dictionary<string, int> { { "Attack", 2 } }
                },
                new Skill
                {
                    Id = "rogue_assassinate",
                    Name = "Assassinate",
                    Description = "A critical strike with high damage potential.",
                    Category = SkillCategory.Ultimate,
                    Type = SkillType.Active,
                    RequiredClass = CharacterClass.Rogue,
                    RequiredLevel = 5,
                    SkillPointCost = 3,
                    Tier = 2,
                    Prerequisites = new List<string> { "rogue_stealth" },
                    Cooldown = 20,
                    ManaCost = 0
                }
            };

            // Cleric Skills
            var clericSkills = new[]
            {
                new Skill
                {
                    Id = "cleric_heal",
                    Name = "Heal",
                    Description = "Restores health to self or ally.",
                    Category = SkillCategory.Magic,
                    Type = SkillType.Active,
                    RequiredClass = CharacterClass.Cleric,
                    RequiredLevel = 1,
                    SkillPointCost = 1,
                    Tier = 1,
                    Cooldown = 2,
                    ManaCost = 15
                },
                new Skill
                {
                    Id = "cleric_blessing",
                    Name = "Divine Blessing",
                    Description = "Permanently increases maximum health.",
                    Category = SkillCategory.Passive,
                    Type = SkillType.Passive,
                    RequiredClass = CharacterClass.Cleric,
                    RequiredLevel = 2,
                    SkillPointCost = 1,
                    Tier = 1,
                    StatBonuses = new Dictionary<string, int> { { "MaxHealth", 15 } }
                },
                new Skill
                {
                    Id = "cleric_resurrection",
                    Name = "Resurrection",
                    Description = "Revive from death with partial health.",
                    Category = SkillCategory.Ultimate,
                    Type = SkillType.Passive,
                    RequiredClass = CharacterClass.Cleric,
                    RequiredLevel = 7,
                    SkillPointCost = 4,
                    Tier = 2,
                    Prerequisites = new List<string> { "cleric_heal", "cleric_blessing" }
                }
            };

            // Add all skills to the dictionary
            foreach (var skill in warriorSkills.Concat(mageSkills).Concat(rogueSkills).Concat(clericSkills))
            {
                _allSkills[skill.Id] = skill;
            }

            LogMessage($"Created {_allSkills.Count} default skills");
        }

        private Dictionary<string, object> ExecuteSkillEffect(Skill skill, object target)
        {
            var effects = new Dictionary<string, object>();

            // Simple skill effect implementation
            switch (skill.Id)
            {
                case "warrior_power_strike":
                    effects["damage_multiplier"] = 1.5;
                    effects["description"] = "Deals 150% weapon damage";
                    break;

                case "warrior_berserker_rage":
                    effects["attack_bonus"] = 10;
                    effects["defense_penalty"] = -5;
                    effects["duration"] = 10;
                    effects["description"] = "Gain +10 attack, -5 defense for 10 seconds";
                    break;

                case "mage_fireball":
                    effects["magic_damage"] = 25;
                    effects["description"] = "Deals 25 fire damage";
                    break;

                case "mage_meteor":
                    effects["magic_damage"] = 80;
                    effects["area_effect"] = true;
                    effects["description"] = "Deals 80 fire damage to all enemies";
                    break;

                case "rogue_stealth":
                    effects["invisible"] = true;
                    effects["duration"] = 5;
                    effects["description"] = "Become invisible for 5 seconds";
                    break;

                case "rogue_assassinate":
                    effects["critical_chance"] = 0.8;
                    effects["damage_multiplier"] = 3.0;
                    effects["description"] = "80% chance for 300% critical damage";
                    break;

                case "cleric_heal":
                    var healAmount = 30 + (_currentPlayer?.Level ?? 1) * 5;
                    effects["heal_amount"] = healAmount;
                    effects["description"] = $"Restores {healAmount} health";
                    
                    // Actually heal the player
                    if (_currentPlayer != null)
                    {
                        _playerManager.Heal(healAmount, "Heal skill");
                    }
                    break;

                default:
                    effects["description"] = "Skill effect not implemented";
                    break;
            }

            return effects;
        }

        private void ApplySkillPassiveEffects(Skill skill)
        {
            if (_currentPlayer == null || skill.Type != SkillType.Passive)
                return;

            foreach (var bonus in skill.StatBonuses)
            {
                var currentBonus = _appliedPassiveBonuses.GetValueOrDefault(bonus.Key, 0);
                _appliedPassiveBonuses[bonus.Key] = currentBonus + bonus.Value;

                // Apply to player stats
                switch (bonus.Key.ToLowerInvariant())
                {
                    case "attack":
                        _currentPlayer.Attack += bonus.Value;
                        break;
                    case "defense":
                        _currentPlayer.Defense += bonus.Value;
                        break;
                    case "maxhealth":
                        _currentPlayer.MaxHealth += bonus.Value;
                        _currentPlayer.Health = Math.Min(_currentPlayer.Health + bonus.Value, _currentPlayer.MaxHealth);
                        break;
                }
            }
        }

        private void RemoveSkillPassiveEffects(Skill skill)
        {
            if (_currentPlayer == null || skill.Type != SkillType.Passive)
                return;

            foreach (var bonus in skill.StatBonuses)
            {
                var currentBonus = _appliedPassiveBonuses.GetValueOrDefault(bonus.Key, 0);
                _appliedPassiveBonuses[bonus.Key] = Math.Max(0, currentBonus - bonus.Value);

                // Remove from player stats
                switch (bonus.Key.ToLowerInvariant())
                {
                    case "attack":
                        _currentPlayer.Attack = Math.Max(1, _currentPlayer.Attack - bonus.Value);
                        break;
                    case "defense":
                        _currentPlayer.Defense = Math.Max(0, _currentPlayer.Defense - bonus.Value);
                        break;
                    case "maxhealth":
                        _currentPlayer.MaxHealth = Math.Max(1, _currentPlayer.MaxHealth - bonus.Value);
                        _currentPlayer.Health = Math.Min(_currentPlayer.Health, _currentPlayer.MaxHealth);
                        break;
                }
            }
        }

        private void PublishSkillTreeUpdate()
        {
            if (_currentPlayer == null)
                return;

            var skillTree = GetSkillTree(_currentPlayer.CharacterClass);
            var learnedSkills = GetLearnedSkills();
            var availableSkills = GetAvailableSkills();
            var availableSkillPoints = GetAvailableSkillPoints();

            EventManager.Publish(new SkillTreeUpdatedEvent(_currentPlayer, _currentPlayer.CharacterClass, skillTree, learnedSkills, availableSkills, availableSkillPoints));
        }

        private void OnGameStarted(GameStartedEvent e)
        {
            SetCurrentPlayer(e.Player);
            
            // Load skills if not already loaded
            if (_allSkills.Count == 0)
            {
                LoadSkills();
            }

            PublishSkillTreeUpdate();
        }

        private void OnPlayerLevelUp(PlayerLeveledUpEvent e)
        {
            // Player gains skill points on level up
            if (_currentPlayer != null && e.Player == _currentPlayer)
            {
                _currentPlayer.SkillPoints += 2; // 2 skill points per level
                PublishSkillTreeUpdate();
                LogMessage($"Player gained 2 skill points from leveling up (Level {e.NewLevel})");
            }
        }
    }
} 