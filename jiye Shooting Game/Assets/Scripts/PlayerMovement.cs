using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlayerMovement : MonoBehaviour
{
    Animator anim;
    Camera _camera;
    // CharacterController _controller;

    public float speed = 5f;//캐릭터의 속도
    public float runSpeed = 8f;
    public float finalSpeed;

    [SerializeField] private float jumpForce;//순간 얼마나의 힘으로 뛰어 오를것인가

    private bool isGround; //true즉 땅에 있을 때만 점프가 가능함

    //땅 착지 여부
    private CapsuleCollider capsuleCollider;

    public bool toggleCameraRotation;//배그 같이 알트를 꾹 눌렀을때 둘러보기 가능한것 처럼
    public bool run;

    public float smoothness = 10f;

    private StatusController thePlayerStatus;//플레이어 hp가져오기
    MeshRenderer[] meshs;

    [SerializeField] private AudioClip playerAttacked;
    private AudioSource theAudio;

    bool isDamage;
    bool isBorder;


    private Rigidbody myRigid;
    // Start is called before the first frame update

    [SerializeField] private Image blood;

    

    void Start()
    {
        myRigid = GetComponent<Rigidbody>();
        theAudio = GetComponent<AudioSource>();
        thePlayerStatus = FindObjectOfType<StatusController>();
        meshs = GetComponentsInChildren<MeshRenderer>();//여러개를 가져오기때문에 getcomponent's'
        anim = GetComponentInChildren<Animator>();
        _camera = Camera.main;
        capsuleCollider = GetComponent<CapsuleCollider>();
        
        // _controller = this.GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {

        
        IsGround();
        TryJump();

        if (Input.GetKey(KeyCode.LeftControl))
        {
            toggleCameraRotation = true;//둘러보기 활성화
        }
        else
        {
            toggleCameraRotation = false;//둘러보기 비활성화
        }


        if (Input.GetKey(KeyCode.LeftShift))
        {
            run = true;
        }
        else
        {
            run = false;
        }

        if (!isBorder)
        {
            InputMovement();
        }


    }

    private void IsGround()
    {
        isGround = Physics.Raycast(transform.position, Vector3.down, capsuleCollider.bounds.extents.y + 0.1f);
        
    }

    private void TryJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGround == true)
        {

            Jump();
        }
    }

    private void Jump()
    {
        myRigid.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        anim.SetTrigger("Jump");
    }
    void LateUpdate()
    {
        //카메라와 관련되어 있기 때문
        if (toggleCameraRotation != true)
        {
            Vector3 playerRotate = Vector3.Scale(_camera.transform.forward, new Vector3(1, 0, 1));
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(playerRotate), Time.deltaTime * smoothness);//구 형태로 인터폴레이션 해줌
        }
    }

    void InputMovement()
    {
        finalSpeed = (run) ? runSpeed : speed;
        /*
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        Vector3 moveDirection = forward * Input.GetAxisRaw("Vertical") + right * Input.GetAxisRaw("Horizontal");//getaxisraw는 getaxis의 부드러움을 빼줌


        _controller.Move(moveDirection.normalized * finalSpeed * Time.deltaTime);//Move():유니티에서 애용하는 api 
  */
        float _moveDirX = Input.GetAxisRaw("Horizontal");//방향키 좌우 또는 a,d를 눌렀을때 -1,1이 반환되어 들어감. 안누르면 0이 리턴
        float _moveDirZ = Input.GetAxisRaw("Vertical");//정면과 뒤

        Vector3 _moveHorizontal = transform.right * _moveDirX; //transform.right:unity에서 위치 값(1,0,0) * [-1/0/1]
        Vector3 _moveVertical = transform.forward * _moveDirZ; //(0,0,1)

        Vector3 _velocity = (_moveHorizontal + _moveVertical).normalized * finalSpeed;
        myRigid.MovePosition(transform.position + _velocity * Time.deltaTime);

        anim.SetBool("isRun", _velocity != Vector3.zero);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "enemybullet")
        {
            if (!isDamage)
            {
                Bullet enemyBullet = other.GetComponent<Bullet>();
                thePlayerStatus.DecreaseHP(10);

                blood.enabled = true;

                PlaySE(playerAttacked);

                StartCoroutine(OnDamage());
            }
            if (other.GetComponent<Rigidbody>() != null)
                Destroy(other.gameObject);



        }
    }


    

    IEnumerator OnDamage()
    {
        isDamage = true;

        foreach (MeshRenderer mesh in meshs)
        {
            mesh.material.color = Color.red;

        }

        blood.enabled = true;

        yield return new WaitForSeconds(1f);
        isDamage = false;
        foreach (MeshRenderer mesh in meshs)
        {
            mesh.material.color = Color.white;

        }

        blood.enabled = false;
    }



    private void PlaySE(AudioClip _clip)
    {
        theAudio.clip = _clip;
        theAudio.Play();

    }

    void FixedUpdate()
    {
        FreezeVelocity();
        StopToWall();
    }

    void StopToWall()
    {
        Debug.DrawRay(transform.position, transform.forward * 5, Color.green);
        isBorder = Physics.Raycast(transform.position, transform.forward, 5, LayerMask.GetMask("wall"));

    }
    void FreezeVelocity()
    {
        myRigid.velocity = Vector3.zero;
        myRigid.angularVelocity = Vector3.zero;
    }
}
