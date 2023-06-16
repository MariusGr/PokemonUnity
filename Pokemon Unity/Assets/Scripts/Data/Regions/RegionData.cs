using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewRegion", menuName = "World/Region")]
public class RegionData : ScriptableObject, IRegionData
{
    public string fullName;
    public string Name { get => fullName; }
    public AudioClip mainMusicTrack;
    public AudioClip MainMusicTrack { get => mainMusicTrack; }
}
