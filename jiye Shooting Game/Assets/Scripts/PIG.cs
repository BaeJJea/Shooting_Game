using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PIG : MonoBehaviour
{
    [SerializeField]    private string animalName;//동물의 이름
    [SerializeField]    private int hp;//동물의 체력
    [SerializeField]    private float walkSpeed;//걷기 스피드
    [SerializeField] private float runSpeed;//뛰기 스피드
    [SerializeField] private float turningSpeed;//회전 스피드
    private float applySpeed;

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


    //랜덤한 방향으로 걷기 위한것
    // private Vector3 direction;
     private Vector3 destination;//목적지


    // Start is called before the first frame update
    void Start()
    {
        nav = GetComponent<NavMeshAgent>();
        theAudio = GetComponent<AudioSource>();
        currentTime = waitTime;
        isAction = true;
    }

    // Update is called once per frame
    void Update()
    {
        ElapseTime();
        Move();
       // Rotation();
    }

    private void Move()
    {
        if (isWalking || isRunning)
            //  rigid.MovePosition(transform.position + transform.forward * applySpeed * Time.deltaTime);
            nav.SetDestination(transform.position + destination * 5f);
    }

   /* private void Rotation()
    {//움직일때만 회전하게 만듦
        if (isWalking || isRunning)
        {
            Vector3 _rotation = Vector3.Lerp(transform.eulerAngles, new Vector3(0f, direction.y, 0f), turningSpeed);//자기자신의 위치, 목표위치, 속도
            rigid.MoveRotation(Quaternion.Euler(_rotation));
        }
    }*/

    private void ElapseTime()
    {
        if(isAction)
        {
            currentTime -= Time.deltaTime;
            if (currentTime <= 0)//다음 랜덤 행동 개시
            {
                ReSet();
            }
        }
    }

    private void ReSet()
    {
        isWalking = false;        isRunning = false;
        nav.speed = walkSpeed;
        isAction = true;
        //nav 목적지 초기화 시키기
        nav.ResetPath();
        anim.SetBool("Walk", isWalking);        anim.SetBool("Run", isRunning);

        // direction.Set(0f, Random.Range(0f, 360f), 0f);//리셋할때마다 방향도 어디로 갈지 리셋 시킴
        destination.Set(Random.Range(-0.2f,0.2f), 0f , Random.Range(-0.5f, 1f));//Random.Range(-0.5f, 1f)많이 멀리갈지 적당히 멀리갈지 정해줌
        RandomAction();
    }

    private void RandomAction()
    {
        RandomSound();
        isAction = true;

        int _random = Random.Range(0, 4);//대기, 걷기->0,1,----2는 포함되지 않음 ※2f하면 2도 포함됨
        if (_random == 0)
        {
            currentTime = waitTime;
            Debug.Log("대기");
        }
        else if (_random == 1)
        {
            TryWalk();
        }

        else if (_random == 2)
        {
            TryWalk();
        }

        else if (_random == 3)
        {
            TryWalk();
        }
    }

    private void TryWalk()
    {
        isWalking = true;
        anim.SetBool("Walk", isWalking);
        currentTime = walkTime;
        nav.speed = walkSpeed;
        Debug.Log("걷기");
    }

    //플레이어의 반대방향으로 뛰게 만들기
    public void Run(Vector3 _targetPos)
    {
        //direction = Quaternion.LookRotation(transform.position - _targetPos).eulerAngles;
        destination = new Vector3(transform.position.x - _targetPos.x, 0f, transform.position.z - _targetPos.z).normalized;//normalized: 벡터 합이 1이 되게 함
        currentTime = runTime;
        isWalking = false;
        isRunning = true;
        nav.speed = runSpeed;
        anim.SetBool("Run", isRunning);
    }
    //데미지를 입을때 run을 실행
    public void Damage(int _dmg, Vector3 _targetPos)
    {
        hp -= _dmg;
        if(hp <= 0)
        {
            Debug.Log("체력 0 이하");
            return;
        }

        PlaySE(sound_pig_Dead);
        Run(_targetPos);
    }
    //일상 사운드 랜던 재생
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
}
