using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewMove", menuName = "Pokemon/Move")]
public class MoveData : ScriptableObject
{
    public string fullName;
    public PokemonTypeData pokeType;
    public int maxPP;
    public int power;

    public AnimationClip animationClipPlayer;
    public AnimationClip animationClipOpponent;

    public bool hasSpecialUsageText;
    public string specialUsageText;

    public AnimationClip GetAnimationClip(int character)
    {
        if (character == Constants.PlayerIndex)
            return animationClipPlayer;
        return animationClipOpponent;
    }
}
