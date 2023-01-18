using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;//input output, data출력


[System.Serializable]//직렬화를 해야 백터값을 float값으로 바꿈->읽고 쓰기가 쉬움 데이터를 직렬화하면 한줄로 데이터들이 나열돼서 저장 장치에 읽고 쓰기가 쉬워짐
public class SaveData//data를 저장시킬 클래스
{
    public Vector3 playerPos;//플레이어의 위칫값을 기억하기위함
    public Vector3 playerRot;
}
public class SaveNLoad : MonoBehaviour
{
    private SaveData saveData = new SaveData();

    private string SAVE_DATA_DIRECTORY;//저장시킬 경로
    private string SAVE_FILENAME = "/SaveFile.txt";//저장시킬 경로

    private PlayerController thePlayer;


    // Start is called before the first frame update
    void Start()
    {
        SAVE_DATA_DIRECTORY = Application.dataPath + "/Saves/";

        if (!Directory.Exists(SAVE_DATA_DIRECTORY))//디렉토리가 없을때 만들어줌
        {
            Directory.CreateDirectory(SAVE_DATA_DIRECTORY);
        }
    }

    public void SaveData()// json을 이용하여 저장시킴
    {
        thePlayer = FindObjectOfType<PlayerController>();
        saveData.playerPos = thePlayer.transform.position;//플레어의 좌표값이 저장됨
        saveData.playerRot = thePlayer.transform.eulerAngles;

        string json = JsonUtility.ToJson(saveData);//플레이어의 위치를 json화 시킴

        File.WriteAllText(SAVE_DATA_DIRECTORY + SAVE_FILENAME, json);//실제 물리적인 파일로 저장| 텍스트를 전부다 기어시키기

        Debug.Log("저장 완료");
        Debug.Log(json);


    }

    public void LoadData()
    {
        if (File.Exists(SAVE_DATA_DIRECTORY + SAVE_FILENAME))
        {
            string loadJson = File.ReadAllText(SAVE_DATA_DIRECTORY + SAVE_FILENAME);//SAVE_DATA_DIRECTORY경로에 SAVE_FILENAME이 파일을 가져와서 읽음, 텍스트를 이용해서 꺼내오기 힘듦
            saveData = JsonUtility.FromJson<SaveData>(loadJson);//json화 된것을 다시 풀어줌savedata로


            thePlayer = FindObjectOfType<PlayerController>();

            thePlayer.transform.position = saveData.playerPos;//위치값을 playerpos 값으로 해줌
            thePlayer.transform.eulerAngles = saveData.playerRot;//player의 바라보는 방향 저장

            Debug.Log("로드 완료");
        }
        else
            Debug.Log("세이브 파일이 없습니다.");//세이브 파일이 없는데 load버튼을 누르면 불러 들일 것이 없음->버그가 발생 할 수 있음.

    }

}
