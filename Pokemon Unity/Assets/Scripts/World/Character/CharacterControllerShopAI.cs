using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControllerShopAI : CharacterControllerBase, IInteractable
{
    [SerializeField] string greetingText = "Willkommen!\nKann ich helfen?";
    [SerializeField] string byeText = "Beehre uns bald wieder!";

    public override CharacterData CharacterData { get => null; }

    public void Interact(Character player)
    {
        EventManager.Pause();
        character.Movement.LookInPlayerDirection();
        StartCoroutine(ShopCoroutine());
    }

    private IEnumerator ShopCoroutine()
    {
        yield return Services.Get<IDialogBox>().DrawText(greetingText, DialogBoxContinueMode.User, true);
        EventManager.Unpause();
        yield return Services.Get<IDialogBox>().DrawText(byeText, DialogBoxContinueMode.User, true);
    }
}
