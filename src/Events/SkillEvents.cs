using System.Collections.Generic;
using WinFormsApp1.Interfaces;

namespace WinFormsApp1.Events
{
    /// <summary>
    /// Event published when a player learns a new skill
    /// </summary>
    public class SkillLearnedEvent : GameEvent
    {
        public Player Player { get; set; }
        public Skill Skill { get; set; }
        public int SkillPointsSpent { get; set; }
        public int RemainingSkillPoints { get; set; }
        public bool WasSuccessful { get; set; }
        public string FailureReason { get; set; }

        public override int Priority => 8; // High priority

        public SkillLearnedEvent(Player player, Skill skill, int skillPointsSpent, int remainingSkillPoints, bool wasSuccessful, string failureReason = "")
        {
            Player = player;
            Skill = skill;
            SkillPointsSpent = skillPointsSpent;
            RemainingSkillPoints = remainingSkillPoints;
            WasSuccessful = wasSuccessful;
            FailureReason = failureReason;
            Source = "SkillManager";
        }
    }

    /// <summary>
    /// Event published when a player uses an active skill
    /// </summary>
    public class SkillUsedEvent : GameEvent
    {
        public Player Player { get; set; }
        public Skill Skill { get; set; }
        public object Target { get; set; }
        public bool WasSuccessful { get; set; }
        public string FailureReason { get; set; }
        public Dictionary<string, object> SkillEffects { get; set; }
        public int ManaCost { get; set; }
        public double CooldownDuration { get; set; }

        public override int Priority => 7; // High priority

        public SkillUsedEvent(Player player, Skill skill, object target, bool wasSuccessful, string failureReason = "", Dictionary<string, object> skillEffects = null, int manaCost = 0, double cooldownDuration = 0)
        {
            Player = player;
            Skill = skill;
            Target = target;
            WasSuccessful = wasSuccessful;
            FailureReason = failureReason;
            SkillEffects = skillEffects ?? new Dictionary<string, object>();
            ManaCost = manaCost;
            CooldownDuration = cooldownDuration;
            Source = "SkillManager";
        }
    }

    /// <summary>
    /// Event published when skills are reset
    /// </summary>
    public class SkillsResetEvent : GameEvent
    {
        public Player Player { get; set; }
        public List<string> ResetSkills { get; set; }
        public int RefundedSkillPoints { get; set; }
        public double RefundPercentage { get; set; }
        public bool WasSuccessful { get; set; }
        public string FailureReason { get; set; }

        public SkillsResetEvent(Player player, List<string> resetSkills, int refundedSkillPoints, double refundPercentage, bool wasSuccessful, string failureReason = "")
        {
            Player = player;
            ResetSkills = resetSkills ?? new List<string>();
            RefundedSkillPoints = refundedSkillPoints;
            RefundPercentage = refundPercentage;
            WasSuccessful = wasSuccessful;
            FailureReason = failureReason;
            Source = "SkillManager";
        }
    }

    /// <summary>
    /// Event published when passive skill effects are applied or removed
    /// </summary>
    public class PassiveSkillEffectsChangedEvent : GameEvent
    {
        public Player Player { get; set; }
        public Dictionary<string, int> StatBonuses { get; set; }
        public bool WereApplied { get; set; } // True if applied, false if removed
        public List<string> AffectedSkills { get; set; }

        public PassiveSkillEffectsChangedEvent(Player player, Dictionary<string, int> statBonuses, bool wereApplied, List<string> affectedSkills = null)
        {
            Player = player;
            StatBonuses = statBonuses ?? new Dictionary<string, int>();
            WereApplied = wereApplied;
            AffectedSkills = affectedSkills ?? new List<string>();
            Source = "SkillManager";
        }
    }

    /// <summary>
    /// Event published when skills are loaded from data
    /// </summary>
    public class SkillsLoadedEvent : GameEvent
    {
        public int SkillCount { get; set; }
        public bool WasSuccessful { get; set; }
        public string ErrorMessage { get; set; }
        public List<string> LoadedSkillIds { get; set; }
        public Dictionary<CharacterClass, int> SkillsByClass { get; set; }

        public override int Priority => 15; // Highest priority

