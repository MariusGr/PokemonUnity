using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleUI : MonoBehaviour, IBattleUI
{
    [SerializeField] private MoveButtonsCollection moveButtons;

    void Awake()
    {
        Services.Register(this);
    }

    void Update()
    {
        
    }

    public void StartNewBattle(CharacterData playerData, NPCData opponentData)
    {
        moveButtons.AssignMoves(playerData.pokemons[0].moves);
    }
}
