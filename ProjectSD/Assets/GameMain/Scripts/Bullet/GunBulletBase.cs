using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunBulletBase : MonoBehaviour
{
    [SerializeField]
    protected GunBulletStatus status;
    protected Rigidbody bulletRigidbody;
    protected Collider bulletCollider;

    private AudioSource bulletAudioSource;
    public GameObject bulletParticle;

    private bool isAttack = false;
    public float bulletSpeed = 1f;

    [SerializeField]
    private DamageText damageText;

    public float textSize = 0.1f;

    private void Awake()
    {
        Init();
    }

    protected void Init()
    {
        status = Instantiate(status);
        bulletAudioSource = GetComponent<AudioSource>();
        bulletRigidbody = GetComponent<Rigidbody>();
        bulletCollider = GetComponent<Collider>();
        bulletRigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;
        bulletCollider.isTrigger = true;
        Invoke("AutoRemove", status.lifeTime);
    }

    public void Move(Vector3 direction)
    {
        bulletRigidbody.velocity = direction * status.bulletSpeed;
    }
    public void Trans(Vector3 direction, Vector3 point, float time)
    {
        StartCoroutine(moveRoutine(direction, point, time));
    }

    IEnumerator moveRoutine(Vector3 direction, Vector3 point, float time)
    {
        yield return new WaitForSeconds(time/ status.bulletSpeed);
        transform.position = point - direction * (time / status.bulletSpeed);
        Move(direction);
    }

    protected void Remove()
    {
        transform.localScale = Vector3.zero;
        Destroy(gameObject, 1f);
    }

    protected void AttackReaction(int damage)
    {

        //GameObject effect = Instantiate(bulletParticle, transform.position, Quaternion.Euler(-transform.forward));
        PlayParticle();

        damageText.gameObject.SetActive(true);
        GameObject obj = new GameObject();
        obj.transform.position = damageText.transform.position;
        damageText.transform.SetParent(obj.transform);
        damageText.SetDamage(damage);
        float distance = Vector3.Distance(transform.position, PlayerBase.instance.transform.position) ;
        //damageText.transform.localScale = damageText.transform.localScale * textSize * (Mathf.Abs(distance)+1);
        damageText.SetTextSize(distance);

        isAttack = true;
        bulletRigidbody.velocity = Vector3.zero;
        transform.localScale = Vector3.zero;

        ARAVRInput.PlayVibration(damage/500, damage/100, damage/100, ARAVRInput.Controller.LTouch);
        ARAVRInput.PlayVibration(ARAVRInput.Controller.RTouch);

        bulletAudioSource.Play();
        Remove();
    }

    public void PlayParticle()
    {
        bulletParticle.transform.SetParent(null);
        bulletParticle.SetActive(true);
        bulletParticle.GetComponent<ParticleSystem>().Play();
    }

    protected void AutoRemove()
    {
        if (!isAttack)
        {
            Remove();
        }
    }

    protected float GetDamage(bool isCritical)
    {
        /*
        float criticalChance = Random.Range(0f,100f);
        float criticalMultiple = 1;
        if(criticalChance<= status.criticalChance)
        {
            criticalMultiple = status.criticalMultiple;
        }

        return status.bulletDamage*(1+criticalMultiple);
        */

        if (isCritical)
        {
            return status.bulletDamage * (1 + status.criticalRate);
        }
        else
        {
            return status.bulletDamage;
        }
    }

    public float GetRate()
    {
        return status.fireRate;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody == null)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Floor"))
            {
                PlayParticle();
                Remove();
            }
            return;
        }

        IHitObject enemy = other.attachedRigidbody.GetComponent<IHitObject>();

        if (enemy != null && !isAttack && (other.CompareTag("HitPoint") || other.CompareTag("LuckyPoint")))
        {
            int damage = (int)GetDamage(other.gameObject.CompareTag("LuckyPoint"));
            enemy.Hit(damage);

            if(other.CompareTag("LuckyPoint"))
            {
                GameObject obj = other.attachedRigidbody.gameObject;

                // 약점위치 변경하는 메소드 실행 (이때 접촉한 약점 게임오브젝트를 매개변수로 보내줘야함)
                // [SSC] 2023.10.19 매개변수 other.gameobject에서 부모 오브젝트로 변경
                obj.GetComponent<LuckyPointController>().ChangePoint(other.transform.parent.gameObject);

                GameManager.Instance.SubtractGold(-50);

            }

            AttackReaction(damage);
        }

    }
}
