using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{


    [SerializeField] private GameObject go_BaseUi;
    [SerializeField] private SaveNLoad theSaveNLoad;


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (!GameManager.isPause)
                CallMenu();
            else
                CloseMenu();
        }
    }
    private void CallMenu()
    {
        GameManager.isPause = true;
        go_BaseUi.SetActive(true);
        Time.timeScale = 0f;//게임을 0배속함 = 정지함
    }

    private void CloseMenu()
    {
        GameManager.isPause = false;
        go_BaseUi.SetActive(false);
        Time.timeScale = 1f;//게임을 1배속함 = 실행함
    }

    public void ClickSave()
    {
        Debug.Log("세이브");
        theSaveNLoad.SaveData();
    }
    public void ClickLoad()
    {
        Debug.Log("Load");
        theSaveNLoad.LoadData();
    }
    public void ClickExit()
    {
        Debug.Log("게임종료");
        Application.Quit();
    }




}
