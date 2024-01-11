using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GunBulletStatus", menuName ="Scriptable Object/PlayerBullet")]
public class GunBulletStatus : ScriptableObject
{
    public int ID;
    public string context;

    public float fireRate;
    public float bulletDamage;
    public float bulletSpeed;

    //public float criticalChance;
    //public float criticalMultiple;

    public float criticalRate;

    public float lifeTime;

}
