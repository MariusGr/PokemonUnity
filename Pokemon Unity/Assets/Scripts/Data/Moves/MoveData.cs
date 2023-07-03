using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CollectionExtensions;
using AYellowpaper;

[CreateAssetMenu(fileName = "NewMove", menuName = "Pokemon/Move")]
public class MoveData : BaseScriptableObject, IMoveData
{
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public string Description { get; private set; }
    [field: SerializeField] public InterfaceReference<IPokemonTypeData, ScriptableObject> PokeType { get; private set; }

    [field: SerializeField] public int MaxPP { get; private set; }
    [field: SerializeField] public int Power { get; private set; }
    [field: SerializeField] public int Accuracy { get; private set; }

    [field: SerializeField] public float StatusNonVolatileInflictedTargetChance { get; private set; } = 1f;
    [field: SerializeField] public InterfaceReference<IStatusEffectNonVolatileData, ScriptableObject> StatusNonVolatileInflictedTarget { get; private set; }
    [field: SerializeField] public InterfaceReference<IStatusEffectNonVolatileData, ScriptableObject> StatusNonVolatileInflictedSelf { get; private set; }
    [field: SerializeField] public float StatusVolatileInflictedTargetChance { get; private set; } = 1f;
    [field: SerializeField] public InterfaceReference<IStatusEffectVolatileData, ScriptableObject> StatusVolatileInflictedTarget { get; private set; }
    [field: SerializeField] public InterfaceReference<IStatusEffectVolatileData, ScriptableObject> StatusVolatileInflictedSelf { get; private set; }

    // TODO: bundle these with wait text that is currently defined in StatusEffectData?
    [field: SerializeField] public int RoundsBeforeFirstEffectVolatile { get; private set; } = 0;
    [field: SerializeField] public int RoundsBeforeFirstEffectNonVolatile { get; private set; } = 0;

    [field: SerializeField] public InspectorFriendlySerializableDictionary<Stat, int> StatModifiersSelf { get; private set; }
    [field: SerializeField] public InspectorFriendlySerializableDictionary<Stat, int> StatModifiersTarget { get; private set; }
    [field: SerializeField] public InterfaceReference<IMoveCategory, ScriptableObject> Category { get; private set; }

    [field: SerializeField] public AnimationClip AnimationClipPlayer { get; private set; }
    [field: SerializeField] public AnimationClip AnimationClipOpponent { get; private set; }
    [field: SerializeField] public AudioClip Sound { get; private set; }
    [field: SerializeField] public bool UsesCryForSound { get; private set; }

    [field: SerializeField] public bool HasSpecialUsageText { get; private set; }
    [field: SerializeField] public string SpecialUsageText { get; private set; }

    [field: SerializeField] public float Recoil { get; private set; } = 0;

    public bool DoesNotInflictDamage => Power < 1;
    public bool DoesNotInflictRecoil => Recoil < 1;

    public bool DoesNotModifyStats
        => StatModifiersTarget.Count < 1 &&
        StatModifiersSelf.Count < 1;

    public bool OnlyInflictsNonVolatileStatusEffectOnTarget
        => DoesNotInflictDamage &&
        !(StatusNonVolatileInflictedTarget is null) &&
        StatusVolatileInflictedTarget is null &&
        StatusNonVolatileInflictedSelf is null &&
        StatusVolatileInflictedSelf is null &&
        DoesNotInflictRecoil &&
        DoesNotModifyStats;

    public bool OnlyInflictsVolatileStatusOnTarget
        => DoesNotInflictDamage &&
        !(StatusVolatileInflictedTarget is null) &&
        StatusNonVolatileInflictedTarget is null &&
        StatusNonVolatileInflictedSelf is null &&
        StatusVolatileInflictedSelf is null &&
        DoesNotInflictRecoil &&
        DoesNotModifyStats;

    public bool OnlyInflictsBothStatusEffectsOnTarget
        => DoesNotInflictDamage &&
        !(StatusVolatileInflictedTarget is null) &&
        !(StatusNonVolatileInflictedTarget is null) &&
        StatusNonVolatileInflictedSelf is null &&
        StatusVolatileInflictedSelf is null &&
        DoesNotInflictRecoil &&
        DoesNotModifyStats;

    public AnimationClip GetAnimationClip(int character)
    {
        if (character == Constants.PlayerIndex)
            return AnimationClipPlayer;
        return AnimationClipOpponent;
    }
}
