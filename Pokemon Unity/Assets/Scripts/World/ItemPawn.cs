using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPawn : MonoBehaviour, IInteractable
{
    [SerializeField] Item item;
    [SerializeField] AudioClip pickUpMusic;

    public void Interact(Character player)
    {
        EventManager.Pause();
        StartCoroutine(TakeItem(player));
    }

    private IEnumerator TakeItem(Character player)
    {
        PlayerData.Instance.GiveItem(item);
        BgmHandler.Instance.PlayMFX(pickUpMusic);
        Services.Get<IDialogBox>().DrawText(
            $"{player.characterData.name} findet {item.data.name}!", DialogBoxContinueMode.External);
        yield return new WaitForSeconds(pickUpMusic.length);
        Services.Get<IDialogBox>().Close();
        gameObject.SetActive(false);
        EventManager.Unpause();
    }
}
