using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Region : MonoBehaviour
{
    public RegionData data;

    void OnTriggerEnter(Collider other)
    {
        RegionManager.Instance.PlayerEnterRegion(data);
    }
}
