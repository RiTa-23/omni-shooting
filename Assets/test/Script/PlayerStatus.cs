using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerStatus : MonoBehaviour
{
    // Start is called before the first frame update
    //ステータス
    [SerializeField]public int HP;
    [SerializeField]public float Energy;

    [SerializeField]public int MaxHP=100;
    [SerializeField]public int MaxEnergy=100;

    public PlayerInput playerInput;
    public int P_Num;//プレイヤー番号（初期のplayerInput.user.indexの値）

    //SE&BGM
    AudioSource audioSource;
    [SerializeField] AudioClip deadSE;
    [SerializeField] AudioClip hitSE;
    [SerializeField] AudioClip enterSE;
    //Effect
    [SerializeField] ParticleSystem deadEffect;
    [SerializeField] ParticleSystem hitEffect;
    //ステータスバー
    [SerializeField]Slider HPbar;
    [SerializeField]Slider Energybar;
    private SpriteRenderer spriteRenderer;
    //エネルギー自然回復量
    public float energyNaturalRecovery=0.12f;
    //カラーコード
    string colorCode;
    //VS_GameManager
    GameObject GM;
    public VS_GameManager VS_GM;
    //PlayerController
    PlayerController playerController;




    private void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        audioSource = GetComponent<AudioSource>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        playerController=gameObject.GetComponent<PlayerController>();

        P_Num = playerInput.user.index;
        print($"プレイヤー#{P_Num}が入室");
        audioSource.PlayOneShot(enterSE);

        //カラーコード
        var oneP_color = "#FF00F3";
        //var oneP_color = "#AD00FF";
        var twoP_color = "#00FFFD";
        /*var threeP_color = "#B4FF00";*/
        var threeP_color="#EDFF00";
        var fourP_color = "#5DFF00";

        //プレイヤーごとに見た目を変える
        switch (playerInput.user.index)
        {
            case 0: 
                colorCode = oneP_color;break;
            case 1:
                colorCode= twoP_color;break;
            case 2: 
                colorCode= threeP_color;break;
            case 3: 
                colorCode= fourP_color;break;
        }
        if (ColorUtility.TryParseHtmlString(colorCode, out Color color))
        {
            spriteRenderer.color = color;
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("bullet") && !VS_GM.isDead[P_Num])
        {
            bulletStatus bulletStatus_;
            bulletStatus_ = collision.GetComponent<bulletStatus>();

            int killeMeEnemy = bulletStatus_.Owner;

            if (killeMeEnemy!= P_Num)
            {
                HPUpdate(-bulletStatus_.Damage);

                //敵のリザルト調整
                VS_GM.giveDamage[killeMeEnemy] += bulletStatus_.Damage;
                //HPが0以下なら自分を倒した敵のキル数を加算
                if (HP <= 0)
                {
                    VS_GM.killNum[killeMeEnemy]++;
                }
                //弾を消す
                Destroy(collision);

                //hitEffect
                Instantiate(hitEffect, this.transform.position, Quaternion.identity, this.transform);
                audioSource.PlayOneShot(hitSE);
                gameObject.GetComponent<SpriteRenderer>().DOColor(Color.red, 0.15f).OnComplete(() =>
                {
                    if (ColorUtility.TryParseHtmlString(colorCode, out Color color))
                    {
                        spriteRenderer.color = color;
                    }
                });  
            }
        }
    }
    public void EnergyUpdate(float recoveryAmount)
    {
        Energy += recoveryAmount;
        if(Energy > MaxEnergy)
        {
            Energy = MaxEnergy;
        }
        Energybar.value = (float)Energy / (float)MaxEnergy;
    }
    public void HPUpdate(int recoveryAmount)
    {
        HP += recoveryAmount;
        if(HP>MaxHP)
        {
            HP = MaxHP;
        }
        HPbar.value = (float)HP / (float)MaxHP;
    }

    void Update()
    {   
        if (Energy < MaxEnergy)
        {
            EnergyUpdate(energyNaturalRecovery);
        }

        //脱落処理
        if (HP <= 0 && !VS_GM.isDead[P_Num])
        {
            //isDeadをtrue、Rankに生き残っている数を代入→aliveNumを減らす
            VS_GM.isDead[P_Num] = true;
            VS_GM.Rank[P_Num] = VS_GM.aliveNum;
            VS_GM.aliveNum--;
            //操作不可にする
            playerController.AllControllArrow(false, false);

            print($"プレイヤー#{playerInput.user.index}が撃墜！");
            AudioSource.PlayClipAtPoint(deadSE, new Vector3(0, 0, -10));
            var deadEffect_ =Instantiate(deadEffect, this.transform.position, Quaternion.identity, this.transform);
            gameObject.GetComponent<SpriteRenderer>().DOColor(Color.red, 0.5f).OnComplete(() =>
            {
                gameObject.GetComponent<SpriteRenderer>().DOColor(Color.white, 0.2f).OnComplete(() =>
                {
                    //死亡エフェクトを切り離す
                    deadEffect_.gameObject.transform.parent = null;
                    //ステージから退避させる
                    this.transform.position = GameObject.Find("tempPos").gameObject.transform.position;
                    
                });

            });
        }
    }
    public void FindGM()
    {
        GM = GameObject.Find("GameManager");
        VS_GM = GM.GetComponent<VS_GameManager>();
    }
    public void ResetStatus()
    {
        HP = MaxHP;
        Energy = MaxEnergy;
        HPbar.value = (float)HP / (float)MaxHP;
        Energybar.value = (float)Energy / (float)MaxEnergy;
        if (ColorUtility.TryParseHtmlString(colorCode, out Color color))
        {
            spriteRenderer.color = color;
        }
    }


}
