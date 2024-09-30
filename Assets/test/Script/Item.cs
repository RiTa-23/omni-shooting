using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;

public class Item : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    enum ItemName
    {
        speedUp,//速度UP
        maxSpeedUp,//最大速度UP（主にドッジで役立つ）
        maxEnergyUp,//最大エネルギー上昇
        energyRecoverySpeedUp,//エネルギー回復スピードUP
        rapidFireUp,//連射力UP
        life,//ライフ回復
        invincible,//無敵（一定時間ダメージ無効）
        infinity,//無限（一定時間エネルギー使い放題）
    }
    [SerializeField]ItemName thisItemName;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            PlayerStatus playerStatus=collision.GetComponent<PlayerStatus>();
            PlayerController playerController=collision.GetComponent<PlayerController>();
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

                    //効果音とエフェクトがつけよう↑

                    
            }
            //消滅する
            Destroy(gameObject);
        }
    }
}
