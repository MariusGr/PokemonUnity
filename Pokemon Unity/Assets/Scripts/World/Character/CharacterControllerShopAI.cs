using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControllerShopAI : CharacterControllerBase, IInteractable
{
    [SerializeField] string greetingText = "Willkommen!\tWie kann ich helfen?";
    [SerializeField] string byeText = "Beehre uns bald wieder!";
    [SerializeField] ItemData[] items;

    public override CharacterData CharacterData { get => null; }

    private IShopUI shopUI;

    private void Awake() => shopUI = Services.Get<IShopUI>();

    public void Interact(Character player)
    {
        EventManager.Pause();
        character.Movement.LookInPlayerDirection();
        StartCoroutine(ShopCoroutine());
    }

    private IEnumerator ShopCoroutine()
    {
        yield return Services.Get<IDialogBox>().DrawText(greetingText, DialogBoxContinueMode.User, true);
        shopUI.Open(CloseShop, items);
    }

    private void CloseShop(ISelectableUIElement selection, bool goBack)
    {
        shopUI.Close();
        EventManager.Unpause();
        Services.Get<IDialogBox>().DrawText(byeText, DialogBoxContinueMode.User, true);
    }
}
