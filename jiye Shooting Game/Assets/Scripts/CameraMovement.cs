using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform objectTofollow;
    public float followspeed = 10f;
    public float sensitivity = 100f;
    public float clampAngle = 70f;

    //마우스 인풋을 받을 변수
    private float rotx;
    private float roty;

    public Transform realCamera;
    public Vector3 dirNormalized;//벡터는 크기와 방향 둘다 가지고 있지만 방향만 필요함
    public Vector3 finalDir;//최종적으로 정해진 방향을 저장해 줄 벡터 값
    public float minDistance;//최소거리
    public float maxDistance;//최대거리
    public float finalDistance;//최종거리
    public float smoothness = 10f;

    // Start is called before the first frame update
    void Start()
    {
        rotx = transform.localRotation.eulerAngles.x;
        roty = transform.localRotation.eulerAngles.y;

        dirNormalized = realCamera.localPosition.normalized;
        finalDistance = realCamera.localPosition.magnitude;

    }

    // Update is called once per frame
    void Update()
    {
        rotx += -(Input.GetAxis("Mouse Y")) * sensitivity * Time.deltaTime;//마지막 프레임에서 현재플레임까지 시간 간격을 보여줌
        roty += Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;

        rotx = Mathf.Clamp(rotx, -clampAngle, clampAngle);//수학 관련한 함수에서는 Mathf쓰기 //카메라 최대 움직이는 각도
        Quaternion rot = Quaternion.Euler(rotx, roty, 0);
        transform.rotation = rot;

    }

    void LateUpdate()//update가 끝난 다음에 되는 함수//카메라가 따라가야함 
    {
        transform.position = Vector3.MoveTowards(transform.position, objectTofollow.position, followspeed * Time.deltaTime);

        finalDir = transform.TransformPoint(dirNormalized * maxDistance);//로컬 스페이스에서 월드 스페이스로 바꿔줌
        RaycastHit hit;// 앞에 장애물이 있다면 카메라가 그 앞으로 가기

        if (Physics.Linecast(transform.position, finalDir, out hit))
        {
            finalDistance = Mathf.Clamp(hit.distance, minDistance, maxDistance);
        }
        else
        {
            finalDistance = maxDistance;

        }
        realCamera.localPosition = Vector3.Lerp(realCamera.localPosition, dirNormalized * finalDistance, Time.deltaTime * smoothness);

    }
}
