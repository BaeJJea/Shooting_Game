using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float walkSpeed;

    [SerializeField]
    private float runSpeed;

    public float finalSpeed;

    [SerializeField] private float jumpForce;//순간 얼마나의 힘으로 뛰어 오를것인가

    private bool isGround; //true즉 땅에 있을 때만 점프가 가능함

    //땅 착지 여부
    private CapsuleCollider capsuleCollider;

    [SerializeField]
    private float lookSensitivity;//카메라의 민감도

    [SerializeField]
    private float cameraRotationLimit;//설정하지 않을 시 고개를 들때 한바퀴 돌게됨

    private float currentCameraRotationX = 0; //0f -> 정면보기 45f : 45도 위를 바라좀

    [SerializeField]
    private Camera theCamera;//player에는 카메라 요소가 없음->그 안의 자식 개체에 있기 때문
    private StatusController thePlayerStatus;//플레이어 hp가져오기
    MeshRenderer[] meshs;

    //플레이어 피격시 소리 나오게
    [SerializeField] private AudioClip playerAttacked;
    private AudioSource theAudio;



    bool isDamage; //무적타임
    bool isBorder;
    public bool run;
    Animator anim;

    [SerializeField] private Image blood;

    private Rigidbody myRigid;
    // Start is called before the first frame update
    void Start()
    {
        //theCamera = FindObjectType<Camera>(); 이렇게도 할 수 있음
        myRigid = GetComponent<Rigidbody>();
        thePlayerStatus = FindObjectOfType<StatusController>();
        meshs = GetComponentsInChildren<MeshRenderer>();//여러개를 가져오기때문에 getcomponent's'
        theAudio = GetComponent<AudioSource>();
        anim = GetComponentInChildren<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider>();
   
    }

    // Update is called once per frame
    void Update()
    {
        IsGround();
        TryJump();

        Move();
       

        CameraRotation();
        CharacterRotation();

        if (Input.GetKey(KeyCode.LeftShift))
        {
            run = true;
        }
        else
        {
            run = false;
        }
        


    }

    private void IsGround()
    {
        isGround = Physics.Raycast(transform.position, Vector3.down, capsuleCollider.bounds.extents.y+0.1f);
        
    }

    private void TryJump()
    {
        if(Input.GetKeyDown(KeyCode.Space) && isGround == true)
        {
            
            Jump();
        }
    }

    private void Jump()
    {
        myRigid.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }


    void FixedUpdate()
    {
        FreezeVelocity();
    //    StopToWall();
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



    private void Move()
    {
        finalSpeed = (run) ? runSpeed : walkSpeed;
        float _moveDirX = Input.GetAxisRaw("Horizontal");//방향키 좌우 또는 a,d를 눌렀을때 -1,1이 반환되어 들어감. 안누르면 0이 리턴
        float _moveDirZ = Input.GetAxisRaw("Vertical");//정면과 뒤

        Vector3 _moveHorizontal = transform.right * _moveDirX; //transform.right:unity에서 위치 값(1,0,0) * [-1/0/1]
        Vector3 _moveVertical = transform.forward * _moveDirZ; //(0,0,1)

        Vector3 _velocity = (_moveHorizontal + _moveVertical).normalized * finalSpeed;
        //(1,0,0)+(0,0,1)=(1,0,1)->2->normalized->(0.5,0,0.5)합이 1이 나오면 유니티가 계산 하기 더 빠름
        myRigid.MovePosition(transform.position + _velocity * Time.deltaTime);
        //time.deltatime의 값은 약 0.016, deltaTime을 곱해 줌으로써 순간이동하여 움직이는 것이 아닌 점진적으로 움직임.


        anim.SetBool("isRun", _velocity != Vector3.zero);

    }

    private void CameraRotation()
    {
        //카메라 상하 회전
        float _xRotation = Input.GetAxisRaw("Mouse Y");//마우스는 3차원이 아님 x와y밖에 없음
        float _cameraRotationX = _xRotation * lookSensitivity;//천천히 움직이게
        currentCameraRotationX -= _cameraRotationX;//마우스와 시야가 반전 되었기 때문에 마이너스를 해줌
        currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit);//무작정 더하지 못하고 limit 걸리게 하기 위함
        //currentCamerRotationX를 -cameraRotationLimit와 cameraRotationLimit사이에 가둠(Clamp)

        theCamera.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);//currentCameraRotationX를 theCamera에 적용 시키기

    }

    private void CharacterRotation()
    {
        //좌우 캐릭터 회전
        float _yRotation = Input.GetAxisRaw("Mouse X");
        Vector3 _characterRotationY = new Vector3(0f, _yRotation, 0f) * lookSensitivity;
        myRigid.MoveRotation(myRigid.rotation * Quaternion.Euler(_characterRotationY));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "enemybullet")
        {
            if (!isDamage)
            {
                Bullet enemyBullet = other.GetComponent<Bullet>();
                thePlayerStatus.DecreaseHP(10);
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
}
