
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class KHJUIManager : MonoBehaviour
{
    float currentTime = 0f;
    float clearTime = 2.0f;

    float sizeBuffCheck = 10f;
    float speedBuffCheck = 10f;

    //[SerializeField] private GameObject startCanvas;
    [SerializeField] private GameObject startPanel;
    [SerializeField] private UIHitCollider startHit;

    //[SerializeField] private GameObject endCanvas;
    [SerializeField] private GameObject endPanel;
    [SerializeField] private UIHitCollider endHit;

    [SerializeField] private GameObject restartPanel;
    [SerializeField] private UIHitCollider restartHit;

    //{플레이어 관련 UI
    //[SerializeField] private GameObject playerUi;
    [SerializeField] private GameObject pUiPivot; //playerUi 하위 패널입니다.

    [Header("TopPanel")]
    [SerializeField] private GameObject topPanel;
    
    //[SerializeField] private GameObject rightPanel; // 채팅과 버프 이미지 있는 패널

    [Header("LeftPanel")]
    [SerializeField] private GameObject leftPanel; //버프 이미지 있는 패널
    public GameObject buffPanel; //버프 패널 오브젝트
    [SerializeField] private GameObject sizeUpBuff;
    [SerializeField] private Image sizeUpImage;
    [SerializeField] private TMP_Text sizeUpText;

    [SerializeField] private GameObject speedUpBuff;
    [SerializeField] private Image speedUpImg;
    [SerializeField] private TMP_Text speedUpText;

    [Header("UnitDestroyMsg")]
    public TMP_Text[] chatText;  // 팝업 알림 텍스트 리스트 
    public GameObject msgPanel;

    //체력
    [SerializeField] private GameObject healthObj;
    [SerializeField] private Image currentHpImg; // 변동하는 이미지
    [SerializeField] private TMP_Text healthText; //체력 수치 텍스트
    //시간
    public GameObject timePanel;
    [SerializeField] private GameObject timeObj;
    public TMP_Text timeText; // 시간 수치 텍스트
    //}플레이어 관련 UI

    [Header("ShopCanvas")]
   
    [SerializeField] private GameObject shopPanel; //상점 캔버스 하위 패널
    public bool isOpenShop = false;
    //유닛 리스트
    
    private GameObject unitPanel;
    public List<GameObject> unitList = default;
    public bool isOnAim = false;

    //코인
    [SerializeField] private GameObject coinObj; 
    [SerializeField] private TMP_Text coinText; // 코인 수치 텍스트

    [Header("ResultCanvas")]
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private GameObject defeatPanel;
    //[SerializeField] private GameObject rTimeObj;
    //[SerializeField] private GameObject rCoinObj;
    //[SerializeField] private GameObject rKillObj;

    [Header("Boss")]
    [SerializeField] private GameObject bossPanel;
    [SerializeField] private GameObject bossHpObj;
    [SerializeField] private Image currentBossImg; // 변동하는 이미지
    [SerializeField] private TMP_Text bossHpText; //체력 수치 텍스트


    [Header("Sound")]
    //임시 사운드 
    public AudioSource uiAudioSource;
    
    public AudioClip enterGameSound;
    public AudioClip uiClickSound;
    public AudioClip uiVictorySound;
    public AudioClip uiDefeatSound;
    
    //[SerializeField] private AudioClip uiInGameBGSound;



    #region 싱글톤 변수
    //싱글턴으로 관리한다.
    private static KHJUIManager instance;
    public static KHJUIManager Instance
    {
        get
        {
            return instance;
        }

        private set { instance = value; }
    }
    #endregion
    // Start is called before the first frame update
    void Awake()
    {
        instance = this;

        //{시작 종료 재시작 캔버스
        GameObject startCanvas = GameObject.Find("StartCanvas");
        startPanel = startCanvas.transform.GetChild(0).gameObject;
        startHit = startPanel.GetComponent<UIHitCollider>();

        GameObject endCanvas = GameObject.Find("EndCanvas");
        endPanel = endCanvas.transform.GetChild(0).gameObject;
        endHit = endPanel.GetComponent<UIHitCollider>();

        GameObject restartCanvas = GameObject.Find("ReStartCanvas");
        restartPanel = restartCanvas.transform.GetChild(0).gameObject;
        restartHit = restartPanel.GetComponent<UIHitCollider>();
        //} 시작 종료 재시작 캔버스

        //{PlayerUICanvas 
        GameObject playerUi = GameObject.Find("PlayerUICanvas");
        pUiPivot = playerUi.transform.GetChild(0).gameObject;

        //leftPanel
        leftPanel = pUiPivot.transform.GetChild(0).gameObject;
        buffPanel = leftPanel.transform.GetChild(0).gameObject;

        sizeUpBuff = buffPanel.transform.GetChild(0).gameObject;
        sizeUpImage = sizeUpBuff.transform.GetChild(0).gameObject.GetComponent<Image>();
        sizeUpText = sizeUpBuff.transform.GetChild(1).gameObject.GetComponent<TMP_Text>();

        speedUpBuff = buffPanel.transform.GetChild(1).gameObject;
        speedUpImg = speedUpBuff.transform.GetChild(0).gameObject.GetComponent<Image>();
        speedUpText = speedUpBuff.transform.GetChild(1).gameObject.GetComponent<TMP_Text>();

        //topPanel
        topPanel = pUiPivot.transform.GetChild(1).gameObject;  
        //{체력
        healthObj = topPanel.transform.GetChild(1).gameObject;
        currentHpImg = healthObj.GetComponent<Image>();
        healthText = healthObj.transform.GetChild(0).gameObject.GetComponent<TMP_Text>();

        //코인
        GameObject coinPanel = topPanel.transform.GetChild(2).gameObject;
        coinObj = coinPanel.transform.GetChild(0).gameObject;
        coinText = coinObj.GetComponent<TMP_Text>();

        //}PlayerUICanvas 

        //{shopCanvas 관련 변수
        GameObject shopCanvas = GameObject.Find("ShopCanvas");
        shopPanel = shopCanvas.transform.GetChild(0).gameObject;

        unitPanel = shopPanel.transform.GetChild(0).gameObject;
       
        
        //코인 변수
        //GameObject coinPanel = shopPanel.transform.GetChild(1).gameObject;  
        //coinObj = coinPanel.transform.GetChild(0).gameObject;
        //coinText = coinObj.GetComponent<TMP_Text>();
        //}shopCanvas 관련 변수

        //{resultCanvas 관련 변수  나중에 쓰게 된다면 쓰면 됨
        GameObject resultCanvas = GameObject.Find("ResultCanvas");
        resultPanel = resultCanvas.transform.GetChild(0).gameObject;
        victoryPanel = resultPanel.transform.GetChild(0).gameObject;
        defeatPanel = resultPanel.transform.GetChild(1).gameObject;
        //rTimeObj = resultPanel.transform.GetChild(2).gameObject;
        //rCoinObj = resultPanel.transform.GetChild(3).gameObject;
        //rKillObj = resultPanel.transform.GetChild(4).gameObject;
        //}resultCanvas 관련 변수

        //시간 변수
        GameObject timeCanvas = GameObject.Find("TimeCanvas");
        timePanel = timeCanvas.transform.GetChild(0).gameObject;
        timeObj = timePanel.transform.GetChild(0).gameObject;
        timeText = timeObj.GetComponent<TMP_Text>();
        //boss 관련
        GameObject bossCanvas = GameObject.Find("BossCanvas");
        bossPanel = bossCanvas.transform.GetChild(0).gameObject;
        bossHpObj = bossPanel.transform.GetChild(1).gameObject;
        currentBossImg = bossHpObj.GetComponent<Image>();
        bossHpText = bossHpObj.transform.GetChild(0).gameObject.GetComponent<TMP_Text>();

        //버프 종료 알림 텍스트 관련
        GameObject msgCanvas = GameObject.Find("MsgCanvas");
        msgPanel = msgCanvas.transform.GetChild(0).gameObject;

        //사운드 관련 
        uiAudioSource = gameObject.GetComponent<AudioSource>();
        
    }
    private void Start()
    {
        startHit.OnHit.AddListener(() => ClickStartGame()); //함수 연결
        startHit.OnHit.AddListener(() => ClickSound());//클릭 사운드

        endHit.OnHit.AddListener(() => ClickExitGame());
        endHit.OnHit.AddListener(() => ClickSound());

        restartHit.OnHit.AddListener(() => ClickReStart());
        restartHit.OnHit.AddListener(() => ClickSound());


        for (int i = 0; i < unitPanel.GetComponentsInChildren<BuyUnit>().Length; i++)
        {
            unitList.Add(unitPanel.transform.GetChild(i).gameObject);
        }
        
        defeatPanel.SetActive(false);
        victoryPanel.SetActive(false);
        bossPanel.SetActive(false);
        restartPanel.SetActive(false);
        sizeUpBuff.SetActive(false);
        speedUpBuff.SetActive(false);
        //buffPanel.SetActive(false);
        pUiPivot.SetActive(false);
        shopPanel.SetActive(false);
        //resultPanel.SetActive(false);
        timePanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        //코루틴으로 뺼 예정 어떻게?
        ClearMsg();

        if(GameManager.Instance.CheckPlayingGame())
        {
            DisplayTime(GameManager.Instance.GetPlayTime());
        }
        CheckOneBuff();
        CheckTwoBuff();

    }
  
    public void SendMsg()
    {
        Debug.Log("잘되나요?");
        currentTime = 0f;
        PopUpMsg("TEST SEND");
    }

   
    public void ClearMsg()
    {
        currentTime += Time.deltaTime;
        if (currentTime > clearTime)
        {
            for (int i = 1; i < chatText.Length; i++)
            {
                chatText[i - 1].text = chatText[i].text;
            }
            chatText[chatText.Length - 1].text = "";

            currentTime = 0f;
        }
    } //메시지가 2초뒤에 사라질 수 있도록 하는 함수입니다.
    //}메시지 차에 띄울 함수입니다.
    public void ClickStartGame()
    {
        Debug.Log("게임을 시작합니다");
        startPanel.SetActive(false);
        endPanel.SetActive(false);
        pUiPivot.SetActive(true);
        bossPanel.SetActive(true);
        timePanel.SetActive(true);
        EnterGameSound();
        GameManager.Instance.StartGame();
    }//시작 함수는 완료
    public void ClickExitGame()
    {
        Debug.Log("게임을 종료합니다");
        Application.Quit();
    }//종료 버튼 함수도 완료

    public void OnGameOver()
    {
        if(victoryPanel.activeSelf == false)
        {
            DefeatGame();
        }
        //TODO 게임 오버 패널 켜주고 2초뒤 리스타트 버튼 활성화 혹은 
        //ShowResult();
        restartPanel.SetActive(true);
        endPanel.SetActive(true);
        CloseShop();
        
    }
    //결과창 띄우는 함수입니다.
    //private void ShowResult()
    //{
    //    Debug.Log("게임 결과창 출력");
    //    pUiPivot.SetActive(false);
    //    shopPanel.SetActive(false);
    //    resultPanel.SetActive(true);

    //    rTimeObj.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = string.Format("{0}", GameManager.Instance.GetPlayTime());
    //    rCoinObj.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = string.Format("{0}",coinText);
    //    rKillObj.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = "?";
    //}

    public void ClickReStart() //재시작 버튼을 누르면 활성화될 것들 혹은 비활성화 될 것들 해줍니다.
    {
        Debug.Log("게임 재시작 되나?");
        restartPanel.SetActive(false); //재시작 캔버스 패널
        //resultPanel.SetActive(false); // 결과찬 캔버스 패널
        endPanel.SetActive(false); //종료 버튼 
        victoryPanel.SetActive(false);
        defeatPanel.SetActive(false);
        pUiPivot.SetActive(true); // 플레이어 ui 관련 패널
        bossPanel.SetActive(true); //보스 체력 ui 패널 
        timePanel.SetActive (true);//시간 패널 
        msgPanel.SetActive(true);

        InitilizeBuff();
        FindUnitDestroy();
        EnterGameSound();
        
        GameManager.Instance.ReStartGame();
    }

    public void ChangeCoinText()
    {
        coinText.text = "" + GameManager.Instance.currentGold;
    }
   
    public void ChangeHpText(float pHp, float pMaxHp )
    {
        //TODO hp 추가를 해줘야 합니다.
        currentHpImg.fillAmount = pHp / pMaxHp;
        healthText.text = string.Format("{0} / {1}", pHp, pMaxHp); 
    }

    public void ChangeBossHpText(float bHp, float bMaxHp) //Boss hp
    {
        //TODO hp 추가를 해줘야 합니다.
        currentBossImg.fillAmount = bHp / bMaxHp;
        bossHpText.text = string.Format("{0} / {1}", bHp,bMaxHp); 
    }

    private void CheckOneBuff()
    {
        if (sizeUpBuff.activeSelf == true)
        {
            sizeBuffCheck -= Time.deltaTime;
            ChangeSizeUpBuff(sizeBuffCheck);
        }
    }

    private void CheckTwoBuff()
    {
        if (speedUpBuff.activeSelf == true) 
        {
            speedBuffCheck -= Time.deltaTime;
            ChangeSpeedUpBuff(speedBuffCheck);
        }
    }
    public void OnSizeBuff()
    {
        sizeUpBuff.SetActive(true);
    }
    public void OnSpeedBuff()
    {
        speedUpBuff.SetActive(true);
    }
    public void ChangeSizeUpBuff(float buffTime)
    {
        //sizeUpBuff.SetActive(true);
        sizeUpImage.fillAmount = buffTime / 10f;
        sizeUpText.text = string.Format("{0:0}", buffTime);
        if(buffTime <= 0)
        {
            sizeUpBuff.SetActive(false);
        }
    }
    public void ChangeSpeedUpBuff(float buffTime)
    {
        //speedUpBuff.SetActive(true);
        speedUpImg.fillAmount = buffTime / 10f;
        speedUpText.text= string.Format("{0:0}", buffTime);
        if(buffTime <= 0) 
        {
            speedUpBuff.SetActive(false);
        }
    }

    //시간 텍스트 00:00 형식으로 보여주는 함수 
    private void DisplayTime(float timeToDisplay)
    {
        timeToDisplay += 1;
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }


    //{메시지 창에 띄울 함수입니다.
    public void PopUpMsg(string msg)
    {
        bool isInput = false;
        for (int i = 0; i < chatText.Length; i++)
        {
            if (chatText[i].text == "")
            {
                isInput = true;
                chatText[i].text = msg;
                break;
            }
        }
        if (!isInput) // 꽉차면 한칸씩 위로 올림
        {
            for (int i = 1; i < chatText.Length; i++)
            {
                chatText[i - 1].text = chatText[i].text;
            }
            chatText[chatText.Length - 1].text = msg;
        }
    }
    //}메시지 창에 띄울 함수입니다.

    public void OpenShop() //상점을 켜줍니다.
    {
        shopPanel.SetActive(true);
        isOpenShop = true;
    }
    public void CloseShop()
    {
        shopPanel.SetActive(false);
        isOpenShop = false;
        Aim.actions();
    } //상점을 꺼줍니다.

    private void FindUnitDestroy()
    {

        UnitBase[] unitList = FindObjectsOfType<UnitBase>();
        foreach (var unit in unitList)
        {
            Destroy(unit.gameObject);
        }
    }

    public void InitilizeUI()
    {
        //완전히 처음부터 시킬건지 아니면 바로 시작시킬건지
        restartPanel.SetActive(false);
        startPanel.SetActive(true);
        endPanel.SetActive(true);
        pUiPivot.SetActive(false);
        shopPanel.SetActive(false);
        bossPanel.SetActive(false);
        timePanel.SetActive(false);
        msgPanel.SetActive(false);
    }

    public void InitilizeBuff()
    {
        sizeBuffCheck = 10f;
        speedBuffCheck = 10f;
        sizeUpImage.fillAmount = sizeBuffCheck;
        speedUpImg.fillAmount = speedBuffCheck;
        sizeUpBuff.SetActive(false);
        speedUpBuff.SetActive(false);

    }
    public void ResetSizeBuff()
    {
        sizeBuffCheck = 10f;
        sizeUpImage.fillAmount = sizeBuffCheck;
    }
    public void ResetSpeedBuff()
    {
        speedBuffCheck = 10f;
        speedUpImg.fillAmount = speedBuffCheck;
    }

    public void ClickSound() //TODO 나중에 사운드 매니저 생기면 옮길 예정
    {
        uiAudioSource.PlayOneShot(uiClickSound);
    }

    public void EnterGameSound()
    {
        uiAudioSource.PlayOneShot(enterGameSound);
    }


    public void VictoryGame()
    {
        VictoryGameSound();
        OnVictroyPanel();
        OnGameOver();
        bossPanel.SetActive(false);
        pUiPivot.SetActive(false);
        timePanel.SetActive(false);
        msgPanel.SetActive(false);
    }
    public void DefeatGame()
    {
        DefeatGameSound();
        OnDefeatPanel();
        bossPanel.SetActive(false);
        pUiPivot.SetActive(false);
        timePanel.SetActive(false);
        msgPanel.SetActive(false);
    }

    private void VictoryGameSound()
    {
        uiAudioSource.PlayOneShot(uiVictorySound);
    }

    private void DefeatGameSound()
    {
        uiAudioSource.PlayOneShot(uiDefeatSound);
    }

    private void OnVictroyPanel()
    {
        victoryPanel.SetActive(true);
    }
    private void OnDefeatPanel()
    {
        defeatPanel.SetActive(true);
    }
    public void ControlBtnScale()
    {
        Debug.LogFormat("{0} 1번째 bool값",unitList[0].gameObject.GetComponent<BuyUnit>().isOnAim);
        Debug.LogFormat("{0} 2번째 bool값",unitList[1].gameObject.GetComponent<BuyUnit>().isOnAim);
        Debug.LogFormat("{0} 3번째 bool값",unitList[2].gameObject.GetComponent<BuyUnit>().isOnAim);
        Debug.LogFormat("{0} 4번째 bool값",unitList[3].gameObject.GetComponent<BuyUnit>().isOnAim);
        //OffScaleUp();

        for (int i = 0; i < unitList.Count; i++)
        {
            if (unitList[i].gameObject.GetComponent<BuyUnit>().isOnAim == true)
            {
                unitList[i].gameObject.GetComponent<RectTransform>().localScale = new Vector3(1.1f, 1.1f, unitList[i].gameObject.GetComponent<RectTransform>().localScale.z);
            }
            else
            {
                unitList[i].gameObject.GetComponent<RectTransform>().localScale = new Vector3(1.0f, 1.0f, 1.0f);
            }
        }
    }
    public void OffScaleUp()
    {
        for (int i = 0; i < unitList.Count; i++)
        {
            unitList[i].gameObject.GetComponent<BuyUnit>().isOnAim = false;
            unitList[i].gameObject.GetComponent<RectTransform>().localScale = new Vector3(1.0f, 1.0f, 1.0f);
        }
    }
}
