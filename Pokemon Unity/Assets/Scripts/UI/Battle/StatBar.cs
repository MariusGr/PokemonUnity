using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class StatBar : MonoBehaviour
{
    [SerializeField] float width = 1f;
    [SerializeField] Image image;
    [SerializeField] float _value = 1f;

    public float Value
    {
        get => _value;
        set
        {
            _value = value;
            Refresh();
        }
    }

    private void Refresh() => image.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width * _value);

#if (UNITY_EDITOR)
    private void Update() => Refresh();
#endif
}
