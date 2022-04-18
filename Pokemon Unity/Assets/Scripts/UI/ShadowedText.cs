using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShadowedText : MonoBehaviour
{
    [SerializeField] Text _text;
    [SerializeField] Text textShadow;

    public string text
    {
        get => _text.text;
        set {
            _text.text = value;
            textShadow.text = value;
        }
    }
}
