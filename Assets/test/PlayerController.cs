using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    //���͂��󂯎��PlayerInput
    private PlayerInput playerInput;
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

    //SE&BGM
    AudioSource audioSource;
    [SerializeField] AudioClip shootSE;
    [SerializeField] AudioClip dodgeSE;

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

    }
    private void OnMove(InputValue movementValue)
    {
        // Move�A�N�V�����̓��͒l���擾
        /*�ړ�����*/
        movementValue_ = movementValue.Get<Vector2>();//�u�[�X�g�p
        rb.AddForce(movementValue_*20);
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
        // �e���͎��R�ɐݒ�
        bulletRb.AddForce(AngleToVector2(rb.rotation+90) * bulletSpeed);
        // ���ԍ��ŖC�e��j�󂷂�
        Destroy(bullet, bulletLostTime);
    }

    // Update is called once per frame
    void Update()
    {
        //��葬�x�ȏ�Ȃ猸��
        if(rb.velocity.magnitude > speed)
        {
            rb.velocity *=0.95f;
        }
    }
}