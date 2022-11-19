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

    private void Awake()
    {
        positionStart = transform.localPosition;
        rotationStart = transform.localRotation;
        scaleStart = transform.localScale;
    }

    public override void Select()
    {
        transform.position = position;
        transform.rotation = Quaternion.Euler(rotation);
        transform.localScale = scale;
        base.Select();
    }

    public override void Deselect()
    {
        transform.position = positionStart;
        transform.rotation = rotationStart;
        transform.localScale = scaleStart;
        base.Deselect();
    }
}
