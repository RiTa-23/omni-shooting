using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bombDamage : MonoBehaviour
{
    float rangeRadius;//ダメージ範囲半径
    float MaxDamage=30;
    float adjustValue=0.7f;//値調整用（爆弾とプレイヤーの距離計算）
    // Start is called before the first frame update
    void Start()
    {
        rangeRadius = GetComponent<CircleCollider2D>().radius;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            //owner
            int owner = this.transform.parent.GetComponent<bulletStatus>().Owner;
            //敵との距離
            Vector2 dis = collision.transform.position-this.transform.position;
            //与えるダメージ
            float damage= (1 - (dis.magnitude-adjustValue) / rangeRadius)*MaxDamage;
            Debug.Log(damage + "ダメージ");
            
            collision.GetComponent<PlayerStatus>().DamageProcess(damage,owner);
            //ノックバック
            collision.GetComponent<Rigidbody2D>().AddForce(dis*100);
        }
    }

}