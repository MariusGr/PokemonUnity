using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectableTransform : SelectableUIElement
{
    [SerializeField] private Vector3 position = Vector3.zero;
    [SerializeField] private Vector3 rotation = Vector3.zero;
    [SerializeField] private Vector3 scale = Vector3.one;

    private Vector3 positionStart;
    private Quaternion rotationStart;
    private Vector3 scaleStart;

    public override void Initialize(int index)
    {
        base.Initialize(index);
        positionStart = transform.localPosition;
        rotationStart = transform.localRotation;
        scaleStart = transform.localScale;
    }

    public override void Select()
    {
        transform.localPosition = positionStart + position;
        transform.rotation = Quaternion.Euler(rotationStart.eulerAngles + rotation);
        transform.localScale = scaleStart + scale;
        base.Select();
    }

    public override void Deselect()
    {
        transform.localPosition = positionStart;
        transform.rotation = rotationStart;
        transform.localScale = scaleStart;
        base.Deselect();
    }
}
