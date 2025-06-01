using System.Collections.Generic;
using WinFormsApp1.Events;

namespace WinFormsApp1.Interfaces
{
    /// <summary>
    /// Interface for combat system management
    /// </summary>
    public interface ICombatManager : IBaseManager
    {
        /// <summary>
        /// Whether combat is currently active
        /// </summary>
        bool IsInCombat { get; }

        /// <summary>
        /// Current enemy being fought
        /// </summary>
        Enemy CurrentEnemy { get; }

        /// <summary>
        /// Current combat turn number
        /// </summary>
        int CurrentTurn { get; }

        /// <summary>
        /// Whether it's the player's turn
        /// </summary>
        bool IsPlayerTurn { get; }

        /// <summary>
        /// Start combat with an enemy
        /// </summary>
        /// <param name="player">Player participating in combat</param>
        /// <param name="enemy">Enemy to fight</param>
        /// <param name="location">Location where combat takes place</param>
        /// <param name="isRandomEncounter">Whether this is a random encounter</param>
        void StartCombat(Player player, Enemy enemy, Location location, bool isRandomEncounter = false);

        /// <summary>
        /// End the current combat
        /// </summary>
        /// <param name="result">Result of the combat</param>
        void EndCombat(CombatResult result);

        /// <summary>
        /// Player attacks the enemy
        /// </summary>
        /// <param name="player">Player performing the attack</param>
        /// <returns>Attack result with damage and effects</returns>
        AttackResult PlayerAttack(Player player);

        /// <summary>
        /// Enemy attacks the player
        /// </summary>
        /// <param name="enemy">Enemy performing the attack</param>
        /// <param name="player">Player being attacked</param>
        /// <returns>Attack result with damage and effects</returns>
        AttackResult EnemyAttack(Enemy enemy, Player player);

        /// <summary>
        /// Player defends, reducing incoming damage
        /// </summary>
        /// <param name="player">Player defending</param>
        /// <returns>Defense result with bonus and effects</returns>
        DefenseResult PlayerDefend(Player player);

        /// <summary>
        /// Player attempts to flee from combat
        /// </summary>
        /// <param name="player">Player attempting to flee</param>
        /// <returns>True if flee was successful</returns>
        bool PlayerFlee(Player player);

        /// <summary>
        /// Player uses an item in combat
        /// </summary>
        /// <param name="player">Player using the item</param>
        /// <param name="item">Item being used</param>
        /// <returns>True if item was used successfully</returns>
        bool UseItem(Player player, Item item);

        /// <summary>
        /// Calculate damage for an attack
        /// </summary>
        /// <param name="attacker">Entity performing the attack</param>
        /// <param name="defender">Entity being attacked</param>
        /// <param name="baseAttack">Base attack value</param>
        /// <returns>Final damage amount</returns>
        int CalculateDamage(object attacker, object defender, int baseAttack);

        /// <summary>
        /// Check if an attack is a critical hit
        /// </summary>
        /// <param name="attacker">Entity performing the attack</param>
        /// <returns>True if critical hit</returns>
        bool IsCriticalHit(object attacker);

        /// <summary>
        /// Check if an attack misses
        /// </summary>
        /// <param name="attacker">Entity performing the attack</param>
        /// <param name="defender">Entity being attacked</param>
        /// <returns>True if attack misses</returns>
        bool IsAttackMissed(object attacker, object defender);

        /// <summary>
        /// Calculate experience reward for defeating an enemy
        /// </summary>
        /// <param name="enemy">Defeated enemy</param>
        /// <param name="player">Player who defeated the enemy</param>
        /// <returns>Experience points to award</returns>
        int CalculateExperienceReward(Enemy enemy, Player player);

        /// <summary>
        /// Calculate gold reward for defeating an enemy
        /// </summary>
        /// <param name="enemy">Defeated enemy</param>
        /// <returns>Gold amount to award</returns>
        int CalculateGoldReward(Enemy enemy);

        /// <summary>
        /// Generate loot drops from a defeated enemy
        /// </summary>
        /// <param name="enemy">Defeated enemy</param>
        /// <returns>List of items dropped</returns>
        List<Item> GenerateLootDrops(Enemy enemy);

        /// <summary>
        /// Process the end of a combat turn
        /// </summary>
        void ProcessTurnEnd();

        /// <summary>
        /// Get available combat actions for the current state
        /// </summary>
        /// <returns>List of available actions</returns>
        List<string> GetAvailableCombatActions();
    }

    /// <summary>
    /// Result of an attack action
    /// </summary>
    public class AttackResult
    {
        public int Damage { get; set; }
        public bool IsCriticalHit { get; set; }
        public bool WasMissed { get; set; }
        public bool WasBlocked { get; set; }
        public string Description { get; set; } = "";
        public Dictionary<string, object> Effects { get; set; } = new Dictionary<string, object>();

        public AttackResult(int damage, bool isCriticalHit = false, bool wasMissed = false, bool wasBlocked = false)
        {
            Damage = damage;
            IsCriticalHit = isCriticalHit;
            WasMissed = wasMissed;
            WasBlocked = wasBlocked;
        }
    }

    /// <summary>
    /// Result of a defense action
    /// </summary>
    public class DefenseResult
    {
        public int DefenseBonus { get; set; }
        public bool IsSuccessful { get; set; }
        public string Description { get; set; } = "";
        public Dictionary<string, object> Effects { get; set; } = new Dictionary<string, object>();

        public DefenseResult(int defenseBonus, bool isSuccessful = true)
        {
            DefenseBonus = defenseBonus;
            IsSuccessful = isSuccessful;
        }
    }
} 