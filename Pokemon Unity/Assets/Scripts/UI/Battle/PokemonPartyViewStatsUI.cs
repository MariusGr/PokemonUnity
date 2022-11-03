using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PokemonPartyViewStatsUI : PlayerPokemonStatsUI
{
    [SerializeField] private Sprite backgroundFaint;
    [SerializeField] private Sprite backgroundFaintSelected;
    [SerializeField] private Image icon;
    [SerializeField] private GameObject item;
    private Vector3 iconOffset;
    private Sprite backgroundDefault;
    private Sprite backgroundDefaultSelected;
    private new Coroutine animation;
    private Vector3 iconStartPosistion;

    private void Awake()
    {
        iconStartPosistion = icon.transform.localPosition;
    }

    private void OnEnable()
    {
        StartCoroutine(IconAnimation());
        Refresh();
    }

    public override void Initialize(int index)
    {
        base.Initialize(index);
        backgroundDefault = spriteBefore;
        backgroundDefaultSelected = selectedSprite;
    }

    public override void Refresh()
    {
        base.Refresh();
        item.SetActive(false);
        if (!(animation is null))
            StopCoroutine(animation);
        if (pokemon.isFainted)
            SetToFainted();
        else
            SetToDefault();
    }

    public void SetToDefault()
    {
        spriteBefore = backgroundDefault;
        selectedSprite = backgroundDefaultSelected;
        image.sprite = currentSprite;
    }

    public void SetToFainted()
    {
        spriteBefore = backgroundFaint;
        selectedSprite = backgroundFaintSelected;
        image.sprite = currentSprite;
    }

    public override void Select()
    {
        base.Select();
        iconOffset = new Vector3(0, 2.5f, 0);
    }

    public override void Deselect()
    {
        base.Deselect();
        iconOffset = Vector3.zero;
    }

    IEnumerator IconAnimation()
    {
        int iconIndex = 0;
        float interval = Mathf.Clamp((1f - pokemon.hpNormalized), .1f, .8f);
        bool toggle = false;
        while (true)
        {
            yield return new WaitForSeconds(interval);
            iconIndex = (iconIndex + 1) % pokemon.data.icons.Length;
            icon.sprite = pokemon.data.icons[iconIndex];
            if (toggle)
                icon.transform.localPosition = iconStartPosistion + iconOffset;
            else
                icon.transform.localPosition = iconStartPosistion;

            toggle = !toggle;
        }
    }
}
