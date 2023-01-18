using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour
{
    public string sceneName = "GameStage";


    //씬 이동을 하면 기존의 것들을 파괴가 됨 ->싱글턴을 시켜 줘야한다
    public static Title instance;
    //로드 구현하기
    private SaveNLoad theSaveNLoad;

    


    private void Awake()
    {


        if (instance == null)
        {
            instance = this; //자기 자신 넣어주기
            DontDestroyOnLoad(gameObject);//자기 자신이 파괴 되면 안됨
        }

        else
            Destroy(this.gameObject);
    }


    public void ClickStart()
    {
        Debug.Log("로딩");
        SceneManager.LoadScene(sceneName);
        Destroy(gameObject);


    }

    public void ClickLoad()
    {
        Debug.Log("로드");
        StartCoroutine(LoadCoroutine());
        SceneManager.LoadScene(sceneName);//타이틀에는 플레이어가 없기 때문에 이 코드가 없으면 버그가 걸림

        theSaveNLoad.LoadData();
        

    }

    IEnumerator LoadCoroutine()
    {
        //다음 씬 넘어가는데 싱크 관련해서 무언가를 하겠다. 완전히 동기화가 일어날때 까지 대기 ∵동기화가 안일어나면 플레이어가 없는데 플레이어의 위치를 지정할려고 해서 오류 발생함
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);//타이틀에는 플레이어가 없기 때문에 이 코드가 없으면 버그가 걸림
        while (!operation.isDone)//로딩이 끝나느냐
        { //끝나지 않는다면 계속 대기
            yield return null;//로딩이 끝날때까지 대기하다가 로딩이 끝나면 yield문을 벗어남
        }
        theSaveNLoad = FindObjectOfType<SaveNLoad>();//다른 씬에 있는것을 찾게 하기
        theSaveNLoad.LoadData();//이게 이루어 지면 기존에 있던것이 파괴가 됨
        Destroy(gameObject);//SetActive(false)로 해주기 왜냐면 파괴되지 않게 dontdestroy 했기 때문
       
    }
    public void ClickExit()
    {
        Debug.Log("게임종료");
        Application.Quit();
    }


}
