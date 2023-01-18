using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public enum Type { A,B,C};
    public Type enemyType;
    public int maxHealth;
    public int curHealth;//현재 체력
    public Transform Target;//ai의 표적물
    public BoxCollider meleeArea;//콜라이더를 담을 변수 추가
    public GameObject bullet;
    //돼지 공격 소리
    [SerializeField] private AudioClip sound_pig_attack;
    private AudioSource theAudio;

    public bool isChase;//추적을 결정하는 변수 추가
    public bool isAttack;

    Rigidbody rigid;
    BoxCollider boxCollider;
    Material mat;

    NavMeshAgent nav;//ai만들기
    public Animator anim;

    


    //초기화
    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        mat = GetComponentInChildren<SkinnedMeshRenderer>().material;//material은 mesh Renderer컴포넌트에서 접근 가능
        nav = GetComponent<NavMeshAgent>();//ai 만들기 초기화
        anim = GetComponentInChildren<Animator>();
        theAudio = GetComponent<AudioSource>();


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
        {
            nav.SetDestination(Target.position);//목표물 추적하기
            nav.isStopped = !isChase;
        }
        
    }

    void FixedUpdate()
    {
        FreezeVelocity();
        Targerting();
    }

    void Targerting()
    {

        float targetRadius =0;
        float targetRange=0;
        
        switch (enemyType)
        {
            case Type.A:
                targetRadius = 1.5f;
                targetRange = 3f;

                break;
            case Type.B:
                targetRadius = 1f;
                targetRange = 6f;
                break;
            case Type.C:
                targetRadius = 0.5f;
                targetRange = 25f;

                break;

        }

        RaycastHit[] rayHits =
            Physics.SphereCastAll(transform.position, targetRadius, transform.forward, targetRange, LayerMask.GetMask("Player"));

        if(rayHits.Length>0 && !isAttack)
        {
            StartCoroutine(Attack());
            PlaySE(sound_pig_attack);

        }
    }

    IEnumerator Attack()
    {
        isChase = false;
        isAttack = true;
        anim.SetBool("Attack", true);

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
                rigid.AddForce(transform.forward * 20, ForceMode.Impulse);
                meleeArea.enabled = true;
                yield return new WaitForSeconds(0.5f);
                rigid.velocity = Vector3.zero;
                meleeArea.enabled = false;
                
                yield return new WaitForSeconds(1f);
                break;
            case Type.C:
                yield return new WaitForSeconds(0.5f);
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



    void FreezeVelocity()
    {
        if (isChase)
        {
            rigid.velocity = Vector3.zero;
            rigid.angularVelocity = Vector3.zero;
        }
    }


    //날라오는 총알에 맞아야 함
    //총알을 맞을때 데미지만큼 피가 까짐
    void OnTriggerEnter(Collider other)
    {
        Bullet bullet = other.GetComponent<Bullet>();
        curHealth -= bullet.damage;

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
            Destroy(gameObject, 4);
        }
    }

    private void PlaySE(AudioClip _clip)
    {
        theAudio.clip = _clip;
        theAudio.Play();

    }

}
