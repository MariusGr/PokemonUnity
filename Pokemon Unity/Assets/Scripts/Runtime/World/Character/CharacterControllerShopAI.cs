using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControllerShopAI : CharacterControllerBase, IInteractable
{
    [SerializeField] string greetingText = "Willkommen!\tWie kann ich helfen?";
    [SerializeField] string byeText = "Beehre uns bald wieder!";
    [SerializeField] ItemData[] items;

    public override CharacterData CharacterData { get => null; }

    public void Interact(Character player)
    {
        EventManager.Pause();
        character.Movement.LookInPlayerDirection();
        StartCoroutine(ShopCoroutine());
    }

    private IEnumerator ShopCoroutine()
    {
        yield return DialogBox.Instance.DrawText(greetingText, DialogBoxContinueMode.User, true);
        ShopUI.Instance.Open(CloseShop, items);
    }

    private void CloseShop(SelectableUIElement selection, bool goBack)
    {
        ShopUI.Instance.Close();
        EventManager.Unpause();
        DialogBox.Instance.DrawText(byeText, DialogBoxContinueMode.User, true);
    }
}
