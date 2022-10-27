using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class ShadowedText : MonoBehaviour
{
    [SerializeField] Text textDefault;
    [SerializeField] Text textShadow;
    [SerializeField] string _text = "Enter text here";
    [SerializeField] TextAnchor _alignment = TextAnchor.UpperCenter;
    [SerializeField] Font _font;
    [SerializeField] bool useDifferentFontForShadow;
    [SerializeField] Font shadowFont;

    public string text
    {
        get => _text;
        set {
            _text = value;
            textDefault.text = _text;
            textShadow.text = _text;
        }
    }

    public TextAnchor alignment
    {
        get => _alignment;
        set
        {
            _alignment = value;
            textDefault.alignment = _alignment;
            textShadow.alignment = _alignment;
        }
    }

    public Font font
    {
        get => _font;
        set
        {
            _font = value;
            textDefault.font = _font;
            textShadow.font = useDifferentFontForShadow ? shadowFont : _font;
        }
    }

#if (UNITY_EDITOR)
    private void Update()
    {
        text = _text;
        alignment = _alignment;
        font = _font;
        textShadow.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, textDefault.rectTransform.sizeDelta.x);
        textShadow.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, textDefault.rectTransform.sizeDelta.y);
    }
#endif
}
