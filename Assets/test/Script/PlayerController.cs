using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Haptics;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    //���͂��󂯎��PlayerInput
    private PlayerInput playerInput;
    private PlayerStatus playerStatus;
    //�ړ����ɉ������
    public float force=7;
    //�ړ����x���
    public float maxSpeed = 10;
    //�ړ����͕���
    private Vector2 movementValue_;
    private Vector2 LookValue_;
    private float Lookangle;
    //�e
    [SerializeField] GameObject bulletPrefab;
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

    //SE&BGM
    AudioSource audioSource;
    [SerializeField] AudioClip shootSE;
    [SerializeField] AudioClip dodgeSE;
    [SerializeField] AudioClip enterSE;
    [SerializeField] AudioClip cancelSE;
    //effect
    [SerializeField] ParticleSystem dodgeEffect;
    public LobbyManager lobbyManager;

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
        rb = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();
        audioSource = GetComponent<AudioSource>();
        playerStatus = GetComponent<PlayerStatus>();
        lobbyManager=GameObject.Find("LobbyManager").gameObject.GetComponent<LobbyManager>();
    }
    private void OnMove(InputValue movementValue)
    {
        // Move�A�N�V�����̓��͒l���擾
        /*�ړ�����*/
        if (isControllOk)
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
                StartCoroutine(vibration(0.6f, 0.6f, 0.2f));
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
    private void OnEnter()
    {
        print("�E�{�^�������ꂽ��");
        var VS_GM = playerStatus.VS_GM;
        //result��ʂ̎�
        if(VS_GM!=null)
        {
            //���U���g�m�F����
            if (VS_GM.isResultWait&&!VS_GM.isResultOk[playerStatus.P_Num])
            {
                print("resultOk!");
                VS_GM.isResultOk[playerStatus.P_Num] = true;
                audioSource.PlayOneShot(enterSE);
            }
        }    

    }
    private void OnCancel()
    {
        //���r�[�ɂ���Ƃ�
        if (SceneManager.GetActiveScene().name == "LobbyScene")
        {
            //AllReady����Ȃ��Ȃ�
            if(!lobbyManager.isAllReady)
            {
                //���r�[�ގ�
                lobbyManager.ExitLobby(playerStatus.P_Num);
            }
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