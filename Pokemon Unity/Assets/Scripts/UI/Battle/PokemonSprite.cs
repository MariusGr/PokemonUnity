using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokemonSprite : AnimatedSprite
{
    [SerializeField] int owner;
    [SerializeField] AnimationClip[] faintAnimation;

    public void PlayFaintAnimation() => PlayAnimation(faintAnimation[owner]);
    public void PlayAnimation(Move move) => PlayAnimation(move.data.GetAnimationClip(owner));
}
