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
    [SerializeField]int HP;
    [SerializeField]public float Energy;
    [SerializeField] int MaxHP;
    [SerializeField]int MaxEnergy;
    PlayerInput playerInput;

    //SE&BGM
    AudioSource audioSource;
    [SerializeField] AudioClip deadSE;
    [SerializeField] AudioClip hitSE;
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
    

    private void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        audioSource = GetComponent<AudioSource>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        print($"プレイヤー#{playerInput.user.index}が入室");

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
        if (collision.gameObject.CompareTag("bullet"))
        {
            bulletStatus bulletStatus_;
            bulletStatus_ = collision.GetComponent<bulletStatus>();
            if(bulletStatus_.Owner!= playerInput.user.index)
            {
                HP -= bulletStatus_.Damage;
                HPbar.value =(float)HP/(float)MaxHP;
                Destroy(collision);
                hitEffect.Play();
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

    bool dead = false;
    void Update()
    {
        if (Energy < MaxEnergy)
        {
            Energy += energyNaturalRecovery;
            EnergyUpdate();
        }

        if (HP <= 0&&!dead)
        {
            dead = true;
            print($"プレイヤー#{playerInput.user.index}が撃墜！");
            AudioSource.PlayClipAtPoint(deadSE, new Vector3(0, 0, -10));
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
