using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class bulletStatus : MonoBehaviour
{
    [SerializeField]public int Damage;//弾が与えるダメージ
    [SerializeField]public int Owner;//弾を出した人判定
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //当たったオブジェクトが弾を発射した機体なら消滅しない
        if(collision.CompareTag("Player"))
        {
            var playerStatus=collision.GetComponent<PlayerStatus>();
            if(playerStatus.P_Num !=Owner)
            {
                Destroy(this.gameObject);
            }
        }
        else
        {
            Destroy(this.gameObject);
        }
        
    }
}
