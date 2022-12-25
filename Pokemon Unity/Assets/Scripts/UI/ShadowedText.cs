using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class ShadowedText : MonoBehaviour
{
    [SerializeField] RectTransform parentRect;
    [SerializeField] Text textDefault;
    [SerializeField] Text textShadowDefault;
    [SerializeField] Text textSmall;
    [SerializeField] Text textShadowSmall;
    [SerializeField] float shadowOffset = .5f;
    [SerializeField] string _text = "Enter text here";
    [SerializeField] TextAnchor _alignment = TextAnchor.UpperCenter;
    [SerializeField] Font _font;
    [SerializeField] int _fontSize = 13;
    [SerializeField] float _lineSpacing;
    [SerializeField] bool useDifferentFontForShadow;
    [SerializeField] Font shadowFont;
    [SerializeField] HorizontalWrapMode _horizontalWrapMode;

    Text textField => smallText ? textSmall : textDefault;
    Text textFieldShadow => smallText ? textShadowSmall : textShadowDefault;

    private float textWidth => LayoutUtility.GetPreferredWidth(textDefault.rectTransform); //This is the width the text would LIKE to be
    private float parentWidth => parentRect.rect.width; //This is the actual width of the text's parent container

    bool smallText = false;

    public string text
    {
        get => _text;
        set {
            _text = value;
            string textMultiLine = TextKeyManager.PlaceNewLineChars(_text);
            textDefault.text = textMultiLine;
            textShadowDefault.text = textMultiLine;
            if(!smallText && TextIsTooWide())
            {
                smallText = true;
                Refresh();
            }
            else if (smallText)
            {
                textSmall.text = textMultiLine;
                textShadowSmall.text = textMultiLine;

                if (TextIsTooNarrow())
                {
                    smallText = false;
                    Refresh();
                }
            }
        }
    }

    public TextAnchor alignment
    {
        get => _alignment;
        set
        {
            _alignment = value;
            textField.alignment = _alignment;
            textFieldShadow.alignment = _alignment;
        }
    }

    public Font font
    {
        get => _font;
        set
        {
            _font = value;
            textField.font = _font;
            textFieldShadow.font = useDifferentFontForShadow ? shadowFont : _font;
        }
    }

    public int fontSize
    {
        get => _fontSize;
        set
        {
            _fontSize = value;
            textDefault.fontSize = _fontSize;
            textShadowDefault.fontSize = _fontSize;
        }
    }

    public float linespacing
    {
        get => _lineSpacing;
        set
        {
            _lineSpacing = value;
            textField.lineSpacing = _lineSpacing;
            textFieldShadow.lineSpacing = _lineSpacing;
        }
    }

    public HorizontalWrapMode horizontalWrapMode
    {
        get => _horizontalWrapMode;
        set
        {
            _horizontalWrapMode = horizontalWrapMode;
            textDefault.horizontalOverflow = _horizontalWrapMode;
            textShadowDefault.horizontalOverflow = _horizontalWrapMode;
        }
    }

    protected bool TextIsTooWide() => horizontalWrapMode != HorizontalWrapMode.Wrap && textWidth > parentWidth;
    protected bool TextIsTooNarrow() => horizontalWrapMode == HorizontalWrapMode.Wrap || textWidth <= parentWidth;

    private void SetTextVisible(Text text, bool visible)
    {
        text.color = new Color(text.color.r, text.color.g, text.color.b,
            visible ? 1f : 0);
    }

    private void Refresh()
    {
        SetTextVisible(textDefault, !smallText);
        SetTextVisible(textShadowDefault, !smallText);
        SetTextVisible(textSmall, smallText);
        SetTextVisible(textShadowSmall, smallText);

        alignment = _alignment;
        font = _font;
        fontSize = _fontSize;
        linespacing = _lineSpacing;
        horizontalWrapMode = _horizontalWrapMode;
    }

    //#if (UNITY_EDITOR)
    //    private void Update()
    //#else
    //    private void Start()
    //#endif
    private void Update()
    {
        text = _text;
        Refresh();

        textShadowDefault.rectTransform.position = textDefault.rectTransform.position + new Vector3(1f, -1f, 0) * shadowOffset;
        textShadowSmall.rectTransform.position = textSmall.rectTransform.position + new Vector3(1f, -1f, 0) * shadowOffset;

        //textShadowDefault.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, textField.rectTransform.sizeDelta.x);
        //textShadowDefault.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, textField.rectTransform.sizeDelta.y);
    }
}
