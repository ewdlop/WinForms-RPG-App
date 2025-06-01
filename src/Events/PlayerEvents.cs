using System;

namespace WinFormsApp1.Events
{
    /// <summary>
    /// Event published when player stats change
    /// </summary>
    public class PlayerStatsChangedEvent : GameEvent
    {
        public Player Player { get; set; }
        public StatType StatType { get; set; }
        public int OldValue { get; set; }
        public int NewValue { get; set; }
        public int Delta => NewValue - OldValue;

        public PlayerStatsChangedEvent(Player player, StatType statType, int oldValue, int newValue)
        {
            Player = player;
            StatType = statType;
            OldValue = oldValue;
            NewValue = newValue;
            Source = "PlayerManager";
        }
    }

    /// <summary>
    /// Event published when player levels up
    /// </summary>
    public class PlayerLeveledUpEvent : GameEvent
    {
        public Player Player { get; set; }
        public int OldLevel { get; set; }
        public int NewLevel { get; set; }
        public int SkillPointsGained { get; set; }

        public override int Priority => 10; // High priority for level up events

        public PlayerLeveledUpEvent(Player player, int oldLevel, int newLevel, int skillPointsGained)
        {
            Player = player;
            OldLevel = oldLevel;
            NewLevel = newLevel;
            SkillPointsGained = skillPointsGained;
            Source = "PlayerManager";
        }
    }

    /// <summary>
    /// Event published when player health changes
    /// </summary>
    public class PlayerHealthChangedEvent : GameEvent
    {
        public Player Player { get; set; }
        public int OldHealth { get; set; }
        public int NewHealth { get; set; }
        public int MaxHealth { get; set; }
        public string Reason { get; set; } // e.g., "Combat", "Potion", "Healing"

        public bool IsHealing => NewHealth > OldHealth;
        public bool IsDamage => NewHealth < OldHealth;
        public double HealthPercentage => MaxHealth > 0 ? (double)NewHealth / MaxHealth : 0;

        public PlayerHealthChangedEvent(Player player, int oldHealth, int newHealth, string reason = "")
        {
            Player = player;
            OldHealth = oldHealth;
            NewHealth = newHealth;
            MaxHealth = player.MaxHealth;
            Reason = reason;
            Source = "PlayerManager";
        }
    }

    /// <summary>
    /// Event published when player dies
    /// </summary>
    public class PlayerDiedEvent : GameEvent
    {
        public Player Player { get; set; }
        public string CauseOfDeath { get; set; }
        public Enemy KilledBy { get; set; }

        public override int Priority => 15; // Very high priority

        public PlayerDiedEvent(Player player, string causeOfDeath, Enemy killedBy = null)
        {
            Player = player;
            CauseOfDeath = causeOfDeath;
            KilledBy = killedBy;
            Source = "CombatManager";
        }
    }

    /// <summary>
    /// Event published when player gains experience
    /// </summary>
    public class PlayerExperienceGainedEvent : GameEvent
    {
        public Player Player { get; set; }
        public int ExperienceGained { get; set; }
        public int TotalExperience { get; set; }
        public string Source { get; set; }

        public PlayerExperienceGainedEvent(Player player, int experienceGained, string source = "")
        {
            Player = player;
            ExperienceGained = experienceGained;
            TotalExperience = player.Experience;
            Source = source;
        }
    }

    /// <summary>
    /// Event published when player gains or loses gold
    /// </summary>
    public class PlayerGoldChangedEvent : GameEvent
    {
        public Player Player { get; set; }
        public int OldGold { get; set; }
        public int NewGold { get; set; }
        public int Delta => NewGold - OldGold;
        public string Reason { get; set; }

        public bool IsGain => Delta > 0;
        public bool IsLoss => Delta < 0;

        public PlayerGoldChangedEvent(Player player, int oldGold, int newGold, string reason = "")
        {
            Player = player;
            OldGold = oldGold;
            NewGold = newGold;
            Reason = reason;
            Source = "GameManager";
        }
    }
} 