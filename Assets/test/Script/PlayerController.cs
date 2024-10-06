using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Haptics;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    //入力を受け取るPlayerInput
    private PlayerInput playerInput;
    private PlayerStatus playerStatus;
    //移動時に加える力=スピード
    public float force=7;
    //移動速度上限
    public float maxSpeed = 10;
    //移動入力方向
    private Vector2 movementValue_;
    private Vector2 LookValue_;
    private float Lookangle;
    //弾
    [SerializeField] GameObject bulletPrefab;
    //爆弾
    [SerializeField] GameObject bombPrefab;
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
    //サブエネルギー消費量
    public int subEnergy = 30;

    //エイムアシスト調節
    [SerializeField]float aimAsistAngle=0;

    //入力を受け取るPlayerInput
    private PlayerInput _playerInput;
    //アクション名
    private string _fireActionName = "Fire";
    // アクション
    private InputAction _fireAction;

    //インターバルか
    bool isFireInterval;
    //インターバルの時間
    public float intervalTime=0.3f;
    //経過時間
    private float deltaTime=0;

    //SE&BGM
    AudioSource audioSource;
    [SerializeField] AudioClip shootSE;
    [SerializeField] AudioClip dodgeSE;
    [SerializeField] AudioClip EnterSE;
    [SerializeField] AudioClip bombSE;
    //effect
    [SerializeField] ParticleSystem dodgeEffect;
    public LobbyManager lobbyManager;

    public void ResetControllerStatus()
    {
        force = 7;
        maxSpeed = 10;
        intervalTime = 0.3f;
    }

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
        intervalTime = 0.25f;
        rb = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();
        audioSource = GetComponent<AudioSource>();
        playerStatus = GetComponent<PlayerStatus>();
        lobbyManager=GameObject.Find("LobbyManager").gameObject.GetComponent<LobbyManager>();
        _playerInput=GetComponent<PlayerInput>();
        _fireAction = _playerInput.actions[_fireActionName];

        //test
        _playerInput.uiInputModule=EventSystem.current.gameObject.GetComponent<InputSystemUIInputModule>();
    }
    private void OnMove(InputValue movementValue)
    {
        // Moveアクションの入力値を取得
        /*移動操作*/
        if (isControllOk&&rb!=null)
        {
            movementValue_ = movementValue.Get<Vector2>();//ブースト用
            rb.AddForce(movementValue_ * force);
        }
        
    }

    private void OnLook(InputValue LookValue)
    {
        //Lookアクションの入力値を取得
        //Vectorを取得→角度に変換
        LookValue_ = LookValue.Get<Vector2>();
        Lookangle = Vector2ToAngle(LookValue_) - 90;
        if (Lookangle != -90&&rb!=null)
        {
            rb.rotation = Lookangle;
            //エイムアシスト
            AimAssist();
        }
    }
    //エイムアシスト機能
    private void AimAssist()
    {
        GameObject[] p = GameObject.FindGameObjectsWithTag("Player");
        if (p.Length == 1) return;//ターゲットにする敵がいないなら終了

        //射線の直線の方程式の係数を求める
        //プレイヤーとbulletPointの座標の二点から出す
        Vector3 pPos=this.gameObject.transform.position;
        Vector2 bPos = new Vector2(pPos.x,pPos.y) + LookValue_;
        float x1=pPos.x; float x2 = bPos.x;
        float y1=pPos.y; float y2 = bPos.y;

        float a = (y2 - y1) / (x2 - x1);
        float b = -1;
        float c = -a * x1 + y1;

        //敵プレイヤーの座標を取得
        //射線の直線と敵プレイヤーの点の距離を出す
        int target = -1;//ターゲット
        double targetDis=100;//ターゲットの距離

        for (int i=0; i<p.Length; i++)
        {
            //自機ではないかつ死んでいないなら
            if (p[i] != this.gameObject && !playerStatus.isDead)
            {
                Vector3 ePos = p[i].transform.position;
                float x0 = ePos.x; float y0 = ePos.y;
                double tempDis = Math.Abs(a * x0 + b*y0+c)/Math.Sqrt(a*a+b*b);
                if (tempDis < targetDis)
                {
                    targetDis = tempDis;
                    target = i;
                }
            }
        }
        if (target!=-1)
        {
            LookValue_ = p[target].transform.position - pPos;
            //指定の角度内ならエイム補正する(aimAsistAngle)
            if (Math.Abs(Lookangle - Vector2ToAngle(LookValue_) + 90) < aimAsistAngle)
            {
                Lookangle = Vector2ToAngle(LookValue_) - 90;
                rb.rotation = Lookangle;
            }
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
                StartCoroutine(vibration(0.6f, 0.6f, 0.2f));
                Instantiate(dodgeEffect, this.transform.position, Quaternion.identity, this.transform);
                playerStatus.EnergyUpdate(-dodgeEnergy);
            }
        }
    }

    private void OnSub()
    {
        if(isFireOk)
        {
            if(playerStatus.Energy>=subEnergy)
            {
                bulletPoint = transform.Find("bulletPoint").transform.position;
                GameObject bomb = Instantiate(bombPrefab, bulletPoint, Quaternion.Euler(0, 0, rb.rotation));
                audioSource.PlayOneShot(bombSE);
                StartCoroutine(vibration(0.5f, 0.5f, 0.1f));
                playerStatus.EnergyUpdate(-subEnergy);

                bulletStatus bulletStatus_;
                bulletStatus_ = bomb.GetComponent<bulletStatus>();
                bulletStatus_.Owner = playerStatus.P_Num;

                Rigidbody2D bulletRb = bomb.GetComponent<Rigidbody2D>();
                // 弾速は自由に設定
                bulletRb.AddForce(AngleToVector2(rb.rotation + 90) * bulletSpeed);
            }
        }
    }

    private void fire()
    {
        if (isFireOk)
        {
            if (playerStatus.Energy >= fireEnergy)
            {
                bulletPoint = transform.Find("bulletPoint").transform.position;
                GameObject bullet = Instantiate(bulletPrefab, bulletPoint, Quaternion.Euler(0, 0, rb.rotation));
                audioSource.PlayOneShot(shootSE);
                StartCoroutine(vibration(0.5f, 0.5f, 0.1f));
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
            if (lobbyManager != null && !lobbyManager.ready[playerStatus.P_Num])
            {
                lobbyManager.ready[playerStatus.P_Num] = true;
                StartCoroutine(vibration(0.7f, 0.7f, 0.2f));
            }
        }
        
    }
    private void OnOK()
    {
        print("右ボタン押されたよ");
        try
        {
            if (playerStatus.VS_GM != null)
            {
                var VS_GM = playerStatus.VS_GM;
                //result画面の時
                //リザルト確認処理
                if (VS_GM.isResultWait && !VS_GM.isResultOk[playerStatus.P_Num])
                {
                    print("resultOk!");
                    VS_GM.isResultOk[playerStatus.P_Num] = true;
                    audioSource.PlayOneShot(EnterSE);
                }
            }
        }
        catch
        {
            print("OnEnterで例外発生");
        }

    }
    private void OnLeave()
    {
        //ロビーにいるとき
        if (SceneManager.GetActiveScene().name == "LobbyScene")
        {
            //AllReadyじゃないなら
            if(!lobbyManager.isAllReady)
            {
                //ロビー退室（振動止めておく）
                StartCoroutine(vibration(0, 0, 0));
                lobbyManager.ExitLobby(playerStatus.P_Num);
            }
        } 
    }

    private void OnOpenMenu()
    {
        GameObject menuButton = GameObject.FindWithTag("Menu");
        if (menuButton != null)
        {
            menuButton.GetComponent<Button>().onClick.Invoke();
            playerInput.SwitchCurrentActionMap("UI");
        }
    }
    private void OnCancel()
    {
        GameObject XButton = GameObject.FindWithTag("returnPlayer");
        if (XButton != null)
        {
            XButton.GetComponent<Button>().onClick.Invoke();
            playerInput.SwitchCurrentActionMap("Player");
        }
    }

    // Update is called once per frame
    void Update()
    {
        //一定速度以上なら減速
        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity *=0.95f;
        }

        /*ファイア*/
        // 攻撃ボタンの押下状態取得
        bool isFirePressed = _fireAction.IsPressed();
        if (isFirePressed&&!isFireInterval)
        {
            isFireInterval = true;
            fire();
        }
        //インターバル
        if(isFireInterval)
        {
            deltaTime += Time.deltaTime;
            if(deltaTime>intervalTime)
            {
                deltaTime = 0;
                isFireInterval=false;
            }
        }
    }

    //全コントロール許可制御
    public void AllControllArrow(bool controllOk,bool fireOk)
    {
        isControllOk = controllOk;
        isFireOk = fireOk;
    }

    //振動機能
    int vibDuplication = 0;//重複数
    public IEnumerator vibration(float left, float right, float seconds)
    {
        // PlayerInputから振動可能なデバイス取得
        // playerInput.devicesは現在選択されているスキームのデバイス一覧であることに注意
        if (playerInput.devices.FirstOrDefault(x => x is IDualMotorRumble) is not IDualMotorRumble gamepad)
        {
            Debug.Log("デバイス未接続");
            yield break;
        }
        vibDuplication++;
        //振動開始
        gamepad.SetMotorSpeeds(left, right);

        yield return new WaitForSeconds(seconds);

        if (vibDuplication == 1)
        {
            //重複なしなら振動を止める
            gamepad.SetMotorSpeeds(0, 0);
        }
        vibDuplication--;
    }
}