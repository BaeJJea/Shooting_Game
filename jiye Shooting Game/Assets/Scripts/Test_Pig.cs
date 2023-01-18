using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class Test_Pig : MonoBehaviour
{
    [SerializeField] private float walkSpeed;//걷기 스피드
    [SerializeField] private float runSpeed;//뛰기 스피드

    //상태변수
    private bool isAction;//행동을 취하는지 안하는지 판별
    private bool isWalking;//걷는지 안 걷는지 판별하는 상태 변수
    private bool isRunning;


    [SerializeField] private float walkTime;//걷기 시간
    [SerializeField] private float waitTime;//대기 시간
    [SerializeField] private float runTime;
    private float currentTime;

    //필요한 컴포넌트
    [SerializeField] private Animator anim;
    [SerializeField] private Rigidbody rigid;
    [SerializeField] private BoxCollider boxCol;
    private AudioSource theAudio;
    private NavMeshAgent nav;

    [SerializeField] private AudioClip[] sound_pig_Normal;//일상 사운드
    [SerializeField] private AudioClip sound_pig_Dead;
    [SerializeField] private AudioClip sound_pig_Hurt;

    //랜덤한 방향으로 걷기 위한것
    private Vector3 destination;//목적지

    public int maxHealth;
    public int curHealth;//현재 체력
    public Transform Target;//ai의 표적물
    public bool isChase;//추적을 결정하는 변수 추가
    Material mat;


    // Start is called before the first frame update
    void Start()
    {
        nav = GetComponent<NavMeshAgent>();
        theAudio = GetComponent<AudioSource>();
        currentTime = waitTime;
        isAction = true;
        mat = GetComponentInChildren<SkinnedMeshRenderer>().material;//material은 mesh Renderer컴포넌트에서 접근 가능
        TryWalk();

    }

    // Update is called once per frame
    void Update()
    {
        ElapseTime();
        

        // Rotation();
    }

    void ChaseStart()
    {
        isChase = true;
        anim.SetBool("Walk", true);
    }

    private void Move()
    {
        if (isWalking)
            nav.SetDestination(transform.position + destination * 5f);
       // Debug.Log("움직이는중");
    }

   

    private void ElapseTime()
    {
        if (isAction)
        {
            currentTime -= Time.deltaTime;
            if (currentTime <= 0)//다음 랜덤 행동 개시
            {
                TryWalk();
              //  Debug.Log("초기화");
            }
        }
    }

    private void ReSet()
    {
        isWalking = false; isRunning = false;
        nav.speed = walkSpeed;
        isAction = true;
        //nav 목적지 초기화 시키기
        nav.ResetPath();
        anim.SetBool("Walk", isWalking); 
        destination.Set(Random.Range(-0.2f, 0.2f), 0f, Random.Range(-0.5f, 1f));//Random.Range(-0.5f, 1f)많이 멀리갈지 적당히 멀리갈지 정해줌
        RandomAction();
       // Debug.Log("초기화");
    }

    private void RandomAction()
    {
        RandomSound();
        isAction = true;

        
            TryWalk();
        
    }

    private void TryWalk()
    {
        destination.Set(Random.Range(-0.2f, 0.2f), 0f, Random.Range(-0.5f, 1f));
        nav.SetDestination(transform.position + destination * 10f);
        isWalking = true;
        isRunning = false;
        anim.SetBool("Walk", isWalking);
        anim.SetBool("Run", isRunning);
        currentTime = walkTime;
        nav.speed = walkSpeed; 
    //    Debug.Log("걷는중");

    }
    //플레이어의 반대방향으로 뛰게 만들기
    public void Run()
    {
        nav.SetDestination(Target.position);//목표물 추적하기
        currentTime = runTime;
        isWalking = false;
        isRunning = true;
        nav.speed = runSpeed;
        anim.SetBool("Walk", isWalking);
        anim.SetBool("Run", isRunning);
      //  Debug.Log("달려드는중");
    }

    //일상 사운드 랜덤 재생
    private void RandomSound()
    {
        int _random = Random.Range(0, 3);
        PlaySE(sound_pig_Normal[_random]);
    }


    private void PlaySE(AudioClip _clip)
    {
        theAudio.clip = _clip;
        theAudio.Play();

    }

    //날라오는 총알에 맞아야 함
    //총알을 맞을때 데미지만큼 피가 까짐
    void OnTriggerEnter(Collider other)
    {
        Bullet bullet = other.GetComponent<Bullet>();
        curHealth -= bullet.damage;
        PlaySE(sound_pig_Hurt);
        StartCoroutine(OnDamage());
        Debug.Log(curHealth);
    }

    //피격 로직
    //맞았을 때에 빨간색으로 변함, 아직 안 죽었을 때에는 원래대로 돌아오고 죽었을때는 회색으로 변한뒤 4초뒤 사라짐
    IEnumerator OnDamage()
    {
        if (curHealth > 0)
        {
            mat.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            mat.color = Color.white;
        }

        else
        {
            mat.color = Color.gray;
            anim.SetTrigger("Die");
            isChase = false;
            nav.enabled = false;//사망 리액션을 유지하기 위해 NavAgent를 비활성
            PlaySE(sound_pig_Dead);

            Destroy(gameObject, 4);
        }
    }
}
