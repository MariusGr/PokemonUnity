using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPawn : MonoBehaviour, IInteractable
{
    [SerializeField] Item item;

    public void Interact(Character player)
    {
        EventManager.Pause();
        StartCoroutine(TakeItem(player));
    }

    private IEnumerator TakeItem(Character player)
    {
        PlayerData.Instance.GiveItem(item);
        yield return Services.Get<IDialogBox>().DrawText(
            $"{player.characterData.name} findet {item.data.name}!", DialogBoxContinueMode.User, closeAfterFinish: true);
        gameObject.SetActive(false);
        EventManager.Unpause();
    }
}
