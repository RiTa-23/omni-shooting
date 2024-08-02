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
    //���͂��󂯎��PlayerInput
    private PlayerInput playerInput;
    private PlayerStatus playerStatus;
    //�ړ����x
    [SerializeField] float speed;
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
    //��������
    public bool ready = false;

    //SE&BGM
    AudioSource audioSource;
    [SerializeField] AudioClip shootSE;
    [SerializeField] AudioClip dodgeSE;
    [SerializeField] AudioClip enterSE;
    [SerializeField] AudioClip cancelSE;
    //effect
    [SerializeField] ParticleSystem dodgeEffect;

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
    }
    private void OnMove(InputValue movementValue)
    {
        // Move�A�N�V�����̓��͒l���擾
        /*�ړ�����*/
        if (isControllOk)
        {
            movementValue_ = movementValue.Get<Vector2>();//�u�[�X�g�p
            rb.AddForce(movementValue_ * 7);
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
            if (!ready)
            {
                ready = true;
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

    // Update is called once per frame
    void Update()
    {
        //��葬�x�ȏ�Ȃ猸��
        if (rb.velocity.magnitude > speed)
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
}