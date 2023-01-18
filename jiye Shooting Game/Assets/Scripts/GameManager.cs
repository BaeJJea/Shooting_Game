using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static bool canPlayerMove = true;//플레이어의 움직임 제어

    public static bool isPause = false;//메뉴가 호출되면 true가 됨
    public static bool isplywin = false;
    public static bool isFirstcamera = false;

    

    public Camera firstPersonCamera;
    public AudioListener firstPersonLister;
    public Camera WeaponPersonCamera;



    public Camera thirdPersonCamera;
    public AudioListener thirdPersonLister;


    public GameObject gameobject;
    public GameObject thirdcamera;

    [SerializeField] private GameObject CrosshairUi;

    Rigidbody P_rigidbody;

    public Camera Map;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;//마우스 가운데 놓고 잠금 ※사실 이때 visible도 자동 false 됨
        Cursor.visible = false;// 시각화를 위해서 추가
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.M))
        {
            Map.enabled = true;
        }
        else
        {
            Map.enabled = false;
        }
        
        if (isPause || isplywin)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            canPlayerMove = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            canPlayerMove = true;
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (!GameManager.isFirstcamera)
            {

                ThirdPersonView();
            }
            else
            {


                FirstPersonView();
            }

        }
    }

    public void ThirdPersonView()
    {
        GameManager.isFirstcamera = true;

        firstPersonCamera.enabled = false;
        firstPersonLister.enabled = false;
        thirdPersonLister.enabled = true;
        WeaponPersonCamera.enabled = false;
        thirdPersonCamera.enabled = true;
        gameobject.GetComponent<PlayerMovement>().enabled = true;
        gameobject.GetComponent<PlayerController>().enabled = false;
        thirdcamera.GetComponent<CameraMovement>().enabled = true;
        CrosshairUi.SetActive(false);

    }

    private void FirstPersonView()
    {
        GameManager.isFirstcamera = false;


        firstPersonLister.enabled = true;
        thirdPersonLister.enabled = false;



        firstPersonCamera.enabled = true;
        WeaponPersonCamera.enabled = true;
        thirdPersonCamera.enabled = false;

        gameobject.GetComponent<PlayerMovement>().enabled = false;
        gameobject.GetComponent<PlayerController>().enabled = true;
        thirdcamera.GetComponent<CameraMovement>().enabled = false;
        CrosshairUi.SetActive(true);
    }
}
