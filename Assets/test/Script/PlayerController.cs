using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    //入力を受け取るPlayerInput
    private PlayerInput playerInput;
    private PlayerStatus playerStatus;
    //移動速度
    [SerializeField] float speed;
    //移動入力方向
    private Vector2 movementValue_;
    private Vector2 LookValue_;
    private float Lookangle;
    //弾
    [SerializeField] GameObject bulletPrefab;
    //弾速
    [SerializeField]float bulletSpeed;
    //弾発射ポイント
    private Vector3 bulletPoint;
    //弾が消えるまで
    private float bulletLostTime=2;
    //発砲許可
    public bool isFireOk=false;
    //操作許可
    public bool isControllOk = true;
    //Dodgeエネルギー消費量
    public int dodgeEnergy=12;
    //ファイアエネルギー消費量
    public int fireEnergy=10;
    //準備完了
    public bool ready = false;

    //SE&BGM
    AudioSource audioSource;
    [SerializeField] AudioClip shootSE;
    [SerializeField] AudioClip dodgeSE;
    [SerializeField] AudioClip enterSE;
    [SerializeField] AudioClip cancelSE;
    //effect
    [SerializeField] ParticleSystem dodgeEffect;

    //ベクトルから角度を求める
    public static float Vector2ToAngle(Vector2 vector)
    {
        return Mathf.Atan2(vector.y, vector.x) * Mathf.Rad2Deg;
    }
    //角度から単位ベクトル
    public static Vector2 AngleToVector2(float angle)
    {
        var radian = angle * (Mathf.PI / 180);
        return new Vector2(Mathf.Cos(radian), Mathf.Sin(radian)).normalized;
    }
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();
        audioSource = GetComponent<AudioSource>();
        playerStatus = GetComponent<PlayerStatus>();
    }
    private void OnMove(InputValue movementValue)
    {
        // Moveアクションの入力値を取得
        /*移動操作*/
        if (isControllOk)
        {
            movementValue_ = movementValue.Get<Vector2>();//ブースト用
            rb.AddForce(movementValue_ * 7);
        }
        
    }

    private void OnLook(InputValue LookValue)
    {
        //Lookアクションの入力値を取得
        //Vectorを取得→角度に変換
        LookValue_ = LookValue.Get<Vector2>();
        Lookangle = Vector2ToAngle(LookValue_) - 90;
        if(Lookangle!=-90)
        {
            rb.rotation = Lookangle;
        }
        
    }

    private void OnDodge()
    {
        if (isControllOk)
        {
            if (movementValue_ != new Vector2(0, 0) && playerStatus.Energy >= dodgeEnergy)
            {
                rb.AddForce(movementValue_ * 1000);
                audioSource.PlayOneShot(dodgeSE);
                Instantiate(dodgeEffect, this.transform.position, Quaternion.identity, this.transform);
                playerStatus.EnergyUpdate(-dodgeEnergy);
            }
        }
    }

    private void OnFire()
    {
        if (isFireOk)
        {
            if (playerStatus.Energy >= fireEnergy)
            {
                bulletPoint = transform.Find("bulletPoint").transform.position;
                GameObject bullet = Instantiate(bulletPrefab, bulletPoint, Quaternion.Euler(0, 0, rb.rotation));
                audioSource.PlayOneShot(shootSE);
                playerStatus.EnergyUpdate(-fireEnergy);

                bulletStatus bulletStatus_;
                bulletStatus_ = bullet.GetComponent<bulletStatus>();
                bulletStatus_.Owner = playerStatus.P_Num;

                Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
                // 弾速は自由に設定
                bulletRb.AddForce(AngleToVector2(rb.rotation + 90) * bulletSpeed);
                // 時間差で砲弾を破壊する
                Destroy(bullet, bulletLostTime);
            }
        }
        //ロビーにいるとき
        if (SceneManager.GetActiveScene().name == "LobbyScene")
        {
            //準備完了
            if (!ready)
            {
                ready = true;
            }
        }
        
    }
    private void OnEnter()
    {
        print("右ボタン押されたよ");
        var VS_GM = playerStatus.VS_GM;
        //result画面の時
        if(VS_GM!=null)
        {
            //リザルト確認処理
            if (VS_GM.isResultWait&&!VS_GM.isResultOk[playerStatus.P_Num])
            {
                print("resultOk!");
                VS_GM.isResultOk[playerStatus.P_Num] = true;
                audioSource.PlayOneShot(enterSE);
            }
        }    

    }

    // Update is called once per frame
    void Update()
    {
        //一定速度以上なら減速
        if (rb.velocity.magnitude > speed)
        {
            rb.velocity *=0.95f;
        }
    }

    //全コントロール許可制御
    public void AllControllArrow(bool controllOk,bool fireOk)
    {
        isControllOk = controllOk;
        isFireOk = fireOk;
    }
}