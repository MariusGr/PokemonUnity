using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewRegion", menuName = "World/Region")]
public class RegionsData : ScriptableObject
{
    public string fullName;
    public AudioClip mainMusicTrack;
}
