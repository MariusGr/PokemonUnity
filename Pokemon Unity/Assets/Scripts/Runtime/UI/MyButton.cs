using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MyButton : Button
{
    protected override void Awake()
    {
        UIInput.Instance.Release(gameObject.name);
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        UIInput.Instance.Press(gameObject.name);
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
        UIInput.Instance.Release(gameObject.name);
    }
}
