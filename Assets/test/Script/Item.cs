using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] AudioClip itemGetSE;
    // Start is called before the first frame update

    void Start()
    {
        SpriteRenderer spriteRenderer= GetComponent<SpriteRenderer>();
        string colorCode = "";

        //何のアイテムになるかランダムに決める
        int rnd = UnityEngine.Random.Range(1, 101);//1~100の範囲でランダムな整数値
        if (rnd <= 15)
        {
            thisItemName = ItemName.speedUp;
            colorCode = "#31BCFF";//水色
        }
        else if(rnd<=30)
        {
            thisItemName = ItemName.maxSpeedUp;
            colorCode = "#3152FF";//青
        }
        else if(rnd<=45)
        {
            thisItemName = ItemName.maxEnergyUp;
            colorCode = "#FF7532";//オレンジ
        }
        else if(rnd<=60)
        {
            thisItemName = ItemName.rapidFireUp;
            colorCode = "#FF313C";//赤
        }
        else if(rnd<=70)
        {
            thisItemName = ItemName.energyRecoverySpeedUp;
            colorCode = "#E9FF31";//黄色
        }
        else if(rnd<=80)
        {
            thisItemName = ItemName.life;
            colorCode = "#FF81EC";//ピンク
        }
        else
        {
            thisItemName = ItemName.speedUp;
            colorCode = "#31BCFF";//水色
        }
        //アイテムごとに見た目を変える
        if (ColorUtility.TryParseHtmlString(colorCode, out Color color))
        {
            spriteRenderer.color = color;
        }
        //時間経過で消滅
        Destroy(this.gameObject, 30f);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    enum ItemName
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            PlayerStatus playerStatus=collision.GetComponent<PlayerStatus>();
            PlayerController playerController=collision.GetComponent<PlayerController>();
            //効果音・エフェクト
            AudioSource audioSource=collision.GetComponent<AudioSource>();
            audioSource.PlayOneShot(itemGetSE);
            //アイテムの効果
            switch (thisItemName)
            {
                    case ItemName.speedUp: playerController.force += 5;break;
                    case ItemName.maxSpeedUp: playerController.maxSpeed += 5;break;
                    case ItemName.maxEnergyUp:playerStatus.MaxEnergy += 30;break;
                    case ItemName.energyRecoverySpeedUp:playerStatus.energyNaturalRecovery += 0.05f;break;
                    case ItemName.rapidFireUp:
                    if(playerController.intervalTime>0.02)
                    playerController.intervalTime -= 0.02f;break;
                    case ItemName.life:playerStatus.HPUpdate(20);break;
            }
            //消滅する
            Destroy(gameObject);
        }
    }
}