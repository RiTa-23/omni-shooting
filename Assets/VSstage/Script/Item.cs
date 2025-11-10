using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] AudioClip itemGetSE;
    //[SerializeField] Sprite speedUp_img;
    //[SerializeField] Sprite maxSpeedUp_img;
    [SerializeField] Sprite maxEnergyUp_img;
    [SerializeField] Sprite rapidFireUp_img;
    [SerializeField] Sprite energySpeedUp_img;
    [SerializeField] Sprite lifeRecovery_img;
    [SerializeField] Sprite invincible_img;
    [SerializeField] Sprite infinity_img;

    //アイテム出現確率
    //[SerializeField] int speedUp_P = 15;
    //[SerializeField] int maxSpeedUp_P = 15;
    int maxEnergyUp_P = 25;
    int rapidFireUp_P = 25;
    int energySpeedUp_P = 25;
    int lifeRecovery_P = 15;
    int invincible_P = 5;
    int infinity_P = 5;

    //アイテムの効果を変数で決定
    private int maxEnergyUp_value = 40;
    private float rapidFireUp_value = 0.05f;
    private float energySpeedUp_value = 10f;
    private int lifeRecovery_value = 30;


    void Start()
    {
        SpriteRenderer spriteRenderer= GetComponent<SpriteRenderer>();

        //何のアイテムになるかランダムに決める
        int rnd = UnityEngine.Random.Range(1, 101);//1~100の範囲でランダムな整数値

        //乱数からアイテム決定
        //アイテムごとに見た目を変える
        //if (rnd <= speedUp_P)
        //{
        //    thisItemName = ItemName.speedUp;
        //    spriteRenderer.sprite = speedUp_img;
        //}
        //else if(rnd<=speedUp_P+maxSpeedUp_P)
        //{
        //    thisItemName = ItemName.maxSpeedUp;
        //    spriteRenderer.sprite = maxSpeedUp_img;
        //}
        if(rnd<=  maxEnergyUp_P)
        {
            thisItemName = ItemName.maxEnergyUp;
           spriteRenderer.sprite= maxEnergyUp_img;
        }
        else if(rnd<=  maxEnergyUp_P+rapidFireUp_P)
        {
            thisItemName = ItemName.rapidFireUp;
            spriteRenderer.sprite = rapidFireUp_img;
        }
        else if(rnd<=  maxEnergyUp_P + rapidFireUp_P+energySpeedUp_P)
        {
            thisItemName = ItemName.energySpeedUp;
            spriteRenderer.sprite= energySpeedUp_img;
        }
        else if(rnd<=  maxEnergyUp_P + rapidFireUp_P + energySpeedUp_P+lifeRecovery_P)
        {
            thisItemName = ItemName.lifeRecovery;
            spriteRenderer.sprite=lifeRecovery_img;
        }
        else if(rnd<= maxEnergyUp_P + rapidFireUp_P + energySpeedUp_P + lifeRecovery_P+invincible_P)
        {
            thisItemName = ItemName.invincible;
            spriteRenderer.sprite = invincible_img;
        }
        else if(rnd<=  maxEnergyUp_P + rapidFireUp_P + energySpeedUp_P + lifeRecovery_P+invincible_P+infinity_P)
        {
            thisItemName = ItemName.infinity;
            spriteRenderer.sprite=infinity_img;
        }
        
        //時間経過で消滅
        Destroy(this.gameObject, 30f);

    }

    public enum ItemName
    {
        //speedUp,//速度UP
        //maxSpeedUp,//最大速度UP（主にドッジで役立つ）
        maxEnergyUp,//最大エネルギー上昇
        rapidFireUp,//連射力UP
        energySpeedUp,//エネルギー回復スピードUP
        lifeRecovery,//ライフ回復
        invincible,//無敵（一定時間ダメージ無効）
        infinity,//無限（一定時間エネルギー使い放題）
    }
    [SerializeField]ItemName thisItemName;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //ログの追加
            TextMeshProUGUI LogText;
            LogText = GameObject.Find("LogText").GetComponent<TextMeshProUGUI>();

            GameObject player = collision.gameObject;
            int playerNum = player.GetComponent<PlayerStatus>().P_Num + 1;

            //アイテム数が上限に達していない（5個まで）
            //もしくはアイテムリストに存在しない（上限がない）ならアイテムを取得する
            PlayerItem playerItem=player.GetComponent<PlayerItem>();
            var ItemList = playerItem.ItemList;
            if (!ItemList.ContainsKey(thisItemName) || ItemList[thisItemName]<5)
            {
                //アイテムリストに追加
                if(ItemList.ContainsKey(thisItemName))
                {
                    playerItem.AddItem(thisItemName);
                    //アイテムゲットのログ追加
                    LogText.text += "player" + playerNum + " ： " + thisItemName + " Lv." + ItemList[thisItemName] + "\n";
                }
                else
                {
                    //アイテムログ追加
                    LogText.text += "player" + playerNum + " ： " + thisItemName +"\n";
                }


                PlayerStatus playerStatus = player.GetComponent<PlayerStatus>();
                PlayerController playerController = player.GetComponent<PlayerController>();
                //効果音・エフェクト
                AudioSource audioSource = player.GetComponent<AudioSource>();
                audioSource.PlayOneShot(itemGetSE);
                //アイテムの効果
                switch (thisItemName)
                {
                    //case ItemName.speedUp: playerController.force += 2.5f; break;
                    //case ItemName.maxSpeedUp: playerController.maxSpeed += 2; break;
                    case ItemName.maxEnergyUp: playerStatus.MaxEnergy += maxEnergyUp_value; break;
                    case ItemName.energySpeedUp: playerStatus.energyNaturalRecovery += energySpeedUp_value; break;
                    case ItemName.rapidFireUp:
                        if (playerController.intervalTime > 0.02)
                            playerController.intervalTime -= rapidFireUp_value; break;
                    case ItemName.lifeRecovery: playerStatus.HPUpdate(lifeRecovery_value); break;
                    case ItemName.invincible: playerStatus.Invincible();break;
                    case ItemName.infinity: playerStatus.Infinity();break;
                }
                //消滅する
                Destroy(gameObject);
            }
        }
    }
}