using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PokemonSprite : AnimatedSprite
{
    [SerializeField] int owner;
    [SerializeField] AnimationClip[] faintAnimation;
    [SerializeField] AnimationClip[] inflictStatusAnimation;
    [SerializeField] AnimationClip[] statUpAnimation;
    [SerializeField] AnimationClip[] statDownAnimation;

    [SerializeField] RawImage overlay;
    [SerializeField] Texture healOverlay;
    [SerializeField] Texture statUpOverlay;
    [SerializeField] Texture statDownOverlay;

    public IEnumerator PlayInflictStatusAnimation() => PlayAnimation(inflictStatusAnimation[owner]);
    public IEnumerator PlayFaintAnimation() => PlayAnimation(faintAnimation[owner]);
    public IEnumerator PlayMoveAnimation(Move move) => PlayAnimation(move.data.GetAnimationClip(owner));

    public IEnumerator PlayStatUpAnimation()
    {
        overlay.texture = statUpOverlay;
        return PlayAnimation(statUpAnimation[owner]);
    }

    public IEnumerator PlayStatDownAnimation()
    {
        overlay.texture = statDownOverlay;
        return PlayAnimation(statDownAnimation[owner]);
    }

    public void SetSpriteVisible(bool visible) => spriteImage.enabled = visible;
}
