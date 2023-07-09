using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PC : MonoBehaviour, IInteractable
{
    [SerializeField] private AudioClip openSound;
    [SerializeField] private AudioClip closeSound;

    public void Interact(Character player)
    {
        EventManager.Pause();
        SfxHandler.Play(openSound);
        PokeBoxUI.Instance.Open(CloseBox);
    }

    private void CloseBox(SelectableUIElement selection, bool goBack)
    {
        if (goBack)
        {
            SfxHandler.Play(closeSound);
            PokeBoxUI.Instance.Close();
            EventManager.Unpause();
        }
    }
}
