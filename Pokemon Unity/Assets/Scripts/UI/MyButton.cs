using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MyButton : Button
{
    protected override void Awake()
    {
        MobileInput.Instance.Release(gameObject.name);
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        MobileInput.Instance.Press(gameObject.name);
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
        MobileInput.Instance.Release(gameObject.name);
    }
}
