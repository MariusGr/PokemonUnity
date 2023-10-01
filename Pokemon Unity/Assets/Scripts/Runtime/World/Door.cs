using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    [SerializeField] private bool locked;
    [SerializeField] private BoxCollider interactionCollider;
    [SerializeField] private BoxCollider triggerCollider;
    [SerializeField] private Transform spawn;
    [SerializeField] private Door otherSide;
    [SerializeField] private AudioClip enterSound;

    public Vector3 spawnPosition => spawn.position;
    public Direction directionTriggerToEntrance { get; private set; }

    private void Start()
    {
        Vector3 entrancePositon = transform.TransformPoint(interactionCollider.center + transform.localPosition);
        Vector3 triggerPositon = transform.TransformPoint(triggerCollider.center + transform.localPosition);
        directionTriggerToEntrance = GridVector.GetLookAt(triggerPositon, entrancePositon).ToDirection();
        locked = locked || otherSide is null;
        interactionCollider.enabled = locked;
        triggerCollider.enabled = !locked;
        if (locked)
            gameObject.layer = LayerManager.InteractableLayer;
        else
            gameObject.layer = LayerManager.EntranceLayer;
    }

    public void Interact(Character player)
    {
        EventManager.Pause();
        StartCoroutine(ShowMessage());
    }

    private IEnumerator ShowMessage()
    {
        yield return DialogBox.Instance.DrawText("Abgeschlossen.", closeAfterFinish: true);
        EventManager.Unpause();
    }

    void OnTriggerEnter(Collider other) => PlayerCharacter.Instance.EnterEntranceTreshhold(this);
    void OnTriggerExit(Collider other) => PlayerCharacter.Instance.LeaveEntranceTrehshold(this);

    public void Enter()
    {
        EventManager.Pause();
        SfxHandler.Play(enterSound);
        StartCoroutine(EnterCoroutine());
    }

    private IEnumerator EnterCoroutine()
    {
        yield return FadeBlack.Instance.FadeToBlack();
        PlayerCharacter.Instance.TravelToEntrance(otherSide);
        yield return FadeBlack.Instance.FadeFromBlack();
        EventManager.Unpause();
    }
}
