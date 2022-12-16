using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewMove", menuName = "Pokemon/Move")]
public class MoveData : ScriptableObject
{
    public string fullName;
    public string description;
    public PokemonTypeData pokeType;
    public int maxPP;
    public int power;
    public int accuracy;
    public StatusEffectNonVolatile statusInflictedTarget;
    public StatusEffectNonVolatile statusInflictedSelf;
    public MoveCategory category;

    public AnimationClip animationClipPlayer;
    public AnimationClip animationClipOpponent;

    public bool hasSpecialUsageText;
    public string specialUsageText;

    public float recoil = 0;

    public AnimationClip GetAnimationClip(int character)
    {
        if (character == Constants.PlayerIndex)
            return animationClipPlayer;
        return animationClipOpponent;
    }
}
