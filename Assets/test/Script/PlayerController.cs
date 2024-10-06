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
    //���͂��󂯎��PlayerInput
    private PlayerInput playerInput;
    private PlayerStatus playerStatus;
    //�ړ����ɉ������=�X�s�[�h
    public float force=7;
    //�ړ����x���
    public float maxSpeed = 10;
    //�ړ����͕���
    private Vector2 movementValue_;
    private Vector2 LookValue_;
    private float Lookangle;
    //�e
    [SerializeField] GameObject bulletPrefab;
    //���e
    [SerializeField] GameObject bombPrefab;
    //�e��
    [SerializeField]float bulletSpeed;
    //�e���˃|�C���g
    private Vector3 bulletPoint;
    //�e��������܂�
    private float bulletLostTime=2;
    //���C����
    public bool isFireOk=false;
    //���싖��
    public bool isControllOk = true;
    //Dodge�G�l���M�[�����
    public int dodgeEnergy=12;
    //�t�@�C�A�G�l���M�[�����
    public int fireEnergy=10;
    //�T�u�G�l���M�[�����
    public int subEnergy = 30;

    //�G�C���A�V�X�g����
    [SerializeField]float aimAsistAngle=0;

    //���͂��󂯎��PlayerInput
    private PlayerInput _playerInput;
    //�A�N�V������
    private string _fireActionName = "Fire";
    // �A�N�V����
    private InputAction _fireAction;

    //�C���^�[�o����
    bool isFireInterval;
    //�C���^�[�o���̎���
    public float intervalTime=0.3f;
    //�o�ߎ���
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

    //�x�N�g������p�x�����߂�
    public static float Vector2ToAngle(Vector2 vector)
    {
        return Mathf.Atan2(vector.y, vector.x) * Mathf.Rad2Deg;
    }
    //�p�x����P�ʃx�N�g��
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
        // Move�A�N�V�����̓��͒l���擾
        /*�ړ�����*/
        if (isControllOk&&rb!=null)
        {
            movementValue_ = movementValue.Get<Vector2>();//�u�[�X�g�p
            rb.AddForce(movementValue_ * force);
        }
        
    }

    private void OnLook(InputValue LookValue)
    {
        //Look�A�N�V�����̓��͒l���擾
        //Vector���擾���p�x�ɕϊ�
        LookValue_ = LookValue.Get<Vector2>();
        Lookangle = Vector2ToAngle(LookValue_) - 90;
        if (Lookangle != -90&&rb!=null)
        {
            rb.rotation = Lookangle;
            //�G�C���A�V�X�g
            AimAssist();
        }
    }
    //�G�C���A�V�X�g�@�\
    private void AimAssist()
    {
        GameObject[] p = GameObject.FindGameObjectsWithTag("Player");
        if (p.Length == 1) return;//�^�[�Q�b�g�ɂ���G�����Ȃ��Ȃ�I��

        //�ː��̒����̕������̌W�������߂�
        //�v���C���[��bulletPoint�̍��W�̓�_����o��
        Vector3 pPos=this.gameObject.transform.position;
        Vector2 bPos = new Vector2(pPos.x,pPos.y) + LookValue_;
        float x1=pPos.x; float x2 = bPos.x;
        float y1=pPos.y; float y2 = bPos.y;

        float a = (y2 - y1) / (x2 - x1);
        float b = -1;
        float c = -a * x1 + y1;

        //�G�v���C���[�̍��W���擾
        //�ː��̒����ƓG�v���C���[�̓_�̋������o��
        int target = -1;//�^�[�Q�b�g
        double targetDis=100;//�^�[�Q�b�g�̋���

        for (int i=0; i<p.Length; i++)
        {
            //���@�ł͂Ȃ�������ł��Ȃ��Ȃ�
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
            //�w��̊p�x���Ȃ�G�C���␳����(aimAsistAngle)
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
                // �e���͎��R�ɐݒ�
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
                // �e���͎��R�ɐݒ�
                bulletRb.AddForce(AngleToVector2(rb.rotation + 90) * bulletSpeed);
                // ���ԍ��ŖC�e��j�󂷂�
                Destroy(bullet, bulletLostTime);
            }
        }
        //���r�[�ɂ���Ƃ�
        if (SceneManager.GetActiveScene().name == "LobbyScene")
        {
            //��������
            if (lobbyManager != null && !lobbyManager.ready[playerStatus.P_Num])
            {
                lobbyManager.ready[playerStatus.P_Num] = true;
                StartCoroutine(vibration(0.7f, 0.7f, 0.2f));
            }
        }
        
    }
    private void OnOK()
    {
        print("�E�{�^�������ꂽ��");
        try
        {
            if (playerStatus.VS_GM != null)
            {
                var VS_GM = playerStatus.VS_GM;
                //result��ʂ̎�
                //���U���g�m�F����
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
            print("OnEnter�ŗ�O����");
        }

    }
    private void OnLeave()
    {
        //���r�[�ɂ���Ƃ�
        if (SceneManager.GetActiveScene().name == "LobbyScene")
        {
            //AllReady����Ȃ��Ȃ�
            if(!lobbyManager.isAllReady)
            {
                //���r�[�ގ��i�U���~�߂Ă����j
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
        //��葬�x�ȏ�Ȃ猸��
        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity *=0.95f;
        }

        /*�t�@�C�A*/
        // �U���{�^���̉�����Ԏ擾
        bool isFirePressed = _fireAction.IsPressed();
        if (isFirePressed&&!isFireInterval)
        {
            isFireInterval = true;
            fire();
        }
        //�C���^�[�o��
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

    //�S�R���g���[��������
    public void AllControllArrow(bool controllOk,bool fireOk)
    {
        isControllOk = controllOk;
        isFireOk = fireOk;
    }

    //�U���@�\
    int vibDuplication = 0;//�d����
    public IEnumerator vibration(float left, float right, float seconds)
    {
        // PlayerInput����U���\�ȃf�o�C�X�擾
        // playerInput.devices�͌��ݑI������Ă���X�L�[���̃f�o�C�X�ꗗ�ł��邱�Ƃɒ���
        if (playerInput.devices.FirstOrDefault(x => x is IDualMotorRumble) is not IDualMotorRumble gamepad)
        {
            Debug.Log("�f�o�C�X���ڑ�");
            yield break;
        }
        vibDuplication++;
        //�U���J�n
        gamepad.SetMotorSpeeds(left, right);

        yield return new WaitForSeconds(seconds);

        if (vibDuplication == 1)
        {
            //�d���Ȃ��Ȃ�U�����~�߂�
            gamepad.SetMotorSpeeds(0, 0);
        }
        vibDuplication--;
    }
}