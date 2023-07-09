using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PC : MonoBehaviour, IInteractable
{
    [SerializeField] private AudioClip openSound;
    [SerializeField] private AudioClip closeSound;

    private IPokeBoxUI ui;
    private void Awake() => ui = Services.Get<IPokeBoxUI>();

    public void Interact(Character player)
    {
        EventManager.Pause();
        SfxHandler.Play(openSound);
        ui.Open(CloseBox);
    }

    private void CloseBox(ISelectableUIElement selection, bool goBack)
    {
        if (goBack)
        {
            SfxHandler.Play(closeSound);
            ui.Close();
            EventManager.Unpause();
        }
    }
}
