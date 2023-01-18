using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage;
    public bool isMelee;//근접일때 벽에 닿으면 사라지면 안됨


    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "terrain" || other.gameObject.tag == "wall" || other.tag == "Player")
        {
            if (!isMelee)
            {
                Destroy(gameObject);
            }

        }
    }


}
