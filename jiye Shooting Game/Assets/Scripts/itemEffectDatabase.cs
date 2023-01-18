using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class ItemEffect
{
    public string itemName;//아이템의 이름(키값)
    public string[] part;//부위
    public int[] num; //수치
}
public class itemEffectDatabase : MonoBehaviour
{
    [SerializeField]
    private ItemEffect[] itemEffects;

    //필요한 컴포넌트 불러오기
    private StatusController thePlayerStatus;

    private const string HP = "HP";

    public void UseItem(Item _item)
    {
        if(_item.itemType == Item.ItemType.Used)//넘어오는 아이템이 소모품 이라면
        {
            for (int x = 0; x < itemEffects.Length; x++)
            {
                if(itemEffects[x].itemName == _item.itemName)//일치하는 것이 있다면 회복 시키기
                {
                    for (int y = 0; y < itemEffects[x].part.Length; y++)
                    {
                        switch (itemEffects[x].part[y])
                        {
                            case HP:
                                thePlayerStatus.IncreaseHP(itemEffects[x].num[y]);
                                break;
                            default:
                                Debug.Log("잘못된 Status 부위");
                                break;
                            
                        }
                    }
                    return;
                }
                
            }
            Debug.Log("일치하는 itemName이 없습니다.");
        }
    }

}
