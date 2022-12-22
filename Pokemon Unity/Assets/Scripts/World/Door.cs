using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    [SerializeField] bool locked;
    [SerializeField] BoxCollider interactionCollider;
    [SerializeField] BoxCollider triggerCollider;

    private void Start()
    {
        interactionCollider.enabled = locked;
        triggerCollider.enabled = !locked;
        if (locked)
            gameObject.layer = LayerManager.ToLayer(LayerManager.Instance.InteractableLayerMask);
        else
            gameObject.layer = LayerManager.ToLayer(LayerManager.Instance.EntranceLayerMask);
    }

    public void Interact(Character player)
    {
        EventManager.Pause();
        StartCoroutine(ShowMessage());
    }

    private IEnumerator ShowMessage()
    {
        yield return Services.Get<IDialogBox>().DrawText("Abgeschlossen.", closeAfterFinish: true);
        EventManager.Unpause();
    }
}
