using System;
using System.Collections;
using System.Collections.Generic;
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

    // Start is called before the first frame update

    void Start()
    {
        SpriteRenderer spriteRenderer= GetComponent<SpriteRenderer>();

        //何のアイテムになるかランダムに決める
        int rnd = UnityEngine.Random.Range(1, 101);//1~100の範囲でランダムな整数値

        //乱数からアイテム決定
        //アイテムごとに見た目を変える
        if (rnd <= 15)
        {
            thisItemName = ItemName.speedUp;
            spriteRenderer.sprite = speedUp_img;
        }
        else if(rnd<=30)
        {
            thisItemName = ItemName.maxSpeedUp;
            spriteRenderer.sprite = maxSpeedUp_img;
        }
        else if(rnd<=45)
        {
            thisItemName = ItemName.maxEnergyUp;
           spriteRenderer.sprite= maxEnergyUp_img;
        }
        else if(rnd<=60)
        {
            thisItemName = ItemName.rapidFireUp;
            spriteRenderer.sprite = rapidFireUp_img;
        }
        else if(rnd<=70)
        {
            thisItemName = ItemName.energyRecoverySpeedUp;
            spriteRenderer.sprite= energyRecoverySpeedUp_img;
        }
        else if(rnd<=80)
        {
            thisItemName = ItemName.life;
            spriteRenderer.sprite=life_img;
        }
        else
        {
            thisItemName = ItemName.speedUp;
            spriteRenderer.sprite = speedUp_img;
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameObject player = collision.gameObject;
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
            }
            //消滅する
            Destroy(gameObject);
        }
    }
}