using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GunStatus", menuName = "Scriptable Object/Gun")]
public class GunStatus : ScriptableObject
{
    public int ID;
    public string context;
    public float fireRate;
    public int bulletID;
}
