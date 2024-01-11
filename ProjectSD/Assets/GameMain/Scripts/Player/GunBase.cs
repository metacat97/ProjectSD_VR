using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunBase : MonoBehaviour
{
    public GunBulletBase defaultBullet;
    public GunBulletBase enhanceBullet;
    public HandPosition handPosition;
    public LaserPoint point;
    public GunStatus status;

    public Material originMaterial;
    public Material hologramMaterial;

    private AudioSource gunAudioSource;
    private ParticleSystem gunParticle;
    private ARAVRInput.Controller controller;
    private Renderer gunRenderer;
    private bool isEnhance = false;
    private bool canShot = true;




    private const float VIBRATION_TIME = 0.1f;
    private const float VIBRATION_FREQUENCY = 5F;
    private const float VIBRATION_AMPLITUDE = 5F;
    private const float RAYCAST_DISTANCE = 50F;

    private void Awake()
    {
        Init();
    }

    private void Update()
    {
        if (GameManager.Instance.playerState != PlayerState.PLAY)
        {
            return;
        }

        if ((ARAVRInput.GetDown(ARAVRInput.Button.IndexTrigger, controller) ||
            ARAVRInput.Get(ARAVRInput.Button.IndexTrigger, controller)))
        {
            Shot();
        }
    }

    private void Init()
    {   
        gunAudioSource = GetComponent<AudioSource>();
        gunParticle = GetComponentInChildren<ParticleSystem>();
        gunRenderer = GetComponentInChildren<Renderer>();

        if (handPosition == HandPosition.RIGHT)
        {
            controller = ARAVRInput.Controller.RTouch;
        }
        else
        {
            controller = ARAVRInput.Controller.LTouch;
        }
        isEnhance = false;
        ResetSetting();
    }

    public void ResetSetting()
    {
        canShot = true;
        
    }

    public void Shot()
    {

        if (!canShot)
        {
            return;
        }


        GunBulletBase currBullet;
        Vector3 direction;
        if (handPosition == HandPosition.RIGHT)
        {
            direction = ARAVRInput.RHandDirection;
        }
        else
        {
            direction = ARAVRInput.LHandDirection;
        }

        if (!isEnhance)
        {
            currBullet = Instantiate(defaultBullet, point.startPos.position, Quaternion.identity, PlayerBase.instance.bulletPool);
        }
        else
        {
            currBullet = Instantiate(enhanceBullet, point.startPos.position, Quaternion.identity, PlayerBase.instance.bulletPool);
        }
        RaycastHit hit;
        int layer = GlobalFunction.GetLayerMask("Floor", "Boss", "BossBullet", "Monster");

        currBullet.transform.up = direction;
        if (Physics.Raycast(new Ray(point.startPos.position, direction), RAYCAST_DISTANCE, ~layer) &&
            Physics.Raycast(new Ray(point.startPos.position, direction), out hit, RAYCAST_DISTANCE, GlobalFunction.GetLayerMask("Floor")))
        {
            currBullet.Trans(direction, hit.point, hit.distance);
            Debug.Log("=충돌");
        }
        else
        {
            currBullet.Move(direction);
        }

        AttackReaction();

        StartCoroutine(GunDelayRoutine(currBullet.GetRate()));
    }

    private void AttackReaction()
    {
        ARAVRInput.PlayVibration(VIBRATION_TIME, VIBRATION_FREQUENCY, VIBRATION_AMPLITUDE, controller);
        gunAudioSource.Play();
        gunParticle.Play();
    }

    private IEnumerator GunDelayRoutine(float time)
    {
        canShot = false;
        yield return new WaitForSeconds(time);
        canShot = true;
    }

    public void ChangeWeaponMode(bool enable)
    {
        isEnhance = enable;
        if (enable)
        {
            gunRenderer.material = hologramMaterial;
        }
        else
        {
            gunRenderer.material = originMaterial;
        }
    }

}
