using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class MinionBase : MonoBehaviour
{
    private Transform player = default;                             // PC 위치정보 캐싱
    private WaitForSeconds readyTime = new WaitForSeconds(1f);      // PC 추적을 시작할 대기 시간
    private WaitForSeconds coolingTime = new WaitForSeconds(3f);    // DeadZone 트리거시 오브젝트 풀 반환 시간
    public float minionSpeed = 5f;                                  // 졸개의 이동속도
    private float firstDistance = default;                          // PC와의 거리 (생성 되었을시 캐싱)
    protected bool isDetected = false;                                // 추적활성화 불값
    public bool isAttack = false;                                   // 하위 클래스에서 참조할 공격 실행 불값
    private bool isLimit = false;                                   // DeadZone 트리거시 Update상 동작 방지용 불값


    // ************* 자식에서도 해당 컴포넌트에 접근하려면 protected가 아닌 퍼블릭으로 열어야함
    // 나중에 원리 확인해보기

    public Rigidbody myRigid = default;                            // 자신의 Rigidbody 캐싱
    public Collider[] myCollider = default;    
    public Animator myAni = default;                                // 자신의 애니메이터 캐싱 (자식 클래스에서 각자의 애니메이터 인스펙어창에서 할당)
    public AudioSource myAudio;
    public AudioClip spawnClip;
    public AudioClip deathClip;
    public AudioClip hitClip;

    void Awake()
    {
        // PC 오브젝트 캐싱하기
        player = GameObject.FindWithTag("Player").GetComponent<Transform>();

        // 자신의 Rigidbody 캐싱
        myRigid = GetComponent<Rigidbody>();
        myCollider = GetComponents<Collider>();

        myAudio.clip = spawnClip;
        myAudio.Play();

        // 처음 생성되었을시 PC 와의 거리 캐싱 ( PC가 고정되어 있어서 Start시에만 캐싱하면 된다. )
        firstDistance = Vector3.Distance(transform.position, player.transform.position);


    }

    // Update is called once per frame
    protected virtual void Update()
    {
        // TODO : 졸개들은 리미트 지점에 닿으면 모든 행동을 멈추고 오브젝트 풀에 반환
        if (isLimit == true || GameManager.Instance.playerState == PlayerState.DEAD)
        {
            myAni.SetBool("isWalk", false);
            return;
        }

        // 항상 플레이어를 바라본다.

        Vector3 reTarget = player.transform.position;
        reTarget.y = transform.position.y;
        transform.LookAt(reTarget);

        // 플레이어로 향하는 방향 연산
        Vector3 target = (player.transform.position - transform.position).normalized;

        // 생성 되었을시 플레이어의 거리를 저장해둬서 해당 거리의 비율을 비교해 플레이어에게 도달한것을 인지
        if (Vector3.Distance(transform.position, player.transform.position) <= 3.0f)
        {
            myRigid.velocity = Vector3.zero;        // PC에게 도달하면 속도값 0
            isDetected = false;                     // 추적 금지 (걷는 애니메이션, PC향해 velocity 가지기)
            myAni.SetBool("isWalk", false);         // 애니메이션 Idle 전환
            isAttack = true;                        // 자식 클래스에서 쓰일 isAttack (공격을 진행하라)
            return;
        }

        // 추적 상태일 때는 걷는 애니메이션, PC향해 속도값 가지기
        if(isDetected == true)
        {
            myAni.SetBool("isWalk", true);
            target.y = 0f;
            myRigid.velocity = target * minionSpeed;
        }
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        // 졸개들이 절벽을 넘어섰을때 오브젝트 풀에 반환하기 위해 세팅
        if(other.CompareTag("DeadZone"))
        {
            myRigid.velocity = Vector3.zero;
            isLimit = true;
        }
    }

    // 풀링오브젝트로 인한 오브젝트 활성화시 초기상태 초기화
    protected virtual void OnEnable()
    {
        myRigid.useGravity = true;
        foreach (Collider collider in myCollider)
        {
            collider.enabled = true;
        }
        myAudio.clip = spawnClip;
        myAudio.Play();
        isLimit = false;                    // 풀에 반환되기 전 절벽을 넘었을시 세팅값 초기화
        isDetected = false;                 // 풀에 반환되기 전 PC에게 도달했을시 추적상태 초기화
        isAttack = false;                   // 풀에 반환되기 전 PC에게 도달하여 공격진입 상태 초기화
        StartCoroutine(DetectedStart());    // 생성되면 1초 이후에 PC 추적
    }

    // 생성되면 일정시간 이후 PC를 추적할 코루틴
    IEnumerator DetectedStart()
    {
        yield return readyTime;
        isDetected = true;
    }

}
