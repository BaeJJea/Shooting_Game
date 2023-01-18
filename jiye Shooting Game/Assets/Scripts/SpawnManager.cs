using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public int enemyCount;
    public int maxCount;
    public float spawnTime = 3f;//몬스터 생성 주기
    public float curTime;
    public Transform[] spawnPoints;
    [SerializeField]
    private GameObject[] enemys;

    public GameObject player;

    private void Update()
    {
        if (curTime >= spawnTime && enemyCount < maxCount)
        {
            int x = Random.Range(0, spawnPoints.Length);
            int y = Random.Range(0, enemys.Length);
            SpawnEnemy(x, y);
        }
        curTime += Time.deltaTime;
    }

    public void SpawnEnemy(int ranNum, int RandomNum)
    {
        curTime = 0;
        enemyCount++;
        GameObject instantEmeny = Instantiate(enemys[RandomNum], spawnPoints[ranNum]);
        ForEnemy enemy = instantEmeny.GetComponent<ForEnemy>();
        enemy.target = player.transform;
        

    }
}
