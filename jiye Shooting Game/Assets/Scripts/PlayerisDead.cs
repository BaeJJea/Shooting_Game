using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerisDead : MonoBehaviour
{
    [SerializeField] private GameObject go_BaseUi;
    // Start is called before the first frame update


    void Update()
    {
        if (StatusController._instance.currentHp <= 0)
        {

            StartCoroutine(CallingMenu());

        }

    }

    IEnumerator CallingMenu()
    {
        yield return new WaitForSeconds(2f);
        CallMenu();
    }
        

    private void CallMenu()
    {
        GameManager.isplywin = true;
        go_BaseUi.SetActive(true);
        Time.timeScale = 0f;//게임을 0배속함 = 정지함
    }



    public void ClickExit()
    {
        Debug.Log("게임종료");
        Application.Quit();
    }
}
