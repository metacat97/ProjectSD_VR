using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float bulletSpeed = 50f;  // Bullet 속력
    private Rigidbody bulletRigidbody = default;    // Bullet Rigidbody 컴포넌트

    private bool isAttack = false;

    public DamageText damageText;
    public float lifeTime = 5f;
    public int damage = 10;

    private void Awake()
    {
        bulletRigidbody = GetComponent<Rigidbody>();    // Rigidbody 컴포넌트 할당
        bulletRigidbody.velocity = transform.forward * bulletSpeed; // 앞쪽 방향으로 날아가도록 속도 설정
        //damageText = GetComponentInChildren<DamageText>();

        Destroy(gameObject, lifeTime);    // 5초 뒤 Bullet 오브젝트 파괴
    }

    // Bullet의 트리거 충돌시
    private void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody == null)
        {
            Debug.Log(other.gameObject);
            return;
        }

        IHitObject enemy = other.attachedRigidbody.GetComponent<IHitObject>();

        if (enemy != null && !isAttack && (other.gameObject.layer == LayerMask.NameToLayer("Boss")))
        {

            if (other.CompareTag("LuckyPoint"))
            {
                GameObject obj = other.attachedRigidbody.gameObject;

                // 약점위치 변경하는 메소드 실행 (이때 접촉한 약점 게임오브젝트를 매개변수로 보내줘야함)
                // [SSC] 2023.10.19 매개변수 other.gameobject에서 부모 오브젝트로 변경
                obj.GetComponent<LuckyPointController>().ChangePoint(other.transform.parent.gameObject);
                enemy.Hit(damage);

            }
            
            else
            {
                enemy.Hit(damage);
            }

            AttackReaction(damage);
        }
    }


    public void Move(Vector3 direction)
    {

        bulletRigidbody.velocity = direction * bulletSpeed;
    }

    protected void AttackReaction(int damage)
    {

        damageText.gameObject.SetActive(true);
        GameObject obj = new GameObject();
        obj.transform.position = damageText.transform.position;
        damageText.transform.SetParent(obj.transform);
        damageText.SetDamage(damage);
        float distance = Vector3.Distance(transform.position, PlayerBase.instance.transform.position);
        //damageText.transform.localScale = damageText.transform.localScale * textSize * (Mathf.Abs(distance)+1);
        damageText.SetTextSize(distance);

        isAttack = true;
        bulletRigidbody.velocity = Vector3.zero;
        transform.localScale = Vector3.zero;

        ARAVRInput.PlayVibration(ARAVRInput.Controller.LTouch);
        ARAVRInput.PlayVibration(ARAVRInput.Controller.RTouch);

        //bulletAudioSource.Play();
        //bulletParticle.Play();
        Remove();
    }
    protected void Remove()
    {
        transform.localScale = Vector3.zero;
        Destroy(gameObject, 1f);
    }

}
