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
    [SerializeField]float HP;
    [SerializeField]public float Energy;

    [SerializeField]public int MaxHP=100;
    [SerializeField]public int MaxEnergy=100;
    public bool isDead=false;

    public PlayerInput playerInput;
    public int P_Num;//プレイヤー識別番号

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
    [SerializeField] Image HPbar_img;
    [SerializeField]Slider Energybar;
    private SpriteRenderer spriteRenderer;
    //エネルギー自然回復量
    public float energyNaturalRecovery=0.12f;
    //カラーコード
    public string colorCode;
    //VS_GameManager
    GameObject GM;
    public VS_GameManager VS_GM;
    //PlayerController
    PlayerController playerController;

    //カラーコード
    public static string[] P_color =
    {
        "#FF00F3",
        "#00FFFD",
        "#EDFF00",
        "#5DFF00"
    };

    private void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        audioSource = GetComponent<AudioSource>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        playerController=gameObject.GetComponent<PlayerController>();

        //ステータス初期化
        ResetStatus();
        //プレイヤー識別番号、色の割り当て
        ResetPlayer();

        //プレイヤーが上限以上なら削除
        if (P_Num > 3)
            Destroy(gameObject.transform.parent.gameObject);

        print($"プレイヤー#{P_Num}が入室");
        audioSource.PlayOneShot(enterSE);


    }

    //ダメージ処理
    public void DamageProcess(float damage,int owner)
    {
        if(!isDead)
        {
            HPUpdate(-damage);
            //敵のリザルト調整
            if (owner != P_Num)//敵に受けた攻撃なら
            {
                VS_GM.giveDamage[owner] += damage;
                //HPが0以下なら自分を倒した敵のキル数を加算
                if(HP<=0)
                {
                    VS_GM.killNum[owner]++;
                    StartCoroutine(playerController.vibration(1f, 1f, 0.5f));
                }
                else
                {
                    StartCoroutine(playerController.vibration(0.8f, 0.8f, 0.2f));
                }
            }
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

/*    private void OnTriggerEnter2D(Collider2D collision)
    {
        //弾に当たった時の処理
        if (collision.gameObject.CompareTag("bullet") && !isDead)
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
                    StartCoroutine(playerController.vibration(1f, 1f, 0.5f));
                }
                else
                {
                    StartCoroutine(playerController.vibration(0.8f, 0.8f, 0.2f));
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
    }*/
    public void EnergyUpdate(float recoveryAmount)
    {
        Energy += recoveryAmount;
        if(Energy > MaxEnergy)
        {
            Energy = MaxEnergy;
        }
        Energybar.DOValue((float)Energy / (float)MaxEnergy,0.5f);
    }
    public void HPUpdate(float recoveryAmount)
    {
        HP += recoveryAmount;
        if(HP>MaxHP)
        {
            HP = MaxHP;
        }
        //HPの割合
        float HPrate = (float)HP / (float)MaxHP;
        //HPバーの色変化
        if(HPrate<0.25)
        {
            HPbar_img.color = Color.red;
        }
        else if(HPrate<0.5)
        {
            HPbar_img.color = Color.yellow;
        }
        else
        {
            HPbar_img.color = Color.green;
        }
        HPbar.DOValue(HPrate, 0.5f);
    }

    void Update()
    {   
        if (Energy < MaxEnergy)
        {
            EnergyUpdate(energyNaturalRecovery);
        }

        //脱落処理
        if (HP <= 0 && !isDead)
        {
            //isDeadをtrue、Rankに生き残っている数を代入→aliveNumを減らす
            isDead = true;
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
                    //色を戻す
                    ResetPlayer();
                    //速度を0にする
                    this.gameObject.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
                                     
                });

            });
        }
    }
    public void FindGM()
    {
        GM = GameObject.Find("GameManager");
        VS_GM = GM.GetComponent<VS_GameManager>();
    }
    //プレイヤー識別番号、色の割り当て
    public void ResetPlayer()
    {
        P_Num = playerInput.user.index;

        //プレイヤーごとに見た目を変える
        colorCode = P_color[P_Num];
        if (ColorUtility.TryParseHtmlString(colorCode, out Color color))
        {
            spriteRenderer.color = color;
        }
    }
    //ステータス初期化
    public void ResetStatus()
    {
        isDead = false;
        MaxHP = 100;
        MaxEnergy = 100;
        energyNaturalRecovery = 0.12f;
        HP = MaxHP;
        Energy = MaxEnergy;
        HPbar.value = (float)HP / (float)MaxHP;
        Energybar.value = (float)Energy / (float)MaxEnergy;
        HPbar_img.color = Color.green;

        //速度を0にする
        this.gameObject.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
    }
}