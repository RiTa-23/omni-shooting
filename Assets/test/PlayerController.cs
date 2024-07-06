using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    //入力を受け取るPlayerInput
    private PlayerInput playerInput;
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

    //SE&BGM
    AudioSource audioSource;
    [SerializeField] AudioClip shootSE;
    [SerializeField] AudioClip dodgeSE;

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

    }
    private void OnMove(InputValue movementValue)
    {
        // Moveアクションの入力値を取得
        /*移動操作*/
        movementValue_ = movementValue.Get<Vector2>();//ブースト用
        rb.AddForce(movementValue_*20);
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
        if(movementValue_!=new Vector2(0,0))
        {
            rb.AddForce(movementValue_ * 1000);
            audioSource.PlayOneShot(dodgeSE);
        }
    }

    private void OnFire()
    {
        bulletPoint = transform.Find("bulletPoint").transform.position;
        GameObject bullet = Instantiate(bulletPrefab,bulletPoint, Quaternion.Euler(0, 0, rb.rotation));
        audioSource.PlayOneShot(shootSE);

        bulletStatus bulletStatus_;
        bulletStatus_ = bullet.GetComponent<bulletStatus>();
        bulletStatus_.Owner = playerInput.user.index;

        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        // 弾速は自由に設定
        bulletRb.AddForce(AngleToVector2(rb.rotation+90) * bulletSpeed);
        // 時間差で砲弾を破壊する
        Destroy(bullet, bulletLostTime);
    }

    // Update is called once per frame
    void Update()
    {
        //一定速度以上なら減速
        if(rb.velocity.magnitude > speed)
        {
            rb.velocity *=0.95f;
        }
    }
}