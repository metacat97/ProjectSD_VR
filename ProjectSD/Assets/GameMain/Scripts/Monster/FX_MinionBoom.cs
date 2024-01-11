using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FX_MinionBoom : MonoBehaviour
{
    //private ParticleSystem myParticle;

    //private void Start()
    //{
    //    myParticle = GetComponent<ParticleSystem>();
    //}

    private void OnEnable()
    {
        StartCoroutine(Coolobj());
    }

    IEnumerator Coolobj()
    {
        yield return new WaitForSeconds(1f);

        ObjectPoolManager.instance.CoolObj(this.gameObject, PoolObjType.BOOM_DIE);
    }
}
