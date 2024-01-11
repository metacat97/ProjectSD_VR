using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    #region 상수
    const int PLAYER_START_GOLD = 100;  //시작 골드
    #endregion

    #region 싱글톤 변수
    //싱글턴으로 관리한다.
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject gameObject = new GameObject("GameManager");
                instance = gameObject.AddComponent<GameManager>();
                DontDestroyOnLoad(gameObject);
            }
            return instance;
        }

        private set { instance = value; }
    }
    #endregion

    #region private 변수
    private int firstGold = 1000;    // 초기 재화
    private float StartTime = 0;    //시작시간
    private float EndTime = 0;      //종료시간
    private float updateInterval = 1.0f; // 1초 간격으로 업데이트하도록 설정
    private float timeSinceLastUpdate = 0.0f;
    private BackGroundPlayer backGroundPlayer;
    #endregion

    #region public 변수
    public PlayerState playerState = PlayerState.READY;     //현재 게임 상태=
    public Vector3 hitPosition;
    public int currentGold = 0;     // 보유중인 재화

    #endregion


    private void Awake()
    {
        backGroundPlayer = FindObjectOfType<BackGroundPlayer>();
    }

    // [이미정] 231013 재화 초기값 설정
    private void Start()
    {
        backGroundPlayer.ChangeSong(false);
    }

    // [이미정] 231016 raycast hitPosition 추가
    private void Update()
    {
        if (Aim.isChooseTower)
        {
            //KHJ 23.10.18 마우스 포지션을 VR 컨트롤러로 변경
            //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Ray ray = Camera.main.ScreenPointToRay(ARAVRInput.LHandDirection);
            RaycastHit hitPoint;
            if (Physics.Raycast(ray, out hitPoint, 10, LayerMask.GetMask("Floor")))  //50의 테스트 최대 거리입니다. 변경점 90000-> 50
            {
                if (hitPoint.transform.CompareTag("Ground"))
                {
                    hitPosition = hitPoint.point;
                }
            }
        }

        if (!CheckPlayingGame())
        {
            return;
        }

        // 매 프레임마다 deltaTime 값을 누적
        timeSinceLastUpdate += Time.deltaTime;

        // 추후 coroutine으로 변경 + 게임 시작 조건 추가
        // 설정한 간격(updateInterval)이 경과하면 변수를 증가시키고 timeSinceLastUpdate를 재설정
        if (timeSinceLastUpdate >= updateInterval)
        {
            currentGold += 10;
            timeSinceLastUpdate = 0.0f; // 재설정

            // [PSH] 231018 수정
            KHJUIManager.Instance?.ChangeCoinText();
        }
    }

    public void SetGold(int unitPrice)
    {
        currentGold = unitPrice;

        // [PSH] 231018 수정
        KHJUIManager.Instance?.ChangeCoinText();

    }

    // [이미정] 231013 유닛 구매시 재화 소모
    public void SubtractGold(int unitPrice)
    {
        currentGold -= unitPrice;
        Debug.Log("유닛 구매: " + currentGold);

        // [PSH] 231018 수정
        KHJUIManager.Instance?.ChangeCoinText();
    }

    // [이미정] 231013 재화 추가 버튼 누를 시
    public void OnAddGoldBtn()
    {
        currentGold += 50;
        Debug.Log("골드 50추가");

        // [PSH] 231018 수정
        KHJUIManager.Instance?.ChangeCoinText();
    }

    //플레이 타임 불러오기
    //1. 게임중에는 진행 시간을 불러온다
    //2. 게임종료하면 버틴 시간을 불러온다.
    public float GetPlayTime()
    {
        //플레이 or 상점
        if(playerState == PlayerState.PLAY || playerState == PlayerState.SHOP)
        {
            return Time.time - StartTime;
        }
        //준비 or 종료
        else
        {
            return EndTime;
        }
    }

    //게임 시작시 불러온다
    public void ReStartGame()
    {
        //플레이중에는 막는다.
        if (CheckPlayingGame())
        {
            return;
        }

        InitManager();

        //[SSC] 231019 괴수, 랜덤약점 초기화 추가
        //Golem.G_insance.Initilize();
        LuckyPointController.instance.Initialize();
        Golem.G_insance.GolemStart();
        PlayerBase.instance.InitRestart();
        FindObjectOfType<PreviewBase>()?.HideAll();
        backGroundPlayer.ChangeSong(true);
    }

    //게임 시작시 불러온다
    public void StartGame()
    {
        //플레이중에는 막는다.
        if (CheckPlayingGame())
        {
            return;
        }

        InitManager();

        //[SSC] 231019 괴수행동시작 추가
        Golem.G_insance.GolemStart();
        PlayerBase.instance.InitRestart();
        FindObjectOfType<PreviewBase>()?.HideAll();
        backGroundPlayer.ChangeSong(true);
    }

    //게임 종료시 불러온다
    public void EndGame()
    {
        //플레이중이 아니면 막는다.
        if (!CheckPlayingGame())
        {
            return;
        }
        StopManager();
        Aim.isChooseTower = false;
        PlayerBase.instance.ChangeHand(true);
        FindObjectOfType<PreviewBase>()?.HideAll();
        // [SSC] 231020 플레이어 사망시 골렘 동작멈춤 추가
        Golem.G_insance.GolemStop();
        backGroundPlayer.ChangeSong(false);

        ////KHJ 231023 골렘 동작 멈춤 이후 승리 사운드 출력위해 임시 추가
        //KHJUIManager.Instance.VictoryGameSound();
    }


    //게임중인지 아닌지 판단
    public bool CheckPlayingGame()
    {
        return playerState == PlayerState.PLAY || playerState == PlayerState.SHOP;
    }

    //재시작시 내부 적용
    private void InitManager()
    {
        StartTime = Time.time;
        SetGold( PLAYER_START_GOLD);
        playerState = PlayerState.PLAY;
    }


    //종료시 내부 적용
    private void StopManager()
    {
        EndTime = GetPlayTime();
        playerState = PlayerState.DEAD;
    }
}

//플레이어 상태를 나타내는 enum
public enum PlayerState
{
    READY, PLAY, SHOP, DEAD
}