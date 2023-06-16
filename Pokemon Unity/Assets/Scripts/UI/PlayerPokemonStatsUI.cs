using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CollectionExtensions;

public class PlayerPokemonStatsUI : PokemonStatsUI
{
    [SerializeField] protected ShadowedText hp;
    [SerializeField] ShadowedText maxHP;

    public override void RefreshHP()
    {
        base.RefreshHP();

        hp.text = pokemon.hp.ToString();
        if (!(maxHP is null))
            maxHP.text = pokemon.MaxHp.ToString();
    }

    override public IEnumerator RefreshHPAnimated(float speed)
    {
        StartCoroutine(hpBar.SetValueAnimated(pokemon.HpNormalized, speed));
        StartCoroutine(AnimateHPTextCoroutine());
        yield return new WaitWhile(() => hpBar.IsPlayingAnimation());
    }

    IEnumerator AnimateHPTextCoroutine()
    {
        while(hpBar.IsPlayingAnimation())
        {
            hp.text = ((int)(pokemon.MaxHp * hpBar.Value)).ToString();
            yield return new WaitForEndOfFrame();
        }

        RefreshHP();
    }
}
