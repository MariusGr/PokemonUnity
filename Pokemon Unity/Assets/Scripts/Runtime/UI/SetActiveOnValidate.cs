using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetActiveOnValidate : MonoBehaviour
{
    [SerializeField] private bool active;

    private void OnValidate() => gameObject.SetActive(active);
}
