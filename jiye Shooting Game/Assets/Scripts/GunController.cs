using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    //현재 장착된 총
    [SerializeField]
    private Gun currentGun;



    //연사 속도 계산
    private float currentFireRate; //1초에 1씩 깍기 fire rate가 0이 되면 발사 할 수 있게 하기 발사하면 다시 연산

    //상태 변수
    private bool isReload = false; //상태 변수 false일때만 발사가 이루어짐.

    //효과음 재생
    private AudioSource audioSource;//소리 소스(선언)

    //레이저 충돌 정보 받아옴
    private RaycastHit hitInfo;

    //카메라 시점의 정가운데에 쏘는 것이기 때문에 카메라의 정보를 받아옴
    [SerializeField]
    private Camera theCam;

    //피격 이펙트
    [SerializeField]
    private GameObject hit_effect_prefab;

    public Transform bulletPos;//프리펩을 생성해야할 위치
    public GameObject bullet;//프리펩을 저장할 변수

    //정조준
    [SerializeField] private Vector3 originPos;//정조준후 원래로 돌아오기위한 벡터값
    private bool isFineSigntMode = false;



    public Camera firstPersonCamera;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();//선언한 것에 요소를 넣어줌
        WeaponManager.currentWeapon = currentGun.GetComponent<Transform>();
        WeaponManager.currentWeaponAnim = currentGun.anim;
       
        
    }

    // Update is called once per frame
    void Update()
    {
        GunFireRateCalc();
        TryFire();
        TryReload();//수동 재장전
        TryFindSight();
    }

    private void TryFindSight()
    {
        if (Input.GetButtonDown("Fire2") && firstPersonCamera.enabled)
        {
            FineSight();
        }
    }

    private void FineSight()
    {
        isFineSigntMode = !isFineSigntMode;
        currentGun.anim.SetBool("FineSightMode", isFineSigntMode);

        if (isFineSigntMode)
        {
            StopAllCoroutines();
            StartCoroutine(FineSightActivateCoroutine());
        }
        else
        {
            StopAllCoroutines();
            StartCoroutine(FineSightDeactivateCoroutine());
        }

    }

    IEnumerator FineSightActivateCoroutine()
    {
        while(currentGun.transform.localPosition != currentGun.fineSightOrignPos)
        {
            currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, currentGun.fineSightOrignPos, 0.2f);
            yield return null;
        }
    }

    IEnumerator FineSightDeactivateCoroutine()
    {
        while (currentGun.transform.localPosition != originPos)
        {
            currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, originPos, 0.2f);
            yield return null;
        }
    }

    //연사속도 재계산
    private void GunFireRateCalc()
    {
        if (currentFireRate > 0)
            currentFireRate -= Time.deltaTime; //deltaTime: 1초의 역수 값을 가짐 60분의 1  1초에 1씩 감소 시킴

    }

    //발사 시도
    private void TryFire()
    {//누르면 발사
        if (Input.GetButton("Fire1")&&currentFireRate <=0 && !isReload)//isReload가 false일때만 발사
        {
            Fire();
        }
    }


    //발사 전 계산
    private void Fire()
    {
        if (!isReload)
        {
            if (currentGun.currentBulletCount > 0)
                Shoot();
            else
                StartCoroutine(ReloadCoroutine());
        }
    }


