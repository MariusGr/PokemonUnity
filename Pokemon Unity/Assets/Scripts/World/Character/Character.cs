using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public static Character PlayerCharacter { get; private set; }

    [SerializeField] CharacterControllerBase controller;
    [SerializeField] CharacterMovement movement;
    [SerializeField] CharacterAnimator animator;

    public CharacterControllerBase Controller => controller;
    public CharacterMovement Movement => movement;
    public CharacterAnimator Animator => animator;
    public GridVector position => new GridVector(transform.position);
    public CharacterData characterData => controller.CharacterData;
    public Pokemon[] pokemons => controller.CharacterData.pokemons;

    private GridVector startPosition;

    public void Awake()
    {
        if (controller.GetType() == typeof(CharacterControllerPlayer))
            PlayerCharacter = this;
        foreach (Pokemon pokemon in pokemons)
            pokemon.Initialize();
        startPosition = new GridVector(transform.position);
    }

    public void Reset()
    {
        movement.LookInStartDirection();
        transform.position = startPosition;
    }

    public bool RaycastForward(Vector3 direction, LayerMask layerMask, out RaycastHit hitInfo, float maxDistance = .8f)
    {
        Debug.DrawLine(transform.position + Vector3.up * .5f, transform.position + Vector3.up * .5f + direction * maxDistance, Color.red, .2f);
        return Physics.Raycast(
                   origin: transform.position + Vector3.up * .5f,
                   direction: direction,
                   maxDistance: maxDistance,
                   layerMask: layerMask,
                   hitInfo: out hitInfo
                   );
    }

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
            hitInfo.collider.gameObject.GetComponent<IInteractable>()?.Interact(this); ;
    }
}
