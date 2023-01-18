using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : MonoBehaviour
{
    //현재 장착된 Hand형 타입 무기
    [SerializeField]
    private Hand currentHand;

    //공격중??
    private bool isAttack = false;
    private bool isSwing = false;

    private RaycastHit hitInfo;// 광선을 쐈을때 닿은 녀석의 스크립트를 불러와서 체력을 깎거나 할수 있다.정보를 얻어 올수 있다.


    

    // Update is called once per frame
    void Update()
    {
        TryAttck();    
    }

    private void TryAttck()
    {
        //마우스 좌클릭을 누르면 코루틴 실행 -> isAttack이 true가 됨-> 중복 실행을 막음
        if (Input.GetButton("Fire1"))
        {
            if (!isAttack)
            {
                //코루틴 실행.
                StartCoroutine(AttackCorouttine()); 
            }
        }
    }

    IEnumerator AttackCorouttine()
    {
        isAttack = true;
        currentHand.anim.SetTrigger("attack");

        // new WaitForSeconds(currentHand.attackDelayA) 동안 대기
        yield return new WaitForSeconds(currentHand.attackDelayA);
        isSwing = true;
        //공격이 적중했는지 않했는지 알 수 있는 코드를 코루틴을 짜기.
        StartCoroutine(HitCoroutine());
        //공격 활성화 시점
       


        // new WaitForSeconds(currentHand.attackDelayB) 동안 대기
        yield return new WaitForSeconds(currentHand.attackDelayB);
        isSwing = false;

        yield return new WaitForSeconds(currentHand.attackDelay- currentHand.attackDelayA- currentHand.attackDelayB);

        isAttack = false;
    }

    //공격 적중을 알아보는 코루틴
    IEnumerator HitCoroutine()
    {
        //isSwing이true가 됐을때 반복
        while(isSwing)
        {//직접적으로 확인
            if(CheckObject())
            {
                isSwing = false;//한번 적중하면 끝
                //충돌했음
                Debug.Log(hitInfo.transform.name);
            }
            yield return null; //1프레임 대기
        }

    }
    //checkobject를 조건문으로 썼기때문에 void가 아닌 bool로 이용-> return 값으로 true와false가 반환 되게
    private bool CheckObject()
    {
        //레이져를 쏴서 충돌체가 있을때 true반환
        if (Physics.Raycast(transform.position, transform.forward, out hitInfo, currentHand.range)){
            return true;//충돌한 것이 있다면 true반환
        }//현재 자기 위치 캐릭터 기준 앞transform.forward=transform.TransformDirection(Vector3.forward)같은 의미

        return false;
    }
}
