using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CollectionExtensions;
using AYellowpaper;

public interface IMoveData : IInstanceWithId
{
    public string Name { get; }

    public string Description { get; }
    public InterfaceReference<IPokemonTypeData, ScriptableObject> PokeType { get; }
    public int MaxPP { get; }
    public int Power { get; }
    public int Accuracy { get; }

    public float StatusNonVolatileInflictedTargetChance { get; }
    public InterfaceReference<IStatusEffectNonVolatileData, ScriptableObject> StatusNonVolatileInflictedTarget { get; }
    public InterfaceReference<IStatusEffectNonVolatileData, ScriptableObject> StatusNonVolatileInflictedSelf { get; }
    public float StatusVolatileInflictedTargetChance { get; }
    public InterfaceReference<IStatusEffectVolatileData, ScriptableObject> StatusVolatileInflictedTarget { get; }
    public InterfaceReference<IStatusEffectVolatileData, ScriptableObject> StatusVolatileInflictedSelf { get; }

    // TODO: bundle these with wait text that is currently defined in StatusEffectData?
    public int RoundsBeforeFirstEffectVolatile { get; }
    public int RoundsBeforeFirstEffectNonVolatile { get; }

    public InspectorFriendlySerializableDictionary<Stat, int> StatModifiersSelf { get; }
    public InspectorFriendlySerializableDictionary<Stat, int> StatModifiersTarget { get; }
    public InterfaceReference<IMoveCategory, ScriptableObject> Category { get; }

    public AnimationClip AnimationClipPlayer { get; }
    public AnimationClip AnimationClipOpponent { get; }
    public AudioClip Sound { get; }
    public bool UsesCryForSound { get; }

    public bool HasSpecialUsageText { get; }
    public string SpecialUsageText { get; }

    public float Recoil { get; }

    public bool DoesNotInflictDamage { get; }
    public bool DoesNotInflictRecoil { get; }

    public bool DoesNotModifyStats { get; }
    public bool OnlyInflictsNonVolatileStatusEffectOnTarget { get; }
    public bool OnlyInflictsVolatileStatusOnTarget { get; }
    public bool OnlyInflictsBothStatusEffectsOnTarget { get; }
    public AnimationClip GetAnimationClip(int character);
}
