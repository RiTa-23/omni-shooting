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
    [SerializeField]public int MaxHP;
    [SerializeField]public int MaxEnergy;

    public PlayerInput playerInput;
    public int P_Num;//初期のplayerInput.user.indexの値

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
    private new Renderer renderer;
    private SpriteRenderer spriteRenderer;
    //エネルギー自然回復量
    public float energyNaturalRecovery=0.12f;
    //カラーコード
    string colorCode;

    bool isDead = false;
    

    private void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        audioSource = GetComponent<AudioSource>();
        spriteRenderer = GetComponent<SpriteRenderer>();

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
        if (collision.gameObject.CompareTag("bullet")&&!isDead)
        {
            bulletStatus bulletStatus_;
            bulletStatus_ = collision.GetComponent<bulletStatus>();

            int killeMeEnemy = bulletStatus_.Owner;

            if (killeMeEnemy!= P_Num)
            {
                HP -= bulletStatus_.Damage;

                //敵のリザルト調整
                GameObject GM = GameObject.Find("GameManager");
                var VS_GM = GM.GetComponent<VS_GameManager>();

                VS_GM.giveDamage[killeMeEnemy] += bulletStatus_.Damage;
                //HPが0以下なら自分を倒した敵の敵を倒した数を加算し、自分のランクを決定
                if (HP <= 0)
                {
                    isDead = true;
                    VS_GM.killNum[killeMeEnemy]++;
                    VS_GM.Rank[P_Num] = VS_GM.p.Length;
                }

                HPbar.value =(float)HP/(float)MaxHP;
                Destroy(collision);
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
    public void EnergyUpdate()
    {
        Energybar.value = (float)Energy / (float)MaxEnergy;
    }

    //初めて条件に当てはまったか
    bool isFirst = false;
    void Update()
    {
        if (Energy < MaxEnergy)
        {
            Energy += energyNaturalRecovery;
            EnergyUpdate();
        }

        if (HP <= 0&&!isFirst)
        {
            isFirst = true;
            print($"プレイヤー#{playerInput.user.index}が撃墜！");
            AudioSource.PlayClipAtPoint(deadSE, new Vector3(0, 0, -10));
            Instantiate(deadEffect, this.transform.position, Quaternion.identity, this.transform);
            deadEffect.Play();
            gameObject.GetComponent<SpriteRenderer>().DOColor(Color.red, 0.5f).OnComplete(() =>
            {
                gameObject.GetComponent<SpriteRenderer>().DOColor(Color.white, 0.1f).OnComplete(() =>
                {
                    Destroy(gameObject.transform.parent.gameObject);
                });

            });
        }
    }
}
