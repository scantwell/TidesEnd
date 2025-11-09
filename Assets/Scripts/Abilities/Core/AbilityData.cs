using UnityEngine;

namespace TidesEnd.Abilities
{
    /// <summary>
    /// ScriptableObject base for ALL abilities (player, enemy, and boss).
    /// Defines the core parameters, targeting mode, effects, and visual/audio feedback for an ability.
    /// </summary>
    [CreateAssetMenu(fileName = "New Ability", menuName = "Tide's End/Abilities/Ability Data")]
    public class AbilityData : ScriptableObject
    {
        [Header("Identity")]
        [Tooltip("Display name of the ability")]
        public string abilityName;

        [TextArea(3, 5)]
        [Tooltip("Description shown in UI and tooltips")]
        public string description;

        [Tooltip("Icon displayed in UI")]
        public Sprite icon;

        [Tooltip("Type of ability determines execution logic")]
        public AbilityType abilityType;

        [Tooltip("Category helps organize abilities")]
        public AbilityCategory category;

        [Header("Timing")]
        [Tooltip("Cooldown duration in seconds")]
        public float cooldown = 60f;

        [Tooltip("Cast time before ability executes (0 = instant)")]
        public float castTime = 0f;

        [Tooltip("Duration ability remains active (0 = instant)")]
        public float duration = 0f;

        [Tooltip("Can this ability be cast while already casting another?")]
        public bool canCastWhileCasting = false;

        [Header("Targeting")]
        [Tooltip("How the ability selects its target(s)")]
        public TargetingMode targetingMode;

        [Tooltip("Maximum range for targeting/activation")]
        public float range = 0f;

        [Tooltip("Radius of effect area")]
        public float radius = 0f;

        [Tooltip("Requires line of sight to target?")]
        public bool requiresLineOfSight = false;

        [Header("Effects")]
        [SerializeReference]
        [Tooltip("Array of effects applied when ability activates")]
        public AbilityEffect[] effects = new AbilityEffect[0];

        [Header("VFX/Audio")]
        [Tooltip("VFX spawned at cast start")]
        public GameObject castVFXPrefab;

        [Tooltip("VFX spawned while ability is active (zones, projectiles)")]
        public GameObject activeVFXPrefab;

        [Tooltip("VFX spawned on impact/completion")]
        public GameObject impactVFXPrefab;

        [Tooltip("Audio played at cast start")]
        public AudioClip castSound;

        [Tooltip("Audio looped while ability is active")]
        public AudioClip activeSound;

        [Tooltip("Audio played when ability ends")]
        public AudioClip endSound;
        
        [Tooltip("Audio played when ability impacts")]
        public AudioClip impactSound;

        [Header("AI Parameters (For Enemies/Bosses)")]
        [Range(0f, 1f)]
        [Tooltip("0-1, how eagerly AI uses this ability. Higher = more frequent use.")]
        public float aiUsagePriority = 0.5f;

        [Tooltip("Conditions that must be met for AI to use this ability")]
        public AIUseCondition[] aiUseConditions = new AIUseCondition[0];
    }

    /// <summary>
    /// Enum defining the execution pattern of an ability.
    /// Each type has a corresponding AbilityInstance implementation.
    /// </summary>
    public enum AbilityType
    {
        Passive,        // Always active (player passives, boss auras)
        Deployable,     // Placed objects (Bulwark anchor, ritual totems)
        Projectile,     // Fired projectiles (flare, harpoon)
        PlacedZone,     // Ground zones (sanctuary, whirlpool, speed zones)
        Buff,           // Self buffs (Harpooner frenzy)
        ChanneledAoE,   // Channeled effects (Occultist purge, boss pulses)
        SummonMinions,  // Spawn enemies (boss summons)
        Teleport,       // Instant movement (Shambler teleport)
        Possess,        // Take control of target (The Color)
        Transform       // Change entity state (Tide Touched phase)
    }

    /// <summary>
    /// Category for organizational purposes.
    /// </summary>
    public enum AbilityCategory
    {
        Player,
        Enemy,
        Boss
    }

    /// <summary>
    /// Defines how the ability selects and targets entities or positions.
    /// </summary>
    public enum TargetingMode
    {
        Self,           // Caster only
        SingleEnemy,    // One enemy entity
        SingleAlly,     // One ally entity
        AllEnemies,     // All enemies in range
        AllAllies,      // All allies in range
        Ground,         // Position on ground (raycast)
        Direction,      // Direction from caster (forward vector)
        Random          // Random valid target within range
    }

    /// <summary>
    /// Condition data for AI decision-making.
    /// Determines when an enemy or boss should use an ability.
    /// </summary>
    [System.Serializable]
    public struct AIUseCondition
    {
        [Tooltip("Type of condition to evaluate")]
        public AIConditionType conditionType;

        [Tooltip("Threshold value (health %, distance, etc.)")]
        public float threshold;

        [Tooltip("Minimum count required (for allies nearby, etc.)")]
        public int minCount;
    }

    /// <summary>
    /// Types of conditions that can trigger AI ability usage.
    /// </summary>
    public enum AIConditionType
    {
        Always,                         // Use whenever off cooldown (bosses)
        HealthBelow,                    // Use when health < threshold %
        HealthAbove,                    // Use when health > threshold %
        DistanceToTargetLessThan,       // Use when close to target
        DistanceToTargetGreaterThan,    // Use when far from target
        AlliesNearby,                   // Use when allies within radius
        EnemiesNearby,                  // Use when enemies within radius
        InCombat,                       // Use only during combat state
        TargetLowHealth                 // Use when target health < threshold %
    }
}
