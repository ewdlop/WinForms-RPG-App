namespace WinFormsApp1.Events
{
    /// <summary>
    /// Event published when combat starts
    /// </summary>
    public class CombatStartedEvent : GameEvent
    {
        public Player Player { get; set; }
        public Enemy Enemy { get; set; }
        public Location Location { get; set; }
        public bool IsRandomEncounter { get; set; }

        public override int Priority => 8; // High priority

        public CombatStartedEvent(Player player, Enemy enemy, Location location, bool isRandomEncounter = false)
        {
            Player = player;
            Enemy = enemy;
            Location = location;
            IsRandomEncounter = isRandomEncounter;
            Source = "CombatManager";
        }
    }

    /// <summary>
    /// Event published when combat ends
    /// </summary>
    public class CombatEndedEvent : GameEvent
    {
        public Player Player { get; set; }
        public Enemy Enemy { get; set; }
        public CombatResult Result { get; set; }
        public int ExperienceGained { get; set; }
        public int GoldGained { get; set; }
        public List<Item> LootGained { get; set; }

        public override int Priority => 8; // High priority

        public CombatEndedEvent(Player player, Enemy enemy, CombatResult result, int experienceGained = 0, int goldGained = 0, List<Item> lootGained = null)
        {
            Player = player;
            Enemy = enemy;
            Result = result;
            ExperienceGained = experienceGained;
            GoldGained = goldGained;
            LootGained = lootGained ?? new List<Item>();
            Source = "CombatManager";
        }
    }

    /// <summary>
    /// Event published when player attacks
    /// </summary>
    public class PlayerAttackedEvent : GameEvent
    {
        public Player Player { get; set; }
        public Enemy Target { get; set; }
        public int Damage { get; set; }
        public bool IsCriticalHit { get; set; }
        public bool WasMissed { get; set; }
        public CombatAction Action { get; set; }

        public PlayerAttackedEvent(Player player, Enemy target, int damage, bool isCriticalHit = false, bool wasMissed = false, CombatAction action = CombatAction.Attack)
        {
            Player = player;
            Target = target;
            Damage = damage;
            IsCriticalHit = isCriticalHit;
            WasMissed = wasMissed;
            Action = action;
            Source = "CombatManager";
        }
    }

    /// <summary>
    /// Event published when enemy attacks player
    /// </summary>
    public class EnemyAttackedEvent : GameEvent
    {
        public Enemy Enemy { get; set; }
        public Player Target { get; set; }
        public int Damage { get; set; }
        public bool IsCriticalHit { get; set; }
        public bool WasMissed { get; set; }
        public bool WasBlocked { get; set; }

        public EnemyAttackedEvent(Enemy enemy, Player target, int damage, bool isCriticalHit = false, bool wasMissed = false, bool wasBlocked = false)
        {
            Enemy = enemy;
            Target = target;
            Damage = damage;
            IsCriticalHit = isCriticalHit;
            WasMissed = wasMissed;
            WasBlocked = wasBlocked;
            Source = "CombatManager";
        }
    }

    /// <summary>
    /// Event published when an enemy is defeated
    /// </summary>
    public class EnemyDefeatedEvent : GameEvent
    {
        public Player Player { get; set; }
        public Enemy Enemy { get; set; }
        public int ExperienceReward { get; set; }
        public int GoldReward { get; set; }
        public List<Item> LootDrops { get; set; }

        public EnemyDefeatedEvent(Player player, Enemy enemy, int experienceReward, int goldReward, List<Item> lootDrops = null)
        {
            Player = player;
            Enemy = enemy;
            ExperienceReward = experienceReward;
            GoldReward = goldReward;
            LootDrops = lootDrops ?? new List<Item>();
            Source = "CombatManager";
        }
    }

    /// <summary>
    /// Event published when player defends
    /// </summary>
    public class PlayerDefendedEvent : GameEvent
    {
        public Player Player { get; set; }
        public int DefenseBonus { get; set; }
        public bool IsSuccessful { get; set; }

        public PlayerDefendedEvent(Player player, int defenseBonus, bool isSuccessful = true)
        {
            Player = player;
            DefenseBonus = defenseBonus;
            IsSuccessful = isSuccessful;
            Source = "CombatManager";
        }
    }

    /// <summary>
    /// Event published when player attempts to flee
    /// </summary>
    public class PlayerFleeAttemptEvent : GameEvent
    {
        public Player Player { get; set; }
        public Enemy Enemy { get; set; }
        public bool WasSuccessful { get; set; }
        public double FleeChance { get; set; }

        public override bool CanBeCancelled => true;

        public PlayerFleeAttemptEvent(Player player, Enemy enemy, bool wasSuccessful, double fleeChance)
        {
            Player = player;
            Enemy = enemy;
            WasSuccessful = wasSuccessful;
            FleeChance = fleeChance;
            Source = "CombatManager";
        }
    }

    /// <summary>
    /// Combat result enumeration
    /// </summary>
    public enum CombatResult
    {
        Victory,
        Defeat,
        Fled,
        Draw
    }
} 