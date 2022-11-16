using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleMenu : SelectionGraphWindow
{
    protected override void ChooseSelectedElement()
    {
        print("Select Battle Option");
        base.ChooseSelectedElement();
        Services.Get<IBattleManager>().ChooseBattleMenuOption(((BattleMenuButton)selectedElement).option);
    }
}
