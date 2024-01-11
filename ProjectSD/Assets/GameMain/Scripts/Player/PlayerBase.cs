using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBase : MonoBehaviour, IHitObject
{
    public static PlayerBase instance;

    //0 : left 1: right
    private AudioSource audioSource;
    [SerializeField]
    private GunBase[] gun;
    [SerializeField]
    private Aim[] hand;
    [SerializeField]
    private OVRScreenFade bloodEffect;
    [SerializeField]
    private GameObject centerCamera;


    public PlayerStatus status;
    public Transform bulletPool;

    private bool canEffect = true;
    private float maxHP = 100;
    private const float VIBRATION_TIME = 0.2f;
    private const float VIBRATION_FREQUENCY = 10F;
    private const float VIBRATION_AMPLITUDE = 2F;
    private const float EFFECT_TIME = 1.5F;

    private void Awake()
    {
        Init();
    }

    private void Update()
    {
        if ((ARAVRInput.GetDown(ARAVRInput.Button.One, ARAVRInput.Controller.RTouch)||
             ARAVRInput.GetDown(ARAVRInput.Button.Two, ARAVRInput.Controller.RTouch)||
             ARAVRInput.GetDown(ARAVRInput.Button.One, ARAVRInput.Controller.LTouch)||
             ARAVRInput.GetDown(ARAVRInput.Button.Two, ARAVRInput.Controller.LTouch))
             && GameManager.Instance.CheckPlayingGame() && Aim.isChooseTower == false) 
        {
            if(GameManager.Instance.playerState == PlayerState.PLAY)
            {
                GameManager.Instance.playerState = PlayerState.SHOP; 
                KHJUIManager.Instance.OpenShop();
                ChangeHand(true);

                //SHOP UI로 넘어감
            }
            else if (GameManager.Instance.playerState == PlayerState.SHOP)
            {
                GameManager.Instance.playerState = PlayerState.PLAY; 
                KHJUIManager.Instance.CloseShop();
                ChangeHand(false);

                //PLAY UI로 넘어감
            }
        }


    }

    public void ChangeHand(bool isHand)
    {
        gun[0].gameObject.SetActive(!isHand);
        gun[1].gameObject.SetActive(!isHand);
        if (!isHand)
        {
            gun[0].ResetSetting();
            gun[1].ResetSetting();
        }
        hand[0].gameObject.SetActive(isHand);
        hand[1].gameObject.SetActive(isHand);

    }

    private void Init()
    {
        if (instance == null)
        {
            instance = this;
        }


        audioSource = GetComponent<AudioSource>();

        InitRestart();
    }

    public void InitRestart()
    {
        maxHP = status.health;

        //PlayerStatus originStatus = Resources.Load("/"+status.name) as PlayerStatus;
        //status = Instantiate(originStatus);

        status = Instantiate(status);

        status.health = 100;
        maxHP = status.health;
        if (GameManager.Instance.playerState != PlayerState.READY)
        {
            ChangeHand(false);
        }
        //{KHJ추가
        //10.20 KHJ 변경사항 : ChangeHpText (100) -> (100, 100)으로 변경
        KHJUIManager.Instance.ChangeHpText(status.health,maxHP);
        //}KHJ 추가
        bloodEffect.transform.position = Vector3.down * -1000;
        Invoke("SetBloodEffect", 0.5f);

        bulletPool = new GameObject("BulletManager").transform;
    }


    private void SetBloodEffect()
    {

        bloodEffect.transform.SetParent(centerCamera.transform);
        bloodEffect.transform.localPosition = Vector3.zero;
        bloodEffect.transform.localRotation = Quaternion.identity;
    }

    public void Hit(float damage)
    {
        if (!GameManager.Instance.CheckPlayingGame())
            return;

        status.health -= (int)Mathf.Round(damage);
        HitReaction();
        if (status.health <= 0)
        {
            GameManager.Instance.EndGame();
            Invoke("Die", 2);
        }
    }

    private void Die()
    {
        if (bulletPool != null)
        {
            Destroy(bulletPool.gameObject);
        }

        //이 밑으로 
        //KHJUIManager.Instance.DefeatGameSound(); //OnGameOver에 합쳤습니다.
        KHJUIManager.Instance.OnGameOver();
    }

    private void HitReaction()
    {
        //피격처리
        ARAVRInput.PlayVibration(VIBRATION_TIME, VIBRATION_FREQUENCY, VIBRATION_AMPLITUDE, ARAVRInput.Controller.RTouch);
        ARAVRInput.PlayVibration(VIBRATION_TIME, VIBRATION_FREQUENCY, VIBRATION_AMPLITUDE, ARAVRInput.Controller.LTouch);
        if(audioSource!=null && audioSource.clip!=null)
        {
            audioSource.Play();
        }
        if (canEffect)
        {
            StartCoroutine(DelayEffectRoutine());
            bloodEffect.fadeTime = EFFECT_TIME;
            bloodEffect.FadeIn();
        }
        KHJUIManager.Instance?.ChangeHpText(status.health, maxHP);
    }

    IEnumerator DelayEffectRoutine()
    {
        canEffect = false;
        yield return new WaitForSeconds(EFFECT_TIME);
        canEffect = true;
    }

    public void EnhanceGun(bool enhance)
    {
        gun[0].ChangeWeaponMode(enhance);
        gun[1].ChangeWeaponMode(enhance);
    }
}
