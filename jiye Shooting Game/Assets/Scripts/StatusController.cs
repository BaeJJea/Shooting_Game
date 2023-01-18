using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusController : MonoBehaviour
{
    [SerializeField] private int maxhp;//최대치 hp
    public int currentHp;//현재 hp
    private GameManager dead;

    //필요한 이미지
    [SerializeField] private Image[] images_Gauge;

    private const int HP = 0;

    public static StatusController _instance;

    [SerializeField] private Animator anim;
    [SerializeField] private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        currentHp = maxhp;
        _instance = this;
        dead = FindObjectOfType<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        GaugeUpdate();
    }

    private void GaugeUpdate()
    {
        images_Gauge[HP].fillAmount = (float)currentHp / maxhp;

    }

    public void IncreaseHP(int _count)
    {
        if (currentHp + _count < maxhp)
            currentHp += _count;
        else
            currentHp = maxhp;
        Debug.Log(currentHp);
    }

    public void DecreaseHP(int _count)
    {
        currentHp -= _count;
        Debug.Log(currentHp);
        if (currentHp <= 0)
        {
            anim.SetTrigger("Dead");
            player.layer = 19;
            dead.ThirdPersonView();
            Debug.Log("플레이어 죽음");
        }
    }


}
