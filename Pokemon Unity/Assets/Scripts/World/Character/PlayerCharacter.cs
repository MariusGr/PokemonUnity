using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : Character, IPlayerCharacter
{
    private PlayerData playerData => (PlayerData)characterData;
    public override bool IsPlayer => true;

    public static PlayerCharacter Instance { get; private set; }
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
}
