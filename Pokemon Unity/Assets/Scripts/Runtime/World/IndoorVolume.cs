using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndoorVolume : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) => DayNightCycle.Instance.EnterIndoorArea();
    private void OnTriggerExit(Collider other) => DayNightCycle.Instance.ExitIndoorArea();
}
