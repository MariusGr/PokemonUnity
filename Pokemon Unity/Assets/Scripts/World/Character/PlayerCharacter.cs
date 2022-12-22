using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : Character, IPlayerCharacter
{
    public static PlayerCharacter Instance { get; private set; }
    
    public override bool IsPlayer => true;

    private PlayerData playerData => (PlayerData)characterData;
    private CharacterControllerPlayer playerController => (CharacterControllerPlayer)Controller;

    public PlayerCharacter()
    {
        Instance = this;
        Services.Register(this as IPlayerCharacter);
    }

    private void Awake() => Initialize();

    public Coroutine Defeat() => StartCoroutine(DefeatCoroutine());
    private IEnumerator DefeatCoroutine()
    {
        yield return Services.Get<IDialogBox>().DrawText($"Tja... ab ins Poke-Center. Scheiﬂe gelaufen...", DialogBoxContinueMode.User, closeAfterFinish: true);
        PlayerData.Instance.HealAllPokemons();
        transform.position = playerData.lastPokeCenterEntrance.position;
    }

    public void EnterEntranceTreshhold(Door entrance)
    {
        playerController.EnterEntranceTrehshold(entrance);
    }

    public void LeaveEntranceTrehshold(Door entrance)
    {
        playerController.LeaveEntranceTrehshold(entrance);
    }

    public void TravelToEntrance(Door entrance)
    {
        transform.position = entrance.spawnPosition;
        Movement.LookInDirection(-new GridVector(entrance.directionTriggerToEntrance));
    }
}
