using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    public RectTransform uiGroup;
    public Animator anim;

    public GameObject[] itemObj;
    public int[] itemPrice;
    public int[] stUp;
    public Transform[] itemPos;
    public string[] talkData;
    public Text talkText;

    
    Player enterPlayer;


    
        
    
    public void Enter(Player player)
    {
        enterPlayer = player;
        uiGroup.anchoredPosition = Vector3.zero;
    }

    public void Exit()
    {
        anim.SetTrigger("doHello");
        uiGroup.anchoredPosition = Vector3.down * 1000;
    }

   
    public void Buy(int index)
    {
        int price = itemPrice[index];
        if(price > enterPlayer.coin)
        {
            StopCoroutine(Talk());
            StartCoroutine(Talk());
            return;
        }

        enterPlayer.coin -= price;
        Vector3 ranVec = Vector3.right * Random.Range(-3, 3) + Vector3.forward * Random.Range(-3, 3);
        Instantiate(itemObj[index], itemPos[index].position + ranVec, itemPos[index].rotation);     
    }

    public void StBuy(int index)
    {
        int price = itemPrice[index];
        if(price > enterPlayer.coin)
        {
            StopCoroutine(Talk());
            StartCoroutine(Talk());
            return;
        }

        if(index == 0) // 근접무기 공격력 증가
         {
             Debug.Log("응애");
             if(enterPlayer.equipWeapon.type == Type.Melee)
             {
                enterPlayer.equipWeapon.damage += 5;
                enterPlayer.coin -= price;
             }
              
         }      
         else if(index == 1) // 총기류 데미지 1 증가
         {
             if(enterPlayer.equipWeapon.type == Type.Range)
             {
                enterPlayer.equipWeapon.damage += 1;
                enterPlayer.coin -= price;
             }
         }
         else if(index == 2)
         {
             enterPlayer.maxHealth += 20;
             enterPlayer.health = enterPlayer.maxHealth;
             enterPlayer.coin -= price;
         }
    }

    
        

    IEnumerator Talk()
    {
        talkText.text = talkData[1];
        yield return new WaitForSeconds(2f);
        talkText.text = talkData[0];
    }
}
