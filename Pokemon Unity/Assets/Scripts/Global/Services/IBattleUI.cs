using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBattleUI : IService
{
    public void Initialize(CharacterData playerData, NPCData opponentData);
}
