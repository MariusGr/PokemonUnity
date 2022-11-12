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

    private bool isPlayingAnimation = false;
    private void Refresh() => image.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width * _value);
    public void SetValueAnimated(float value, float speed) => StartCoroutine(AnimateStatBarCoroutine(value, speed));

    IEnumerator AnimateStatBarCoroutine(float target, float speed)
    {
        print($"Value {Value} target {target}");
        isPlayingAnimation = true;
        float step = Mathf.Sign(target - Value) * speed;
        System.Func<bool> targetNotYetReached;
        if (Value > target)
            targetNotYetReached = new System.Func<bool>(() => Value > target);
        else
            targetNotYetReached = new System.Func<bool>(() => Value < target);

        while (targetNotYetReached())
        {
            print($"Value {Value} target {target}");
            Value += step;
            yield return new WaitForFixedUpdate();
        }
        Value = target;
        isPlayingAnimation = false;
    }
    public bool IsPlayingAnimation() => isPlayingAnimation;

#if (UNITY_EDITOR)
    private void Update() => Refresh();
#endif
}
