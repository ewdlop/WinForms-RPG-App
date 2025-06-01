using System.Collections.Generic;

namespace WinFormsApp1.Interfaces
{
    /// <summary>
    /// Interface for skill tree and skill learning management
    /// </summary>
    public interface ISkillManager : IBaseManager
    {
        /// <summary>
        /// Current player for skill tracking
        /// </summary>
        Player CurrentPlayer { get; }

        /// <summary>
        /// All available skills in the game
        /// </summary>
        Dictionary<string, Skill> AllSkills { get; }

        /// <summary>
        /// Set the current player for skill tracking
        /// </summary>
        /// <param name="player">Player to track</param>
        void SetCurrentPlayer(Player player);

        /// <summary>
        /// Load all skills from data files
        /// </summary>
        /// <returns>True if skills were loaded successfully</returns>
        bool LoadSkills();

        /// <summary>
        /// Learn a skill for the current player
        /// </summary>
        /// <param name="skillId">ID of the skill to learn</param>
        /// <returns>True if skill was learned successfully</returns>
        bool LearnSkill(string skillId);

        /// <summary>
        /// Check if player can learn a specific skill
        /// </summary>
        /// <param name="skillId">ID of the skill to check</param>
        /// <returns>True if skill can be learned</returns>
        bool CanLearnSkill(string skillId);

        /// <summary>
        /// Get the reason why a skill cannot be learned
        /// </summary>
        /// <param name="skillId">ID of the skill to check</param>
        /// <returns>Reason why skill cannot be learned, or empty if it can be learned</returns>
        string GetSkillLearnFailureReason(string skillId);

        /// <summary>
        /// Check if player has learned a specific skill
        /// </summary>
        /// <param name="skillId">ID of the skill to check</param>
        /// <returns>True if player has learned the skill</returns>
        bool HasSkill(string skillId);

        /// <summary>
        /// Get all skills the player has learned
        /// </summary>
        /// <returns>List of learned skill IDs</returns>
        List<string> GetLearnedSkills();

        /// <summary>
        /// Get all skills available for the player's class
        /// </summary>
        /// <returns>List of available skill IDs</returns>
        List<string> GetAvailableSkills();

        /// <summary>
        /// Get skills that can be learned right now (prerequisites met)
        /// </summary>
        /// <returns>List of learnable skill IDs</returns>
        List<string> GetLearnableSkills();

        /// <summary>
        /// Get a skill by its ID
        /// </summary>
        /// <param name="skillId">ID of the skill</param>
        /// <returns>Skill object or null if not found</returns>
        Skill GetSkill(string skillId);

        /// <summary>
        /// Add a new skill to the game
        /// </summary>
        /// <param name="skill">Skill to add</param>
        /// <returns>True if skill was added successfully</returns>
        bool AddSkill(Skill skill);

        /// <summary>
        /// Remove a skill from the game
        /// </summary>
        /// <param name="skillId">ID of the skill to remove</param>
        /// <returns>True if skill was removed successfully</returns>
        bool RemoveSkill(string skillId);

        /// <summary>
        /// Use a skill (if it's an active skill)
        /// </summary>
        /// <param name="skillId">ID of the skill to use</param>
        /// <param name="target">Target for the skill (optional)</param>
        /// <returns>True if skill was used successfully</returns>
        bool UseSkill(string skillId, object target = null);

        /// <summary>
        /// Check if a skill is on cooldown
        /// </summary>
        /// <param name="skillId">ID of the skill to check</param>
        /// <returns>True if skill is on cooldown</returns>
        bool IsSkillOnCooldown(string skillId);

        /// <summary>
        /// Get remaining cooldown time for a skill
        /// </summary>
        /// <param name="skillId">ID of the skill</param>
        /// <returns>Remaining cooldown in seconds, or 0 if not on cooldown</returns>
        double GetSkillCooldownRemaining(string skillId);

