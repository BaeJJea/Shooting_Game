using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    //static : 공유 자원. 클래스 변수 = 정적 변수| 장점 : 쉽게 접근 가능함, 단점: 보호 수준이 떨어짐, 메모리가 낭비 될 수 있음
    // 무기 중복 교체 실행 방지.
    public static bool isChangeWeapon = false;
    //현재 무기와 현재 무기의 애니메이션
    public static Transform currentWeapon;//기존의 무기를 껐다 키는 기능
    public static Animator currentWeaponAnim;

    //현재 무기의 타입
    [SerializeField]
    private string currentWeaponType;

    //무기 교체 딜레이 타임
    [SerializeField]
    private float changeWeaponDelayTime;
    //무기 교체가 완전히 끝난 시점
    [SerializeField]
    private float changeWeaponEndDelayTime;

    //무기 종류들 전부 관리
    [SerializeField]
    private Gun[] guns;

    //관리 차원에서 쉽게 무기 접근이 가능하도록 만듦.
    private Dictionary<string, Gun> gunDictionary = new Dictionary<string, Gun>();

    //필요한 컴포넌트
    [SerializeField]
    private GunController theGunController;
   

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < guns.Length; i++)
        {
            gunDictionary.Add(guns[i].gunName, guns[i]);//ex)키 값으로 서브머신건이 들어가고 value로 자기 자신이 들어감
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        //1,2,3,4번 누르면 무기 교체하기
        if(!isChangeWeapon)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
                StartCoroutine(ChangeWeaponCoroutine("GUN", "SubMachineGun1"));//1번이 눌리면 무기 교체 실행ak47
            else if (Input.GetKeyDown(KeyCode.Alpha2))
                StartCoroutine(ChangeWeaponCoroutine("GUN", "SubMachineGun2")); ;//2번이 눌리면 맨손 실행

        }
    }

    
    public IEnumerator ChangeWeaponCoroutine(string _type, string _name)//string _type: 총이냐 맨손이냐string _name: 어떤 무기로 바꿀지
    {
        isChangeWeapon = true;
        currentWeaponAnim.SetTrigger("weapon out");

        yield return new WaitForSeconds(changeWeaponDelayTime);

        theGunController.CancelReload();
        WeaponChange(_type, _name);
        yield return new WaitForSeconds(changeWeaponEndDelayTime);//무기를 넣고 꺼내는 애니메이션을 기다린 후에 변경

        currentWeaponType = _type;
        isChangeWeapon = false;
    }

    private void WeaponChange(string _type, string _name)
    {
        if(_type == "GUN")
        {
            theGunController.GunChange(gunDictionary[_name]);
        }
    }

}
