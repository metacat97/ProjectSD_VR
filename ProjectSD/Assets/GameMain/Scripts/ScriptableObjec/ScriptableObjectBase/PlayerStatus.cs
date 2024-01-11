using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStatus", menuName = "Scriptable Object/Player")]
public class PlayerStatus : ScriptableObject
{

    public int ID;
    public string context;
    public int health;
    public int weaponID;
    public int weaponPowerID;

}
