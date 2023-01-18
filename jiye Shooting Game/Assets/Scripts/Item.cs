using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//아이템을 여러개 만들어 주면 됨
[CreateAssetMenu(fileName = "New Item", menuName = "New Item/item" )]
public class Item : ScriptableObject//굳이 gameobject에 붙이지 않아도 됨
{
    public string itemName;//아이템의 이름
    public ItemType itemType;//아이템 타입
    public Sprite itemImage;//아이템의 이미지 ※image는 캔버스에서만 띄울 수 있음, Sprite는 캔버스 필요없이 월드 상에서 띄울수 있음
    public GameObject itemPrefab;

    public string weaponType;//무기 유형

    public enum ItemType
    {
        Equipment,
        Used,
        Ingredient,
        ETC
    }
   
}
