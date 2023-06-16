using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRegionManager : IService
{
    public void PlayerEnterRegion(IRegionData region);
    public IRegionData currentRegion { get; }
}
