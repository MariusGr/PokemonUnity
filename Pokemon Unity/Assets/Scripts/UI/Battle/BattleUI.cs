using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleUI : MonoBehaviour, IBattleUI
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private MoveButtonsCollection moveButtons;

    void Awake()
    {
        Services.Register(this as IBattleUI);
    }

    void Update()
    {
        
    }

    public void Initialize(CharacterData playerData, NPCData opponentData)
    {
        moveButtons.AssignMoves(playerData.pokemons[0].moves.ToArray());
        canvas.enabled = true;
    }
}
