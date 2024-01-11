
using Oculus.Platform.Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Golem : MonoBehaviour, IHitObject
{
    public static Golem G_insance = default;

    public bool restart = false;
    public bool minionRestart = false;
    void Awake()
    {
        G_insance = this;
    }


    // {괴수의 페이즈를 체크할 enum 스테이트
    public enum Phase 
    {  
        READY,
        PHASE_1, 
        PHASE_2,  
        PHASE_3,  
        PHASE_4,  
        PHASE_5,  
        PHASE_6,
        PHASE_LAST, 
        GAMEOVER
    }

    public Phase golemCheck { get; private set;}
    // }괴수의 페이즈를 체크할 enum 스테이트

    private bool isAttack = false;
    private bool isThrow = false;
    private bool isSpawnminion = false;

    // {괴수의 각종 변수

    private Vector3 firstPos = Vector3.zero;
    public float golemMaxHp = 100000f;         // 괴수의 초기 체력
    private float currentHp = default;      // 괴수의 현재 체력    

    private float currentTime = default;    // 페이즈별로 캐싱할 현재 시간
    public float phase1Time = 120f;         // 페이즈 1 제한시간
    public float phase2Hp = 0.8f;           // 페이즈 2 진입하는 HP 비율
    public float phase2Distance = 0.8f;     // 페이즈 2 진입하는 거리비율 = 플레이어로부터 80% 지점
    public float phase2Time = 120f;         // 페이즈 2 제한시간
    public float phase3Hp = 0.6f;           // 페이즈 3 진입하는 HP 비율
    public float phase3Distance = 0.6f;     // 페이즈 3 진입하는 거리비율 = 플레이어로부터 60% 지점
    public float phase3Time = 120f;         // 페이즈 3 제한시간
    public float phase4Hp = 0.4f;           // 페이즈 4 진입하는 HP 비율
    public float phase4Distance = 0.4f;     // 페이즈 4 진입하는 거리비율 = 플레이어로부터 40% 지점
    public float phase4Time = 120f;         // 페이즈 4 제한시간
    public float phase5Hp = 0.2f;           // 페이즈 5 진입하는 HP 비율
    public float phase5Distance = 0.2f;     // 페이즈 5 진입하는 거리비율 = 플레이어로부터 40% 지점
    public float phase5Time = 120f;         // 페이즈 5 제한시간
    public float phase6Hp = 0.05f;          // 페이즈 6 진입하는 HP 비율
    public float phase6Distance = 0.05f;     // 페이즈 6 진입하는 거리비율 = 플레이어로부터 40% 지점
    public float phase6Time = 30f;         // 페이즈 6 제한시간

    private IEnumerator currentCoroutine = null;
    private Transform player = default;     // 플레이어를 캐싱할 변수
    private float firstDistnance = default;       // 괴수와 플레이어의 최초 거리 캐싱할 변수
    public float golemSpeed = 5f;           // 괴수의 PC 추적 속도

    private Vector3 target = default;

    [SerializeField] private GameObject RHandBomb;    // 괴수의 원거리공격 투사체 소환 포지션 : 오른손
    [SerializeField] private GameObject LHandBomb;    // 괴수의 원거리공격 투사체 소환 포지션 : 왼손
    [SerializeField] private Transform MinionSpawn;   // 졸개 소환 위치
    [SerializeField] private Transform distance;      // 괴수의 피벗 위치가 안맞아 실제 거리체크할 오브젝트     
    [SerializeField] private Transform[] spawners;

    // }괴수의 각종 변수

    private Rigidbody golemRigid = default;     // 괴수의 속력을 입력할 컴포넌트
    private Animator golemAni = default;        // 괴수의 애니메이션을 관리할 컴포넌트
    private AudioSource myAudio = null;
    public AudioClip golemWalk;
    public AudioClip golemFear;


    private WaitForSeconds ballThrowcooltime = new WaitForSeconds(5f);
    private WaitForSeconds minionSpawncooltime = new WaitForSeconds(25f);

    // Start is called before the first frame update
    void Start()
    {
        // {게임 입장시 골렘이 가져올 정보들
        player = GameObject.FindWithTag("Player").GetComponent<Transform>();        // 플레이어 태그를 찾아 PC 위치정보 캐싱
        golemRigid = GetComponent<Rigidbody>();     // 괴수의 리지드바디
        golemAni = GetComponent<Animator>();        // 괴수의 애니메이터
        firstPos = transform.position;
        target = (player.transform.position - transform.position).normalized;       // 괴수의 진행할 방향을 체크하기 위한 노말라이즈
        firstDistnance = Vector3.Distance(distance.position, player.transform.position); // 괴수의 초기 위치와 PC의 거리 체크
        myAudio = GetComponent<AudioSource>();

        // }게임 입장시 골렘이 가져올 정보들


        // 게임 재시작에도 적용될 초기화 메소드
        Initilize();

    }

    // { 게임 시작
    public void GolemStart()
    {
        Initilize();
        golemCheck = Phase.PHASE_1;
        StartCoroutine(Phase1());
    }
    // } 게임 시작

    private void Update()
    {

        minionRestart = false;
        //Debug.Log($"골렘 페이즈 체크 : {golemCheck}");
        // 괴수는 항상 플레이어를 바라본다.
        transform.LookAt(player.transform.position);

        //  괴수는 라스트페이즈에 진입하면 PC를 향해 멈추지않고 다가오게 된다.
        if (golemCheck == Phase.PHASE_LAST)
        {
            if(Vector3.Distance(distance.position, player.transform.position) <= 2f)
            {
                // { 게임종료에 따른 괴수의 기능동작 정지 행동들

                player.GetComponent<IHitObject>().Hit(100f);
                golemCheck = Phase.GAMEOVER;            // 스테이트 : 게임오버상태
                golemRigid.velocity = Vector3.zero;     // 괴수는 그자리에서 정지
                golemAni.SetBool("isWalk", false);      // 애니메이션은 IDLE로 돌아간다.

                // } 게임종료에 따른 괴수의 기능동작 정지 행동들

                Debug.Log("게임 종료");
            }
            target.y = 0f;
            golemRigid.velocity = target * golemSpeed;
        }

    }

    #region 1페이즈
    IEnumerator Phase1()
    {
        //Initilize();
        golemAni.SetTrigger("isAttackStop");
        // { 제한시간이 다 되거나 일정체력이 깎이면 1페이즈 탈출
        while (currentTime >= 0f && currentHp >= golemMaxHp * phase2Hp)
        {
            currentTime -= Time.deltaTime;

            // 1페이즈 공격패턴은 졸개 소환만
            if (isSpawnminion == false)
            {
                isSpawnminion = true;
                StartCoroutine(MinionCollTime());
                StartCoroutine(SpawnStop());
                golemAni.SetBool("SpawnMinion", true);
            }

            yield return null;
        }
        // } 제한시간이 다 되거나 일정체력이 깎이면 1페이즈 탈출

        // { 2페이즈 돌입을 위한 현재 동작들 정지
        isAttack = false;
        golemAni.SetTrigger("isAttackStop");
        // } 2페이즈 돌입을 위한 현재 동작들 정지

        golemAni.SetBool("isWalk", true);

        // { 1페이즈가 끝난 시점에서 2페이즈로 넘어가는 상태를 체크중
        while (golemCheck == Phase.PHASE_1)
        {
            // 2페이즈 지점에 도달하게 되면
            if (firstDistnance * phase2Distance >= Vector3.Distance(distance.position, player.transform.position))
            {
                // 골렘의 상태 변경
                golemCheck = Phase.PHASE_2;
            }

            // 2페이즈 지점 도달 전까지는 플레이어 방향으로 등속운동 진행
            golemRigid.velocity = target * golemSpeed;

            yield return null;
        }
        // } 1페이즈가 끝난 시점에서 2페이즈로 넘어가는 상태를 체크중

        // { 2페이즈 돌입
        currentTime = phase2Time;
        golemRigid.velocity = Vector3.zero;
        golemAni.SetBool("isWalk", false);
        StartCoroutine(Phase2());

        // } 2페이즈 돌입
    }
    #endregion

    #region 2페이즈
    IEnumerator Phase2()
    {
        // { 제한시간이 다 되거나 일정체력이 깎이면 2페이즈 탈출
        while (currentTime >= 0f && currentHp >= golemMaxHp * phase3Hp)
        {
            currentTime -= Time.deltaTime;

            if(isSpawnminion == false && isAttack == false)
            {
                isSpawnminion = true;
                isAttack = true;
                StartCoroutine(MinionCollTime());
                StartCoroutine(SpawnStop());
                golemAni.SetBool("SpawnMinion", true);
            }
            else if(isThrow == false && isAttack == false)
            {
                isThrow = true;
                isAttack = true;
                ThrowBallStart();
            }

            // } 상단에 기술한 1페이즈 공격패턴과 동일한 동작

            yield return null;
        }
        // } 제한시간이 다 되거나 일정체력이 깎이면 2페이즈 탈출

        // { 다음 페이즈 돌입을 위한 현재 동작들 정지
        isAttack = false;
        LHandBomb.SetActive(false);
        RHandBomb.SetActive(false);
        golemAni.SetTrigger("isAttackStop");
        // } 다음 페이즈 돌입을 위한 현재 동작들 정지

        golemAni.SetBool("isWalk", true);

        while (golemCheck == Phase.PHASE_2)
        {
            // 2페이즈 지점에 도달하게 되면
            if (firstDistnance * phase3Distance >= Vector3.Distance(distance.position, player.transform.position))
            {
                // 골렘의 상태 변경
                golemCheck = Phase.PHASE_3;
            }

            // 2페이즈 지점 도달 전까지는 플레이어 방향으로 등속운동 진행
            golemRigid.velocity = target * golemSpeed;

            yield return null;
        }

        currentTime = phase3Time;
        golemRigid.velocity = Vector3.zero;
        golemAni.SetBool("isWalk", false);
        StartCoroutine(Phase3());
    }
    #endregion

    #region 3페이즈
    IEnumerator Phase3()
    {
        // { 제한시간이 다 되거나 일정체력이 깎이면 3페이즈 탈출
        while (currentTime >= 0f && currentHp >= golemMaxHp * phase4Hp)
        {
            currentTime -= Time.deltaTime;

            if (isSpawnminion == false && isAttack == false)
            {
                isSpawnminion = true;
                isAttack = true;
                StartCoroutine(MinionCollTime());
                StartCoroutine(SpawnStop());
                golemAni.SetBool("SpawnMinion", true);
            }
            else if (isThrow == false && isAttack == false)
            {
                isThrow = true;
                isAttack = true;
                ThrowBallStart();
            }

            yield return null;
        }
        // } 제한시간이 다 되거나 일정체력이 깎이면 3페이즈 탈출

        // { 다음 페이즈 돌입을 위한 현재 동작들 정지
        isAttack = false;
        LHandBomb.SetActive(false);
        RHandBomb.SetActive(false);
        golemAni.SetTrigger("isAttackStop");
        // } 다음 페이즈 돌입을 위한 현재 동작들 정지

        golemAni.SetBool("isWalk", true);

        // { 1페이즈가 끝난 시점에서 2페이즈로 넘어가는 상태를 체크중
        while (golemCheck == Phase.PHASE_3)
        {
            // 2페이즈 지점에 도달하게 되면
            if (firstDistnance * phase4Distance >= Vector3.Distance(distance.position, player.transform.position))
            {
                // 골렘의 상태 변경
                golemCheck = Phase.PHASE_4;
            }

            // 2페이즈 지점 도달 전까지는 플레이어 방향으로 등속운동 진행
            golemRigid.velocity = target * golemSpeed;

            yield return null;
        }
        // } 1페이즈가 끝난 시점에서 2페이즈로 넘어가는 상태를 체크중

        // { 4페이즈 돌입
        currentTime = phase4Time;
        golemRigid.velocity = Vector3.zero;
        golemAni.SetBool("isWalk", false);
        StartCoroutine(Phase4());

        // } 4페이즈 돌입
    }
    #endregion

    #region 4페이즈
    IEnumerator Phase4 ()
    {
        // { 제한시간이 다 되거나 일정체력이 깎이면 4페이즈 탈출
        while (currentTime >= 0f && currentHp >= golemMaxHp * phase5Hp)
        {
            currentTime -= Time.deltaTime;

            if (isSpawnminion == false && isAttack == false)
            {
                isSpawnminion = true;
                isAttack = true;
                StartCoroutine(MinionCollTime());
                StartCoroutine(SpawnStop());
                golemAni.SetBool("SpawnMinion", true);
            }
            else if (isThrow == false && isAttack == false)
            {
                isThrow = true;
                isAttack = true;
                ThrowBallStart();
            }

            // } 상단에 기술한 1페이즈 공격패턴과 동일한 동작

            yield return null;
        }
        // } 제한시간이 다 되거나 일정체력이 깎이면 4페이즈 탈출

        // { 다음 페이즈 돌입을 위한 현재 동작들 정지
        isAttack = false;
        LHandBomb.SetActive(false);
        RHandBomb.SetActive(false);
        golemAni.SetTrigger("isAttackStop");
        // } 다음 페이즈 돌입을 위한 현재 동작들 정지

        golemAni.SetBool("isWalk", true);

        // { 1페이즈가 끝난 시점에서 2페이즈로 넘어가는 상태를 체크중
        while (golemCheck == Phase.PHASE_4)
        {
            // 2페이즈 지점에 도달하게 되면
            if (firstDistnance * phase5Distance >= Vector3.Distance(distance.position, player.transform.position))
            {
                // 골렘의 상태 변경
                golemCheck = Phase.PHASE_5;
            }

            // 2페이즈 지점 도달 전까지는 플레이어 방향으로 등속운동 진행
            golemRigid.velocity = target * golemSpeed;

            yield return null;
        }
        // } 1페이즈가 끝난 시점에서 2페이즈로 넘어가는 상태를 체크중

        // { 다음 페이즈 진입
        currentTime = phase5Time;
        golemRigid.velocity = Vector3.zero;
        golemAni.SetBool("isWalk", false);
        StartCoroutine(Phase5());
        // } 다음 페이즈 진입

    }
    #endregion

    #region 5페이즈
    IEnumerator Phase5 ()
    {
        // { 제한시간이 다 되거나 일정체력이 깎이면 3페이즈 탈출
        while (currentTime >= 0f && currentHp >= golemMaxHp * phase6Hp)
        {
            currentTime -= Time.deltaTime;

            if (isThrow == false)
            {
                isThrow = true;
                ThrowBallStart();
            }

            // } 상단에 기술한 1페이즈 공격패턴과 동일한 동작

            yield return null;
        }
        // } 제한시간이 다 되거나 일정체력이 깎이면 3페이즈 탈출

        // { 3페이즈 돌입을 위한 현재 동작들 정지
        isAttack = false;
        LHandBomb.SetActive(false);
        RHandBomb.SetActive(false);
        golemAni.SetTrigger("isAttackStop");
        // } 2페이즈 돌입을 위한 현재 동작들 정지

        golemAni.SetBool("isWalk", true);

        // { 1페이즈가 끝난 시점에서 2페이즈로 넘어가는 상태를 체크중
        while (golemCheck == Phase.PHASE_5)
        {
            // 2페이즈 지점에 도달하게 되면
            if (firstDistnance * phase6Distance >= Vector3.Distance(distance.position, player.transform.position))
            {
                // 골렘의 상태 변경
                golemCheck = Phase.PHASE_6;
            }

            // 2페이즈 지점 도달 전까지는 플레이어 방향으로 등속운동 진행
            golemRigid.velocity = target * golemSpeed;

            yield return null;
        }
        // } 1페이즈가 끝난 시점에서 2페이즈로 넘어가는 상태를 체크중

        // { 4페이즈 돌입
        currentTime = phase6Time;
        golemRigid.velocity = Vector3.zero;
        golemAni.SetBool("isWalk", false);
        StartCoroutine(Phase6());
    }
    #endregion

    #region 6페이즈
    IEnumerator Phase6()
    {
        // { 제한시간이 다 되거나 일정체력이 깎이면 6페이즈 탈출
        while (currentTime >= 0f && currentHp >= golemMaxHp * phase5Hp)
        {
            // 페이즈 6부터는 공격패턴이 사라짐
            currentTime -= Time.deltaTime;
            yield return null;
        }
        // } 제한시간이 다 되거나 일정체력이 깎이면 6페이즈 탈출

        // { 다음 페이즈 돌입을 위한 현재 동작들 정지
        isAttack = false;
        LHandBomb.SetActive(false);
        RHandBomb.SetActive(false);
        golemAni.SetTrigger("isAttackStop");
        // } 다음 페이즈 돌입을 위한 현재 동작들 정지

        golemAni.SetBool("isWalk", true);

        // { 라스트 페이즈 돌입
        golemCheck = Phase.PHASE_LAST;
        golemAni.SetBool("isWalk", true);
        golemAni.SetTrigger("isAttackStop");
    }
    #endregion 

    // 원거리 공격 메소드 
    private void ThrowBallStart()
    {
        // 원거리 공격 중 왼팔,오른팔을 정할 랜덤값
        int attackPos = Random.Range(0, 2);

        // { 오브젝트풀링 및 투사체의 포물선 운동에 접근하기 위한 각 장치

        // 랜덤값에 의한 왼팔, 오른팔 동작 스위치문
        switch (attackPos)
        {
            // 0이 입력되면 왼팔
            case 0:
                StartCoroutine(FireCooltime());                     // 원거리 공격 쿨타임
                StartCoroutine(LeftStop());
                golemAni.SetBool("isLeftAttack", true);                // 왼팔 애니메이터 동작
                LHandBomb.SetActive(true);

                break;

            // 1이 입력되면 오른팔
            case 1:
                StartCoroutine(FireCooltime());                     // 원거리 공격 쿨타임
                StartCoroutine(SpawnStop());
                golemAni.SetBool("isRightAttack", true);               // 오른팔 애니메이터 동작
                RHandBomb.SetActive(true);
                break;
            
        }

    }

    // 왼손 애니메이션이 선택 되었을 때 (애니메이션 이벤트 함수)
    private void FireLeft()
    {
        GameObject obj = ObjectPoolManager.instance.GetPoolObj(PoolObjType.ROCK);
        obj.transform.position = LHandBomb.transform.position;
        obj.SetActive(true);
        LHandBomb.SetActive(false);
    }

    // 오른손 애니메이션이 선택 되었을 때 (애니메이션 이벤트 함수)
    private void FireRight()
    {
        GameObject obj = ObjectPoolManager.instance.GetPoolObj(PoolObjType.ROCK);
        obj.transform.position = RHandBomb.transform.position;
        obj.SetActive(true);
        RHandBomb.SetActive(false);
    }

    // 졸개 소환 (애니메이션 이벤트 함수)
    private void SpawnMinion()
    {
        GameObject minion = null;

        int bombCount = 0;

        for(int i = 0; i < 10; i++)
        {
            int rand = Random.Range(0, 2);

            if(bombCount >= 3)
            {
                rand = 0;
            }

            switch(rand)
            {
                case 0:
                    minion = ObjectPoolManager.instance.GetPoolObj(PoolObjType.MINION_BASIC);
                    break;
                case 1:
                    minion = ObjectPoolManager.instance.GetPoolObj(PoolObjType.MINION_BOMB);
                    //minion.transform.position = spawners[i].position;
                    bombCount++;
                    break;
            }

            minion.transform.position = spawners[i].position;
            minion.SetActive(true);

        }
    }

    // 공격애니메이션이 끝나고 지정할 대기 시간
    IEnumerator FireCooltime()
    {
        isAttack = false;
        yield return ballThrowcooltime;
        isThrow = false;
    }

    IEnumerator MinionCollTime()
    {
        isAttack = false;
        yield return minionSpawncooltime;
        isSpawnminion = false;
    }

    public void Initilize()
    {
        StopAllCoroutines();                        // 괴수가 가진 모든 코루틴 정지
        //golemAni.SetTrigger("isAttackStop");
        golemAni.SetBool("isWalk", false);
        isAttack = false;                           // 공격 코루틴 진입조건 초기화
        isThrow = false;                            // ""
        isSpawnminion = false;                      // ""
        currentTime = phase1Time;                   // 페이즈 시간 초기화
        currentHp = golemMaxHp;                     // 괴수의 초기 체력은 설정한 Max체력값
        golemCheck = Phase.READY;                   // 골렘 스테이트 초기화
        transform.position = firstPos;              // 골렘 위치 초기화
        KHJUIManager.Instance.ChangeBossHpText(currentHp, golemMaxHp);

    }

    public void Hit(float damage)
    {
        if(golemCheck != Phase.GAMEOVER)
        {
            currentHp -= damage;
            KHJUIManager.Instance.ChangeBossHpText(currentHp, golemMaxHp);
            if (currentHp <= 0)
            {
                GameManager.Instance.EndGame();
                KHJUIManager.Instance.VictoryGame();
                StopAllCoroutines();
                golemRigid.velocity = Vector3.zero;
                golemCheck = Phase.GAMEOVER;
                golemAni.SetTrigger("isDie");

                //GameManager.Instance.EndGame();
            }


        }
    }

    public void GolemStop()
    {
        StopAllCoroutines();
        golemRigid.velocity = Vector3.zero;
        golemCheck = Phase.GAMEOVER;

        //golemAni.SetTrigger("isAttackStop");
        golemAni.SetBool("isWalk", false);
    }

    IEnumerator SpawnStop()
    {
        yield return new WaitForSeconds(2.3f);

        golemAni.SetBool("SpawnMinion", false);
        golemAni.SetBool("isRightAttack", false);
    }

    IEnumerator LeftStop()
    {
        yield return new WaitForSeconds(1.3f);

        golemAni.SetBool("isLeftAttack", false);
    }

    private void GolemWalk()
    {
        //myAudio.clip = golemWalk;
        //myAudio.Play();

        myAudio.PlayOneShot(golemWalk);
    }

    private void GolemFear()
    {
        //myAudio.clip = golemFear;
        //myAudio.Play();

        myAudio.PlayOneShot(golemFear);

    }

    // LEGACY : 개발일지에 쓰일 공격패턴 애니메이션 설정 오류부분 (애니메이션 이벤트가 아닌 코루틴으로 접근하려 했음)

    //// 원거리 공격 코루틴
    //IEnumerator ThrowBall()
    //{
    //    // 원거리 공격 중 왼팔,오른팔을 정할 랜덤값
    //    int attackPos = Random.Range(0, 2);

    //    // 코루틴 진행 중 다른 공격행동 진입을 방지하기 위한 불값 변경
    //    isAttack = true;

    //    // { 오브젝트풀링 및 투사체의 포물선 운동에 접근하기 위한 각 장치
    //    GameObject obj = ObjectPoolManager.instance.GetPoolObj(PoolObjType.BOMB);       // 풀링 오브젝트 호출
    //    Bomb objBomb = null;            // 투사체의 포물선 운동이 입력된 Cs 캐싱
    //    Rigidbody objRigid = null;      // 투사체의 포물선 운동을 위한 Rigidbody 컴포넌트 캐싱
    //    Vector3 shoot = Vector3.zero;   // 투사체의 포물선 운동값을 캐싱할 변수

    //    // 호출한 오브젝트가 입력 되었다면 각 컴포넌트 가져오기
    //    if(obj != null)
    //    {
    //        objBomb = obj.GetComponent<Bomb>();
    //        objRigid = obj.GetComponent<Rigidbody>();

    //    }
    //    // }오브젝트풀링 및 투사체의 포물선 운동에 접근하기 위한 각 장치

    //    // 랜덤값에 의한 왼팔, 오른팔 동작 스위치문
    //    switch (attackPos)
    //    {
    //        // 0이 입력되면 왼팔
    //        case 0:
    //            golemAni.SetTrigger("isLeftAttack");                // 왼팔 애니메이터 동작
    //            obj.transform.position = LHand.transform.position;  // 왼손 위치에 투사체 소환
    //            obj.transform.parent = LHand.transform;             // 애니메이션에 따른 오브젝트 이동을 위해 잠시 왼손에 종속
    //            obj.SetActive(true);                                // 소환된 투사체 활성화

    //            yield return new WaitForSeconds(0.95f);     // 왼손이 내지르는데 걸리는 시간

    //            obj.transform.parent = null;                // 왼손을 내지르면 투사체 종속 해제

    //            // { 투사체의 포물선 운동 동작
    //            shoot = objBomb.GetVelocity(obj.transform.position, player.transform.position, 30f);
    //            objRigid.velocity = shoot;
    //            // } 투사체의 포물선 운동 동작
    //            break;

    //        // 1이 입력되면 오른팔
    //        case 1:
    //        golemAni.SetTrigger("isRightAttack");                   // 오른팔 애니메이터 동작
    //            obj.transform.position = RHand.transform.position;  // 오른손 위치에 투사체 소환
    //            obj.transform.parent = RHand.transform;             // 애니메이션에 따른 오브젝트 이동을 위해 잠시 오른손에 종속
    //            obj.SetActive(true);                                // 소환된 투사체 활성화

    //            yield return new WaitForSeconds(1.5f);      // 오른손이 내지르는데 걸리는 시간

    //            obj.transform.parent = null;                // 오른손을 내지르면 투사체 종속 해제

    //            // { 투사체의 포물선 운동 동작
    //            shoot = objBomb.GetVelocity(obj.transform.position, player.transform.position, 30f);
    //            objRigid.velocity = shoot;
    //            // } 투사체의 포물선 운동 동작
    //            break;
    //    }

    //    // 원거리 공격의 쿨타임
    //    yield return new WaitForSeconds(5f);

    //    // 쿨타임이 끝나면 공격행동 재진입을 위해 불값 초기화
    //    isAttack = false;

    //    // 원거리 공격 코루틴 재진입을 위해 코루틴 초기화
    //    throwball = ThrowBall();
    //}

    //IEnumerator SpawnMinion22()
    //{
    //    Vector3 spawnPoint = Vector3.zero;

    //    GameObject minion = null;

    //    for(int i = 0; i < 10; i++)
    //    {
    //        int randomMinion = Random.Range(0,2);

    //        switch(randomMinion)
    //        {
    //            case 0:
    //                minion = ObjectPoolManager.instance.GetPoolObj(PoolObjType.MINION_BASIC);
    //                break;
    //            case 1:
    //                minion = ObjectPoolManager.instance.GetPoolObj(PoolObjType.MINION_BOMB);
    //                break;
    //        }

    //        spawnPoint.x = MinionSpawn.transform.position.x + Random.Range(-20f, 20f);
    //        spawnPoint.z = MinionSpawn.transform.position.z + Random.Range(-3f, -1f);
    //        spawnPoint.y = MinionSpawn.transform.position.y;

    //        minion.transform.position = spawnPoint;
    //        minion.SetActive(true);

    //    }

    //    yield return new WaitForSeconds(5f);

    //    isAttack = false;
    //}

}
