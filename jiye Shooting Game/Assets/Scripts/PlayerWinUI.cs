using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class PlayerWinUI : MonoBehaviour
{
    [SerializeField] private GameObject go_BaseUi;
    // Start is called before the first frame update


    public string sceneName = "GameTitle";


    void Update()
    {
        if (killenemycount._instance.count == 10)
        {

            CallMenu();

        }

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

    public void ClickReStart()
    {
        Debug.Log("로딩");
        SceneManager.LoadScene(sceneName);
        Destroy(gameObject);


    }
}
