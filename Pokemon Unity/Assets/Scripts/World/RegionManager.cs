using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegionManager : MonoBehaviour, IRegionManager
{
    public static RegionManager Instance;

    public IRegionData currentRegion { get; private set; }
    private IRegionUI ui;

    public RegionManager()
    {
        Instance = this;
        Services.Register(this as IRegionManager);
    }

    private void Awake()
    {
        ui = Services.Get<IRegionUI>();
    }

    public void PlayerEnterRegion(IRegionData region)
    {
        currentRegion = region;
        BgmHandler.Instance.PlayMain(currentRegion.MainMusicTrack);
        ui.ShowRegionName(currentRegion.Name);
    }
}
