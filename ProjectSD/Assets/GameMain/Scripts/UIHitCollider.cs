using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class UIHitCollider : MonoBehaviour
{
    public UnityEvent OnHit;
    //public UnityEvent HitUp;
    public void HitUI()
    {
       
        OnHit?.Invoke();
        //gameObject.SetActive(false);

    }
    //public void UpHitUi()
    //{
    //    HitUp?.Invoke();
    //}
}
