using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TestBullet : MonoBehaviour
{
    private Rigidbody myRigid = default;

    // Start is called before the first frame update
    void Start()
    {
        myRigid = GetComponent<Rigidbody>();
        myRigid.velocity = Camera.main.transform.forward * 80f;
    }

    private void OnTriggerEnter(Collider other)
    {
        // 몬스터들의 히트 포인트에 총알이 닿으면
        if(other.CompareTag("HitPoint"))
        {
            GameObject obj = other.attachedRigidbody.gameObject;
            obj.GetComponent<IHitObject>().Hit(50f);


            // ============ TODO : 개발일지 혹은 기술문서에 개선사항으로 기술하기 ================

            //// 해당 게임오브젝트를 담고
            //GameObject tempObj = other.gameObject;

            //// 해당 게임오브젝트에 IDamage 인터페이스를 체크하고 (몬스터마다 최상위 스크립트에 있음) 
            //while(tempObj.GetComponent<IHitObject>() == null)
            //{
            //    // 없다면 부모 게임오브젝트를 담는다
            //    // 이를 IDamage 찾을때까지 반복
            //    tempObj = tempObj.transform.parent.gameObject;
            //}

            //// IDamage인터페이스 게임오브젝트를 찾으면 데미지 메소드 실행
            //tempObj.GetComponent<IHitObject>().Hit(10f);

            // ============ TODO : 개발일지 혹은 기술문서에 개선사항으로 기술하기 ================

            Destroy(this.gameObject);
        }

        if (other.CompareTag("LuckyPoint"))
        {

            // 접촉한 오브젝트에 연결된 리지드바디 오브젝트를 담고 (이때 담기는 오브젝트는 최상위 몬스터 오브젝트임)
            GameObject obj = other.attachedRigidbody.gameObject;

            // 약점위치 변경하는 메소드 실행 (이때 접촉한 약점 게임오브젝트를 매개변수로 보내줘야함)
            obj.GetComponent<LuckyPointController>().ChangePoint(other.gameObject.transform.parent.gameObject);
            obj.GetComponent<IHitObject>().Hit(50f * 1.5f);
            Debug.Log(other.gameObject.transform.parent.gameObject);

            Destroy(this.gameObject);


            // ============ TODO : 개발일지 혹은 기술문서에 개선사항으로 기술하기 ================

            //// 해당 게임오브젝트를 담고
            //GameObject tempObj = other.gameObject;

            //while (tempObj.GetComponent<LuckyPointController>() == null)
            //{
            //    // 없다면 부모 게임오브젝트를 담는다
            //    // 이를 IDamage 찾을때까지 반복
            //    tempObj = tempObj.transform.parent.gameObject;
            //}

            //// 약점 변경 메소드 실행 및 데미지 처리
            //tempObj.GetComponent<LuckyPointController>().ChangePoint(other.gameObject);
            //tempObj.GetComponent<IHitObject>().Hit(10f * 1.5f);

            // ============ TODO : 개발일지 혹은 기술문서에 개선사항으로 기술하기 ================

        }
    }
}
