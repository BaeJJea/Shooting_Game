using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{

    [SerializeField]
    private GunController theGunController;
    private Gun currentGun;
    //필요하면 HUB 호출, 필요없으면 HUB 비활성화
    [SerializeField]
    private GameObject go_BulletHUD;

    //총알 개수 텍스트에 반영
    [SerializeField]
    private Text[] text_Bullet;


   
    // Update is called once per frame
    void Update()
    {
        CheckBullet();
    }

    private void CheckBullet()
    {
        currentGun = theGunController.GetGun();
        text_Bullet[0].text = currentGun.carryBullectCount.ToString();//숫자를 글자값으로 변환해줌
        text_Bullet[1].text = currentGun.reloadBulletCount.ToString();
        text_Bullet[2].text = currentGun.currentBulletCount.ToString();
    }
}
