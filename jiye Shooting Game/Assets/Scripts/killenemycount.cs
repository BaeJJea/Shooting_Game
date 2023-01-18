using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class killenemycount : MonoBehaviour
{
    public int count;
    public static killenemycount _instance;

    [SerializeField]
    private Text killedcount;

    // Start is called before the first frame update
    void Start()
    {
        _instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        Checkcount();
    }

    private void Checkcount()
    {
        killedcount.text = count.ToString();
    }
}
