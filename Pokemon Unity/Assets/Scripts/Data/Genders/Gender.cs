using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Gender", menuName = "Pokemon/Gender")]
public class Gender : ScriptableObject
{
    public static Gender GetRandomGender() => Globals.Instance.genders[Random.Range(0, Globals.Instance.genders.Length)];

    public string symbol;
    public string fullName;
}
