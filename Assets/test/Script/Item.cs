using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Search;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] AudioClip itemGetSE;
    [SerializeField] Sprite speedUp_img;
    [SerializeField] Sprite maxSpeedUp_img;
    [SerializeField] Sprite maxEnergyUp_img;
    [SerializeField] Sprite rapidFireUp_img;
    [SerializeField] Sprite energyRecoverySpeedUp_img;
    [SerializeField] Sprite life_img;
    [SerializeField] Sprite invincible_img;
    [SerializeField] Sprite infinity_img;

    [SerializeField] int speedUp_P = 15;
    [SerializeField] int maxSpeedUp_P = 15;
    [SerializeField] int maxEnergyUp_P = 15;
    [SerializeField] int rapidFireUp_P = 15;
    [SerializeField] int energyRecoverySpeedUp_P = 15;
    [SerializeField] int life_P = 15;
    [SerializeField] int invincible_P = 5;
    [SerializeField] int infinity_P = 5;

    // Start is called before the first frame update

    void Start()
    {
        SpriteRenderer spriteRenderer= GetComponent<SpriteRenderer>();

        //何のアイテムになるかランダムに決める
        int rnd = UnityEngine.Random.Range(1, 101);//1~100の範囲でランダムな整数値

        //乱数からアイテム決定
        //アイテムごとに見た目を変える
        if (rnd <= speedUp_P)
        {
            thisItemName = ItemName.speedUp;
            spriteRenderer.sprite = speedUp_img;
        }
        else if(rnd<=speedUp_P+maxSpeedUp_P)
        {
            thisItemName = ItemName.maxSpeedUp;
            spriteRenderer.sprite = maxSpeedUp_img;
        }
        else if(rnd<= speedUp_P + maxSpeedUp_P+ maxEnergyUp_P)
        {
            thisItemName = ItemName.maxEnergyUp;
           spriteRenderer.sprite= maxEnergyUp_img;
        }
        else if(rnd<= speedUp_P + maxSpeedUp_P + maxEnergyUp_P+rapidFireUp_P)
        {
            thisItemName = ItemName.rapidFireUp;
            spriteRenderer.sprite = rapidFireUp_img;
        }
        else if(rnd<= speedUp_P + maxSpeedUp_P + maxEnergyUp_P + rapidFireUp_P+energyRecoverySpeedUp_P)
        {
            thisItemName = ItemName.energyRecoverySpeedUp;
            spriteRenderer.sprite= energyRecoverySpeedUp_img;
        }
        else if(rnd<= speedUp_P + maxSpeedUp_P + maxEnergyUp_P + rapidFireUp_P + energyRecoverySpeedUp_P+life_P)
        {
            thisItemName = ItemName.life;
            spriteRenderer.sprite=life_img;
        }
        else if(rnd<=speedUp_P + maxSpeedUp_P + maxEnergyUp_P + rapidFireUp_P + energyRecoverySpeedUp_P + life_P+invincible_P)
        {
            thisItemName = ItemName.invincible;
            spriteRenderer.sprite = invincible_img;
        }
        else if(rnd<= speedUp_P + maxSpeedUp_P + maxEnergyUp_P + rapidFireUp_P + energyRecoverySpeedUp_P + life_P+invincible_P+infinity_P)
        {
            thisItemName = ItemName.infinity;
            spriteRenderer.sprite=infinity_img;
        }
        
        //時間経過で消滅
        Destroy(this.gameObject, 30f);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public enum ItemName
    {
        speedUp,//速度UP 15%
        maxSpeedUp,//最大速度UP（主にドッジで役立つ）15%
        maxEnergyUp,//最大エネルギー上昇 15%
        rapidFireUp,//連射力UP 15%
        energyRecoverySpeedUp,//エネルギー回復スピードUP 10%
        life,//ライフ回復 10%
        invincible,//無敵（一定時間ダメージ無効）10%
        infinity,//無限（一定時間エネルギー使い放題）10%
    }
    [SerializeField]ItemName thisItemName;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameObject player = collision.gameObject;

            //アイテム数が上限に達していない
            //もしくはアイテムリストに存在しない（上限がない）ならアイテムを取得する
            PlayerItem playerItem=player.GetComponent<PlayerItem>();
            var ItemList = playerItem.ItemList;
            if (!ItemList.ContainsKey(thisItemName) || ItemList[thisItemName]<10)
            {
                //アイテムリストに追加
                if(ItemList.ContainsKey(thisItemName))
                {
                    playerItem.AddItem(thisItemName);
                }

                PlayerStatus playerStatus = player.GetComponent<PlayerStatus>();
                PlayerController playerController = player.GetComponent<PlayerController>();
                //効果音・エフェクト
                AudioSource audioSource = player.GetComponent<AudioSource>();
                audioSource.PlayOneShot(itemGetSE);
                //アイテムの効果
                switch (thisItemName)
                {
                    case ItemName.speedUp: playerController.force += 2.5f; break;
                    case ItemName.maxSpeedUp: playerController.maxSpeed += 2; break;
                    case ItemName.maxEnergyUp: playerStatus.MaxEnergy += 20; break;
                    case ItemName.energyRecoverySpeedUp: playerStatus.energyNaturalRecovery += 0.04f; break;
                    case ItemName.rapidFireUp:
                        if (playerController.intervalTime > 0.02)
                            playerController.intervalTime -= 0.025f; break;
                    case ItemName.life: playerStatus.HPUpdate(20); break;
                    case ItemName.invincible: playerStatus.Invincible();break;
                    case ItemName.infinity: playerStatus.Infinity();break;
                }
                //消滅する
                Destroy(gameObject);
            }
        }
    }
}