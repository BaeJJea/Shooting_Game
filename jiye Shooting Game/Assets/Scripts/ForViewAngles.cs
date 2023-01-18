using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class ForViewAngles : MonoBehaviour
{
    [SerializeField] private float viewAngle; //시야각(120도)
    [SerializeField] private float viewDistance; //시야 거리 (10도)
    [SerializeField] private LayerMask targetMask;// 타겟마스크(플레이어)





    private Test_Pig thePig;



    NavMeshAgent nav;

    void Start()
    {
        nav = GetComponent<NavMeshAgent>();

    }

    // Update is called once per frame
    void Update()
    {
        //매 프레임 마다 시야각 설정
        View();


    }

    private Vector3 BoundaryAngle(float _angle)
    {
        _angle += transform.eulerAngles.y;
        return new Vector3(Mathf.Sin(_angle * Mathf.Deg2Rad), 0f, Mathf.Cos(_angle * Mathf.Deg2Rad));
    }
    private void View()
    {
        Vector3 _leftBoundary = BoundaryAngle(-viewAngle * 0.5f);
        Vector3 _rightBoundary = BoundaryAngle(viewAngle * 0.5f);

        Debug.DrawRay(transform.position + transform.up, _leftBoundary, Color.red);
        Debug.DrawRay(transform.position + transform.up, _rightBoundary, Color.red);

        //시야 안에 있는 모든 객체들을 저장 시킬꺼임
        //overlapSphere: 주변에 있는 컬라이더들을 뽑아내서 저장시키는데 사용
        //                  일정 반경 안에 있는 모든 객체들을 _target 안에 저장
        Collider[] _target = Physics.OverlapSphere(transform.position, viewDistance, targetMask);
        //모든 정보가 target에 들어감 -> 분류하기
        for (int i = 0; i < _target.Length; i++)
        {
            //타겟의 길이만큼 반복 시켜줌
            Transform _targetTf = _target[i].transform;
            if (_targetTf.name == "Player")//시야거리 내에 플레이어가 있다면
            {//시야각 내에 있는지도 확인해야됨
                Vector3 _direction = (_targetTf.position - transform.position).normalized;
                float _angle = Vector3.Angle(_direction, transform.forward);

                //시야각 반 안에 _angle이 들어왔을때
                if (_angle < viewAngle * 0.5f)
                {
                    RaycastHit _hit;//정보를 받아줄 곳

                    //장애물이 있는지 없는지 알아내기, 레이져를 시야거리까지만 쏘기
                    if (Physics.Raycast(transform.position + transform.up, _direction, out _hit, viewDistance))
                    {
                        if (_hit.transform.name == "Player")
                        {
                            Debug.Log("감지됨");
                            Debug.DrawRay(transform.position + transform.up, _direction, Color.blue);



                            nav.enabled = true;
                        }
                        else
                        {
                            nav.enabled = false;

                        }

                    }
                }
            }
        }
    }
}
