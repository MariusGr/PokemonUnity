using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PokemonPartyViewStatsUI : PlayerPokemonStatsUI
{
    [SerializeField] protected Sprite backgroundDefault;
    [SerializeField] protected Sprite backgroundDefaultSelected;
    [SerializeField] protected Sprite backgroundFaint;
    [SerializeField] protected Sprite backgroundFaintSelected;
    [SerializeField] private Image icon;
    [SerializeField] private bool animteIcon;
    [SerializeField] private GameObject item;
    private Vector3 iconOffset;
    
    private new Coroutine animation;
    private Vector3 iconStartPosistion;

    private void Awake() => DoOnAwake();
    private void OnEnable() => DoOnEnable();
    protected void DoOnAwake() => iconStartPosistion = icon.transform.localPosition;
    protected virtual Sprite GetCurrentBackgroundIdle() => pokemon.isFainted ? backgroundFaint : backgroundDefault;
    protected virtual Sprite GetCurrentBackgroundSelected() => pokemon.isFainted ? backgroundFaintSelected : backgroundDefaultSelected;

    protected void DoOnEnable()
    {
        if (animteIcon)
            StartCoroutine(IconAnimation());
        Refresh();
    }

    public override void Initialize(int index) => base.Initialize(index);

    public override void Refresh()
    {
        base.Refresh();
        item.SetActive(false);
        if (!(animation is null))
            StopCoroutine(animation);
        spriteBefore = GetCurrentBackgroundIdle();
        selectedSprite = GetCurrentBackgroundSelected();
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
