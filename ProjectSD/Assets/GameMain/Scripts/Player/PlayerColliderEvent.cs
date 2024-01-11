using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerColliderEvent : MonoBehaviour, IHitObject
{
    PlayerBase player;

    public void Hit(float damage)
    {
        player.Hit(damage);
    }

    private void Awake()
    {
        Init();
    }

    void Init()
    {
        player = transform.parent.GetComponent<PlayerBase>();
    }

}
