using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class bulletStatus : MonoBehaviour
{
    [SerializeField]public int Damage;//弾が与えるダメージ
    [SerializeField]public int Owner;//弾を出した人判定

    [SerializeField]bool isTriggerDestroy;//衝突したら消えるか
    private string colorCode;

    

    private void Start()
    {
        colorCode = PlayerStatus.P_color[Owner];
        if (ColorUtility.TryParseHtmlString(colorCode, out Color color))
        {
            GetComponent<SpriteRenderer>().color = color;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isTriggerDestroy)
        {
            //当たったオブジェクトが弾を発射した機体なら消滅しない
            if (collision.CompareTag("Player"))
            {
                var playerStatus = collision.GetComponent<PlayerStatus>();
                if (playerStatus.P_Num != Owner)
                {
                    //ダメージを与える
                    playerStatus.DamageProcess(Damage, Owner);
                    Destroy(this.gameObject);
                }
            }
            else if(!collision.CompareTag("Item"))
            {
                Destroy(this.gameObject);
            }
        }
    }
}