        public SkillsLoadedEvent(int skillCount, bool wasSuccessful, string errorMessage = "", List<string> loadedSkillIds = null, Dictionary<CharacterClass, int> skillsByClass = null)
        {
            SkillCount = skillCount;
            WasSuccessful = wasSuccessful;
            ErrorMessage = errorMessage;
            LoadedSkillIds = loadedSkillIds ?? new List<string>();
            SkillsByClass = skillsByClass ?? new Dictionary<CharacterClass, int>();
            Source = "SkillManager";
        }
    }

    /// <summary>
    /// Event published when a new skill is added to the game
    /// </summary>
    public class SkillAddedEvent : GameEvent
    {
        public Skill Skill { get; set; }
        public bool WasSuccessful { get; set; }
        public string FailureReason { get; set; }

        public SkillAddedEvent(Skill skill, bool wasSuccessful, string failureReason = "")
        {
            Skill = skill;
            WasSuccessful = wasSuccessful;
            FailureReason = failureReason;
            Source = "SkillManager";
        }
    }

    /// <summary>
    /// Event published when a skill is removed from the game
    /// </summary>
    public class SkillRemovedEvent : GameEvent
    {
        public string SkillId { get; set; }
        public Skill RemovedSkill { get; set; }
        public bool WasSuccessful { get; set; }
        public string FailureReason { get; set; }
        public bool WasLearnedByPlayer { get; set; }

        public SkillRemovedEvent(string skillId, Skill removedSkill, bool wasSuccessful, string failureReason = "", bool wasLearnedByPlayer = false)
        {
            SkillId = skillId;
            RemovedSkill = removedSkill;
            WasSuccessful = wasSuccessful;
            FailureReason = failureReason;
            WasLearnedByPlayer = wasLearnedByPlayer;
            Source = "SkillManager";
        }
    }

    /// <summary>
    /// Event published when skill cooldown expires
    /// </summary>
    public class SkillCooldownExpiredEvent : GameEvent
    {
        public Player Player { get; set; }
        public Skill Skill { get; set; }
        public double CooldownDuration { get; set; }

        public SkillCooldownExpiredEvent(Player player, Skill skill, double cooldownDuration)
        {
            Player = player;
            Skill = skill;
            CooldownDuration = cooldownDuration;
            Source = "SkillManager";
        }
    }

    /// <summary>
    /// Event published when skill tree is updated for a player
    /// </summary>
    public class SkillTreeUpdatedEvent : GameEvent
    {
        public Player Player { get; set; }
        public CharacterClass CharacterClass { get; set; }
        public Dictionary<int, List<string>> SkillTree { get; set; }
        public List<string> LearnedSkills { get; set; }
        public List<string> AvailableSkills { get; set; }
        public int AvailableSkillPoints { get; set; }

        public override int Priority => 6; // Medium-high priority

        public SkillTreeUpdatedEvent(Player player, CharacterClass characterClass, Dictionary<int, List<string>> skillTree, List<string> learnedSkills, List<string> availableSkills, int availableSkillPoints)
        {
            Player = player;
            CharacterClass = characterClass;
            SkillTree = skillTree ?? new Dictionary<int, List<string>>();
            LearnedSkills = learnedSkills ?? new List<string>();
            AvailableSkills = availableSkills ?? new List<string>();
            AvailableSkillPoints = availableSkillPoints;
            Source = "SkillManager";
        }
    }

    /// <summary>
    /// Event published when skill prerequisites are checked
    /// </summary>
    public class SkillPrerequisiteCheckEvent : GameEvent
    {
        public Player Player { get; set; }
        public Skill Skill { get; set; }
        public bool PrerequisitesMet { get; set; }
        public List<string> MissingPrerequisites { get; set; }
        public string FailureReason { get; set; }

        public SkillPrerequisiteCheckEvent(Player player, Skill skill, bool prerequisitesMet, List<string> missingPrerequisites = null, string failureReason = "")
        {
            Player = player;
            Skill = skill;
            PrerequisitesMet = prerequisitesMet;
            MissingPrerequisites = missingPrerequisites ?? new List<string>();
            FailureReason = failureReason;
            Source = "SkillManager";
        }
    }
} 