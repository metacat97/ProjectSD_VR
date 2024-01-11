using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LaserPoint : MonoBehaviour
{
    #region 상수
    const float LASER_LENGTH = 1000;
    #endregion

    #region private 변수
    private LineRenderer lineRenderer;
    ARAVRInput.Controller controller;
    #endregion

    #region public 변수
    public HandPosition handPosition;
    public GameObject hitPointer;
    public Material[] laserMaterials;
    public Transform startPos;
    #endregion


    void Awake()
    {
        Init();
    }

    void Update()
    {
        //플레이 모드가 아니면 비활성화
        if(!GameManager.Instance.CheckPlayingGame())
        {
            return;
        }

        Vector3 _startPos = startPos.position;
        Vector3 _endPos = startPos.position;

        //변수에 값 저장
        ChangeControllerData(out _startPos, out _endPos);

        Ray ray = new Ray(_startPos, _endPos);
        RaycastHit hit;

        //레이캐스트
        //ray와 충돌할 Layer설정
        int colliderMask = GlobalFunction.GetLayerMask("Floor", "Monster", "Boss", "BossBullet");
        //Debug.Log(colliderMask);
        if (Physics.Raycast(ray, out hit, LASER_LENGTH, colliderMask))
        {
            //조준점 보임
            hitPointer.SetActive(true);
            //조준점 거리에 비례해서 크기조절
            hitPointer.transform.localScale = Vector3.one * Vector3.Distance(_startPos, hit.point) * 0.01f;
            //조준점 위치 변경
            hitPointer.transform.position = hit.point;
            //lineRenderer의 끝부분 위치 변경
            _endPos = hit.point;
        }
        else if(hitPointer.activeSelf)
        {
            hitPointer.SetActive(false);
        }

        //조준선 색 변경
        if (ARAVRInput.Get(ARAVRInput.Button.IndexTrigger, controller))
        {
            lineRenderer.material.ChangeMaterialColor(laserMaterials[(int)LaserColor.SHOT].color);
        }
        else if (hit.collider != null)
        {
            lineRenderer.material.ChangeMaterialColor(laserMaterials[(int)LaserColor.TARGET].color);
        }
        else if (hit.collider==null || ARAVRInput.GetUp(ARAVRInput.Button.IndexTrigger, controller))
        {
            lineRenderer.material.ChangeMaterialColor(laserMaterials[(int)LaserColor.IDLE].color);
        }

        //조준선 길이 설정
        lineRenderer.SetPosition(0, _startPos);
        lineRenderer.SetPosition(1, _endPos);

    }

    private void Init()
    {
        if (handPosition == HandPosition.RIGHT)
        {
            controller = ARAVRInput.Controller.RTouch;
        }
        else
        {
            controller = ARAVRInput.Controller.LTouch;
        }

        hitPointer = Instantiate(hitPointer, transform);
        lineRenderer = GetComponentInChildren<LineRenderer>();

        lineRenderer.material = laserMaterials[0];
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = 0.005f;
        lineRenderer.endWidth = 0.015f; 
    }

    //입력 값에 따라서 값 변경
    private void ChangeControllerData(out Vector3 startPos, out Vector3 endPos)
    {
        Transform gunHead = this.startPos;

        if (handPosition == HandPosition.RIGHT)
        {
            //startPos = ARAVRInput.RHandPosition;
            //endPos = startPos + (ARAVRInput.RHandDirection * LASER_LENGTH);
            startPos = gunHead.position;
            endPos = startPos + gunHead.forward * LASER_LENGTH;
            return;
        }

        else if (handPosition == HandPosition.LEFT)
        {
            //startPos = ARAVRInput.LHandPosition;
            //endPos = startPos + (ARAVRInput.LHandDirection * LASER_LENGTH);
            startPos = gunHead.position;
            endPos = startPos + gunHead.forward * LASER_LENGTH;
            return;
        }

        else
        {
            startPos = transform.position;
            endPos = transform.position;
            return;
        }
    }


    public enum LaserColor : int
    {
        IDLE = 0,
        TARGET,
        SHOT
    }
}
public enum HandPosition
{
    LEFT = 0,
    RIGHT=1
}
