using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CollectionExtensions;

[CreateAssetMenu(fileName = "NewMove", menuName = "Pokemon/Move")]
public class MoveData : BaseScriptableObject
{
    public string fullName;
    public string description;
    public PokemonTypeData pokeType;

    public int maxPP;
    public int power;
    public int accuracy;

    public StatusEffectNonVolatileData statusNonVolatileInflictedTarget;
    public StatusEffectNonVolatileData statusNonVolatileInflictedSelf;
    public StatusEffectVolatileData statusVolatileInflictedTarget;
    public StatusEffectVolatileData statusVolatileInflictedSelf;
    public InspectorFriendlySerializableDictionary<Stat, int> statModifiersSelf;
    public InspectorFriendlySerializableDictionary<Stat, int> statModifiersTarget;
    public MoveCategory category;

    public AnimationClip animationClipPlayer;
    public AnimationClip animationClipOpponent;
    public AudioClip sound;
    public bool usesCryForSound;

    public bool hasSpecialUsageText;
    public string specialUsageText;

    public float recoil = 0;

    public bool doesNotInflictDamage => power < 1;
    public bool doesNotInflictRecoil => recoil < 1;

    public bool doesNotModifyStats
        => statModifiersTarget.Count < 1 &&
        statModifiersSelf.Count < 1;

    public bool onlyInflictsNonVolatileStatusEffectOnTarget
        => doesNotInflictDamage &&
        !(statusNonVolatileInflictedTarget is null) &&
        statusVolatileInflictedTarget is null &&
        statusNonVolatileInflictedSelf is null &&
        statusVolatileInflictedSelf is null &&
        doesNotInflictRecoil &&
        doesNotModifyStats;

    public bool onlyInflictsVolatileStatusOnTarget
        => doesNotInflictDamage &&
        !(statusVolatileInflictedTarget is null) &&
        statusNonVolatileInflictedTarget is null &&
        statusNonVolatileInflictedSelf is null &&
        statusVolatileInflictedSelf is null &&
        doesNotInflictRecoil &&
        doesNotModifyStats;

    public bool onlyInflictsBothStatusEffectsOnTarget
        => doesNotInflictDamage &&
        !(statusVolatileInflictedTarget is null) &&
        !(statusNonVolatileInflictedTarget is null) &&
        statusNonVolatileInflictedSelf is null &&
        statusVolatileInflictedSelf is null &&
        doesNotInflictRecoil &&
        doesNotModifyStats;

    public AnimationClip GetAnimationClip(int character)
    {
        if (character == Constants.PlayerIndex)
            return animationClipPlayer;
        return animationClipOpponent;
    }
}
