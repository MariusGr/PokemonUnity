using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PC : MonoBehaviour, IInteractable
{
    private IPokeBoxUI ui;
    private void Awake() => ui = Services.Get<IPokeBoxUI>();

    public void Interact(Character player)
    {
        EventManager.Pause();
        ui.Open(CloseBox);
    }

    private void CloseBox(ISelectableUIElement selection, bool goBack)
    {
        if (goBack)
        {
            ui.Close();
            EventManager.Unpause();
        }
    }
}
