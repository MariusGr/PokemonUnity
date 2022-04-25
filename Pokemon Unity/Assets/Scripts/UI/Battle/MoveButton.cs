using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveButton : MonoBehaviour
{
    [SerializeField] private int index;
    [SerializeField] private ShadowedText textName;
    [SerializeField] private ShadowedText textPP;
    [SerializeField] private Image imageType;
    [SerializeField] private Image imageBackground;
    [SerializeField] private Image imageCover;

    public void AssignMove(Move move)
    {
        index = move.index;
        imageCover.enabled = true;
        imageType.enabled = true;
        imageBackground.color = Color.white;
        textName.text = move.data.fullName;
        textPP.text = $"{move.pp}/{move.data.maxPP}";
        imageType.sprite = move.data.pokeType.titleSprite;
        imageCover.color = move.data.pokeType.color;
    }

    public void AssignNone()
    {
        textName.text = "";
        textPP.text = "";
        imageType.enabled = false;
        imageCover.enabled = false;
        imageBackground.color = Color.grey;
    }
}