//발사 후 계산
    //발사하면 총구에 이펙트가 나감
    private void Shoot()
    {
        StartCoroutine("Shot");
        currentGun.currentBulletCount--;//발사하고 난 뒤 총알을 하나 줄임.
        currentFireRate = currentGun.fireRate;//연사속도 재계산, 발사가 이루어 지고 나서 재계산이 일어남
        PlaySE(currentGun.fire_Sound);
        currentGun.muzzleFlash.Play();
        Hit();//쏘는 족족히 맞추게하기 총이 쏘는 속도는 빠르기 때문, 각각 총알을 생성하고 내뱉게 할려면 오브젝트 풀링이라는 기법이 필요
        

    }

    IEnumerator Shot()
    {
        //총알 발사
        //총알 생성
        GameObject intantBullet = Instantiate(bullet, bulletPos.position, bulletPos.rotation);
        Rigidbody bulletRigid = intantBullet.GetComponent<Rigidbody>();//인스턴스화된 총알에 속도 적용하기
        bulletRigid.velocity = bulletPos.forward * 100;

        yield return null;
    }


    private void Hit()
    { //현재 자기 위치에서 레이저를 앞으로 발사를 함, 발사되면 반환 되는 것을 hitinfo에 저장,사정거리
        if (Physics.Raycast(theCam.transform.position, theCam.transform.forward, out hitInfo, currentGun.range))
        {
            //Debug.Log(hitInfo.transform.name); 맞춘 물건 이름 뜨게
            //타격 위치에 피격 이펙트 뜨기
            GameObject clone = Instantiate(hit_effect_prefab, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
            //var:변수, 반환되는 타입을 모를때 쓰면 됨 ->var clone
            Destroy(clone, 2f);//2초 후에 클론이 삭제됨
        }
    }


    //사운드 재생
    private void PlaySE(AudioClip _clip)
    {
        audioSource.clip = _clip;//mp3 플레이 곡을 넣고
        audioSource.Play();//플레이
       
    }
    //수동 재장전
    //K키를 눌렀을때, 재장전 중일때는 재장전 안되게, 현재 총알이 재장전 총알 보다 작을때만 ->재장전 가능
    private void TryReload()
    {
       
        if(Input.GetKeyDown(KeyCode.R)&&!isReload && currentGun.currentBulletCount < currentGun.reloadBulletCount)
        {
            StartCoroutine(ReloadCoroutine());
        }
    }

    public void CancelReload()
    {
        if(isReload)
        {//isReload가 true일때만 cancel이 되게 해줌
            StopAllCoroutines();
            isReload = false;
        }
    }

    //자동
    //재장전 총알 
    //재장전 도중에 발사를 못하도록 코루틴으로 하기
   IEnumerator ReloadCoroutine()
    {
        if(currentGun.carryBullectCount>0)
        {
            isReload = true; //true로 바꿈으로써 발사를 못하게 함
            currentGun.anim.SetTrigger("Reload");

            currentGun.carryBullectCount += currentGun.currentBulletCount;//현재 소유한 총알이 남았을때 나머지 만큼만 재장전 하기
            currentGun.currentBulletCount = 0;
            yield return new WaitForSeconds(currentGun.reloadTime);

            if(currentGun.carryBullectCount >= currentGun.reloadBulletCount)
            {
                currentGun.currentBulletCount = currentGun.reloadBulletCount;
                currentGun.carryBullectCount -= currentGun.reloadBulletCount;
            }
            else
            {
                currentGun.currentBulletCount = currentGun.carryBullectCount;
                currentGun.carryBullectCount = 0;
            }

            isReload = false;
        }
        else
        {
            Debug.Log("소유한 총알이 없습니다.");
        }

    }

    //getgun를 이용하여 currentGun 값을 불러 올 수 있음
    public Gun GetGun()
    {
        return currentGun;
    }

    public void GunChange(Gun _gun)
    {
        if (WeaponManager.currentWeapon != null)//무언가를 들고 있을 경우->기존에 있는 것은 비활성화 후 바꿔줌
            WeaponManager.currentWeapon.gameObject.SetActive(false); //기존꺼 false로 해서 비활성화 시킴

        currentGun = _gun;
        WeaponManager.currentWeapon = currentGun.GetComponent<Transform>();
        WeaponManager.currentWeaponAnim = currentGun.anim;

        currentGun.transform.localPosition = Vector3.zero;//총의 경우 정조준이 있음 무기를 바꾸면 바뀔 수 도 있음 그래서 000으로 초기화 시킴
        currentGun.gameObject.SetActive(true);//currentgun을 활성화 시킴
    }

}
