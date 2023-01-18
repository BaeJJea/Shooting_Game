using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ForEnemy : MonoBehaviour
{
    public enum Type { A, B, C };
    public Type enemyType;

    public int maxHealth;
    public int curHealth;
    public Transform target;
    public BoxCollider meleeArea;
    public GameObject bullet;




    Material mat;


    NavMeshAgent nav;
    Rigidbody rigid;
    BoxCollider boxCollider;
    [SerializeField] private Animator anim;

    public bool isChase;//쫓아가는중
    public bool isAttack;//공격하는중


    private AudioSource theAudio;
    [SerializeField] private AudioClip sound_pig_Normal;//일상 사운드
    [SerializeField] private AudioClip sound_pig_Dead;
    [SerializeField] private AudioClip sound_pig_Hurt;
    [SerializeField] private AudioClip sound_pig_attack;

    public float currentTime;
    private Vector3 destination;
    [SerializeField] private float walkTime;
    [SerializeField] private float walkSpeed;


    void Awake()
    {
        theAudio = GetComponent<AudioSource>();
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        mat = GetComponentInChildren<SkinnedMeshRenderer>().material;



        nav = GetComponent<NavMeshAgent>();
        currentTime = walkTime;
        Invoke("ChaseStart", 2);


    }


    void ChaseStart()
    {
        isChase = true;
        anim.SetBool("Walk", true);
    }


    void Update()
    {
        if (nav.enabled)
        {   nav.SetDestination(target.position);
            nav.isStopped = !isChase;
            anim.SetBool("Run", true);
        }



        if (nav.enabled == false && curHealth > 0)
        {
            RandomWalk();
            ElapseTime();
            Rotation();
        }

    }

    private void ElapseTime()
    {
        currentTime -= Time.deltaTime;
        if (currentTime <= 0)
        {
            destination.Set(0f, Random.Range(0f, 360f), 0f);
            Debug.Log("회전 초기화");
            PlaySE(sound_pig_Normal);
            Debug.Log("소리");
            currentTime = walkTime;
        }
    }

    private void Rotation()
    {
        Vector3 _rotation = Vector3.Lerp(transform.eulerAngles, destination, 0.01f);//0.01f는 속도
        rigid.MoveRotation(Quaternion.Euler(_rotation));

    }



    private void RandomWalk()
    {
        //  destination.Set(Random.Range(-0.2f, 0.2f), 0f, Random.Range(-0.5f, 1f));
        //rigid.MovePosition(transform.position + destination * 1f);
        rigid.MovePosition(transform.position + (transform.forward * walkSpeed * Time.deltaTime));

        anim.SetBool("Walk", true);


    }


    private void PlaySE(AudioClip _clip)
    {
        theAudio.clip = _clip;
        theAudio.Play();

    }



    void Targeting()
    {

        float targetRadius = 0;
        float targetRange = 0;

        switch (enemyType)
        {

            case Type.A:
                targetRadius = 1f;
                targetRange = 1f;
                break;
            case Type.B:
                targetRadius = 0.5f;
                targetRange = 5f;
                break;
            case Type.C:
                targetRadius = 0.1f;
                targetRange = 20f;

                break;
        }



        RaycastHit[] rayHits =
            Physics.SphereCastAll(transform.position, targetRadius, transform.forward, targetRange, LayerMask.GetMask("Player"));
        if (rayHits.Length > 0 && !isAttack && nav.enabled == true)//rayHits.Length에 정보가 들어오면서 공격중이 아니라면 공격하기
        {
            StartCoroutine(Attack());
        }
    }

    IEnumerator Attack()
    {
        isChase = false;
        isAttack = true;
        anim.SetBool("Attack", true);
        PlaySE(sound_pig_attack);

        switch (enemyType)
        {
            case Type.A:
                yield return new WaitForSeconds(0.2f);
                meleeArea.enabled = true;
                yield return new WaitForSeconds(1f);
                meleeArea.enabled = false;

                yield return new WaitForSeconds(1f);
                break;
            case Type.B:
                yield return new WaitForSeconds(0.1f);
                rigid.AddForce(transform.forward * 70, ForceMode.Impulse);//돌격 구현
                meleeArea.enabled = true;
                yield return new WaitForSeconds(0.5f);
                rigid.velocity = Vector3.zero;
                meleeArea.enabled = false;
                yield return new WaitForSeconds(2f);

                break;
            case Type.C:


                yield return new WaitForSeconds(0.4f);

                GameObject instantBullet = Instantiate(bullet, transform.position, transform.rotation);
                Rigidbody rigidBullet = instantBullet.GetComponent<Rigidbody>();
                rigidBullet.velocity = transform.forward * 20;

                yield return new WaitForSeconds(2f);




                break;
        }



        isChase = true;
        isAttack = false;
        anim.SetBool("Attack", false);

    }




    void FixedUpdate()
    {
        Targeting();
        FreezeVelocity();
    }

    void FreezeVelocity()
    {
       // if (isChase)
        {
            rigid.angularVelocity = Vector3.zero;
            rigid.velocity = Vector3.zero;

        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "bullet")
        {
            Bullet bullet = other.GetComponent<Bullet>();
            curHealth -= bullet.damage;

            StartCoroutine(OnDamage());
            Debug.Log(curHealth);
        }
    }

    IEnumerator OnDamage()
    {
        mat.color = Color.red;

        yield return new WaitForSeconds(0.1f);

        if (curHealth > 0)
        {
            switch (enemyType)
            {
                case Type.A:
                    mat.color = Color.white;

                    break;
                case Type.B:
                    mat.color = Color.yellow;

                    break;
                case Type.C:
                    mat.color = Color.white;

                    break;

            }



            // mat.color = Color.white;

            nav.enabled = true;
            PlaySE(sound_pig_Hurt);
        }
        else
        {
            gameObject.layer = 12;
            nav.enabled = false;
            mat.color = Color.gray;


            anim.SetTrigger("Dead");
            isChase = false;

            PlaySE(sound_pig_Dead);
            Destroy(gameObject, 4);
            killenemycount._instance.count++;
            Debug.Log("죽음");
        }
    }
}
