using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class PokemonPartyViewStatsUI : PlayerPokemonStatsUI
{
    [SerializeField] Image background;
    [SerializeField] Sprite backgroundDefault;

    public void Awake()
    {
        SetBackgroundToDefault();
    }

    public void SetBackgroundToDefault() => background.sprite = backgroundDefault;

#if (UNITY_EDITOR)
    private void Update()
    {
        SetBackgroundToDefault();
    }
#endif
}
