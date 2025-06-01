namespace WinFormsApp1.Interfaces
{
    /// <summary>
    /// Interface for player management operations
    /// </summary>
    public interface IPlayerManager : IBaseManager
    {
        /// <summary>
        /// Current player instance
        /// </summary>
        Player CurrentPlayer { get; }

        /// <summary>
        /// Set the current player
        /// </summary>
        /// <param name="player">Player to set as current</param>
        void SetCurrentPlayer(Player player);

        /// <summary>
        /// Update player health
        /// </summary>
        /// <param name="newHealth">New health value</param>
        /// <param name="reason">Reason for health change</param>
        void UpdateHealth(int newHealth, string reason = "");

        /// <summary>
        /// Add experience to the player
        /// </summary>
        /// <param name="experience">Amount of experience to add</param>
        /// <param name="source">Source of the experience</param>
        void AddExperience(int experience, string source = "");

        /// <summary>
        /// Add or remove gold from the player
        /// </summary>
        /// <param name="amount">Amount to add (positive) or remove (negative)</param>
        /// <param name="reason">Reason for gold change</param>
        void ModifyGold(int amount, string reason = "");

        /// <summary>
        /// Level up the player
        /// </summary>
        void LevelUp();

        /// <summary>
        /// Check if player can level up
        /// </summary>
        /// <returns>True if player has enough experience to level up</returns>
        bool CanLevelUp();

        /// <summary>
        /// Heal the player by a specific amount
        /// </summary>
        /// <param name="amount">Amount to heal</param>
        /// <param name="source">Source of healing</param>
        void Heal(int amount, string source = "");

        /// <summary>
        /// Deal damage to the player
        /// </summary>
        /// <param name="damage">Amount of damage to deal</param>
        /// <param name="source">Source of damage</param>
        void TakeDamage(int damage, string source = "");

        /// <summary>
        /// Check if player is dead
        /// </summary>
        /// <returns>True if player health is 0 or below</returns>
        bool IsDead();

        /// <summary>
        /// Revive the player with specified health
        /// </summary>
        /// <param name="health">Health to revive with</param>
        void Revive(int health);

        /// <summary>
        /// Add skill points to the player
        /// </summary>
        /// <param name="points">Number of skill points to add</param>
        void AddSkillPoints(int points);

        /// <summary>
        /// Spend skill points
        /// </summary>
        /// <param name="points">Number of skill points to spend</param>
        /// <returns>True if player had enough skill points</returns>
        bool SpendSkillPoints(int points);
    }
} 