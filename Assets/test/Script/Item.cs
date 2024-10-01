using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] AudioClip itemGetSE;
    // Start is called before the first frame update

    void Start()
    {
        SpriteRenderer spriteRenderer= GetComponent<SpriteRenderer>();
        string colorCode = "";

        //���̃A�C�e���ɂȂ邩�����_���Ɍ��߂�
        int rnd = UnityEngine.Random.Range(1, 101);//1~100�͈̔͂Ń����_���Ȑ����l
        if (rnd <= 15)
        {
            thisItemName = ItemName.speedUp;
            colorCode = "#31BCFF";//���F
        }
        else if(rnd<=30)
        {
            thisItemName = ItemName.maxSpeedUp;
            colorCode = "#3152FF";//��
        }
        else if(rnd<=45)
        {
            thisItemName = ItemName.maxEnergyUp;
            colorCode = "#FF7532";//�I�����W
        }
        else if(rnd<=60)
        {
            thisItemName = ItemName.rapidFireUp;
            colorCode = "#FF313C";//��
        }
        else if(rnd<=70)
        {
            thisItemName = ItemName.energyRecoverySpeedUp;
            colorCode = "#E9FF31";//���F
        }
        else if(rnd<=80)
        {
            thisItemName = ItemName.life;
            colorCode = "#FF81EC";//�s���N
        }
        else
        {
            thisItemName = ItemName.speedUp;
            colorCode = "#31BCFF";//���F
        }
        //�A�C�e�����ƂɌ����ڂ�ς���
        if (ColorUtility.TryParseHtmlString(colorCode, out Color color))
        {
            spriteRenderer.color = color;
        }
        //���Ԍo�߂ŏ���
        Destroy(this.gameObject, 30f);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    enum ItemName
    {
        speedUp,//���xUP 15%
        maxSpeedUp,//�ő呬�xUP�i��Ƀh�b�W�Ŗ𗧂j15%
        maxEnergyUp,//�ő�G�l���M�[�㏸ 15%
        rapidFireUp,//�A�˗�UP 15%
        energyRecoverySpeedUp,//�G�l���M�[�񕜃X�s�[�hUP 10%
        life,//���C�t�� 10%
        invincible,//���G�i��莞�ԃ_���[�W�����j10%
        infinity,//�����i��莞�ԃG�l���M�[�g������j10%
    }
    [SerializeField]ItemName thisItemName;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            PlayerStatus playerStatus=collision.GetComponent<PlayerStatus>();
            PlayerController playerController=collision.GetComponent<PlayerController>();
            //���ʉ��E�G�t�F�N�g
            AudioSource audioSource=collision.GetComponent<AudioSource>();
            audioSource.PlayOneShot(itemGetSE);
            //�A�C�e���̌���
            switch (thisItemName)
            {
                    case ItemName.speedUp: playerController.force += 5;break;
                    case ItemName.maxSpeedUp: playerController.maxSpeed += 5;break;
                    case ItemName.maxEnergyUp:playerStatus.MaxEnergy += 30;break;
                    case ItemName.energyRecoverySpeedUp:playerStatus.energyNaturalRecovery += 0.05f;break;
                    case ItemName.rapidFireUp:
                    if(playerController.intervalTime>0.02)
                    playerController.intervalTime -= 0.02f;break;
                    case ItemName.life:playerStatus.HPUpdate(20);break;
            }
            //���ł���
            Destroy(gameObject);
        }
    }
}