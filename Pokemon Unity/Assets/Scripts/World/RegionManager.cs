using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegionManager : MonoBehaviour, IService
{
    public static RegionManager Instance;

    public RegionsData currentRegion { get; private set; }
    
    public RegionManager() => Instance = this;

    public void PlayerEnterRegion(RegionsData region)
    {
        currentRegion = region;
        BgmHandler.Instance.PlayMain(currentRegion.mainMusicTrack);
    }
}
