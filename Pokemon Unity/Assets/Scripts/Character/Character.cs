using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField] CharacterControllerBase controller;
    [SerializeField] CharacterMovement movement;
    [SerializeField] CharacterAnimator animator;
    [SerializeField] public Pokemon[] pokemons;

    public CharacterControllerBase Controller => controller;
    public CharacterMovement Movement => movement;
    public CharacterAnimator Animator => animator;
    public GridVector position => new GridVector(transform.position);

    public bool RaycastForward(Vector3 direction, LayerMask layerMask, out RaycastHit hitInfo)
        => Physics.Raycast(
            origin: transform.position + Vector3.up * .5f,
            direction: direction,
            maxDistance: .8f,
            layerMask: layerMask,
            hitInfo: out hitInfo
            );

    public bool RaycastForward(Vector3 direction, LayerMask layerMask)
    {
        RaycastHit hitInfo;
        return RaycastForward(direction, layerMask, out hitInfo);
    }

    public bool RaycastForward(LayerMask layerMask)
        => RaycastForward(movement.CurrentDirectionVector, layerMask);

    public void TryInteract()
    {
        RaycastHit hitInfo;
        if (RaycastForward(movement.CurrentDirectionVector, LayerManager.Instance.InteractableLayerMask, out hitInfo))
            hitInfo.collider.gameObject.GetComponent<Interactable>()?.Interact(this); ;
    }
}