        /// <summary>
        /// Get skills by category
        /// </summary>
        /// <param name="category">Skill category to filter by</param>
        /// <returns>List of skill IDs in the category</returns>
        List<string> GetSkillsByCategory(SkillCategory category);

        /// <summary>
        /// Get skills by character class
        /// </summary>
        /// <param name="characterClass">Character class to filter by</param>
        /// <returns>List of skill IDs for the class</returns>
        List<string> GetSkillsByClass(CharacterClass characterClass);

        /// <summary>
        /// Get the skill tree structure for a character class
        /// </summary>
        /// <param name="characterClass">Character class</param>
        /// <returns>Dictionary of skill tiers and their skills</returns>
        Dictionary<int, List<string>> GetSkillTree(CharacterClass characterClass);

        /// <summary>
        /// Calculate total skill points spent
        /// </summary>
        /// <returns>Total skill points spent by the player</returns>
        int GetTotalSkillPointsSpent();

        /// <summary>
        /// Calculate available skill points
        /// </summary>
        /// <returns>Available skill points for the player</returns>
        int GetAvailableSkillPoints();

        /// <summary>
        /// Reset all learned skills and refund skill points
        /// </summary>
        /// <param name="refundPercentage">Percentage of skill points to refund (0.0 to 1.0)</param>
        /// <returns>True if reset was successful</returns>
        bool ResetSkills(double refundPercentage = 1.0);

        /// <summary>
        /// Get skill description with current effects
        /// </summary>
        /// <param name="skillId">ID of the skill</param>
        /// <returns>Formatted skill description</returns>
        string GetSkillDescription(string skillId);

        /// <summary>
        /// Get skill tooltip with detailed information
        /// </summary>
        /// <param name="skillId">ID of the skill</param>
        /// <returns>Formatted skill tooltip</returns>
        string GetSkillTooltip(string skillId);

        /// <summary>
        /// Check if skill exists
        /// </summary>
        /// <param name="skillId">ID of the skill to check</param>
        /// <returns>True if skill exists</returns>
        bool SkillExists(string skillId);

        /// <summary>
        /// Get passive skill bonuses for the player
        /// </summary>
        /// <returns>Dictionary of stat bonuses from passive skills</returns>
        Dictionary<string, int> GetPassiveSkillBonuses();

        /// <summary>
        /// Apply passive skill effects to player stats
        /// </summary>
        void ApplyPassiveSkillEffects();

        /// <summary>
        /// Remove passive skill effects from player stats
        /// </summary>
        void RemovePassiveSkillEffects();

        /// <summary>
        /// Get skill prerequisites as a formatted string
        /// </summary>
        /// <param name="skillId">ID of the skill</param>
        /// <returns>Formatted prerequisites string</returns>
        string GetSkillPrerequisitesText(string skillId);
    }

    /// <summary>
    /// Skill categories for organization
    /// </summary>
    public enum SkillCategory
    {
        Combat,
        Magic,
        Utility,
        Passive,
        Ultimate
    }

    /// <summary>
    /// Skill types for different behaviors
    /// </summary>
    public enum SkillType
    {
        Active,
        Passive,
        Toggle
    }

    /// <summary>
    /// Represents a skill in the game
    /// </summary>
    public class Skill
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public SkillCategory Category { get; set; }
        public SkillType Type { get; set; }
        public CharacterClass RequiredClass { get; set; }
        public int RequiredLevel { get; set; }
        public int SkillPointCost { get; set; }
        public int Tier { get; set; }
        public List<string> Prerequisites { get; set; }
        public Dictionary<string, int> StatBonuses { get; set; }
        public int Cooldown { get; set; } // In seconds
        public int ManaCost { get; set; }
        public string IconPath { get; set; }
        public bool IsLearned { get; set; }

        public Skill()
        {
            Prerequisites = new List<string>();
            StatBonuses = new Dictionary<string, int>();
            Id = "";
            Name = "";
            Description = "";
            IconPath = "";
        }
    }
} 