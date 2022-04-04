using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveButton : MonoBehaviour
{
    [SerializeField] private int index;
    [SerializeField] private Text textName;
    [SerializeField] private Text textPP;
    [SerializeField] private Image imageType;
    [SerializeField] private Image imageCover;

    public void AssignMove(Move move)
    {
        index = move.index;
        textName.text = move.data.fullName;
        textPP.text = $"{move.pp}/{move.data.maxPP}";
        imageType.sprite = move.data.pokeType.titleSprite;
        imageCover.color = move.data.pokeType.color;
    }

    public void AssignNone()
    {
        textName.text = "";
        textPP.text = "";
        imageType.sprite = null;
        imageCover.color = Color.grey;
    }
}
