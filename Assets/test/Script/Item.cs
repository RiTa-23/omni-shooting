using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Search;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] AudioClip itemGetSE;
    [SerializeField] Sprite speedUp_img;
    [SerializeField] Sprite maxSpeedUp_img;
    [SerializeField] Sprite maxEnergyUp_img;
    [SerializeField] Sprite rapidFireUp_img;
    [SerializeField] Sprite energyRecoverySpeedUp_img;
    [SerializeField] Sprite life_img;
    [SerializeField] Sprite invincible_img;
    [SerializeField] Sprite infinity_img;

    [SerializeField] int speedUp_P = 15;
    [SerializeField] int maxSpeedUp_P = 15;
    [SerializeField] int maxEnergyUp_P = 15;
    [SerializeField] int rapidFireUp_P = 15;
    [SerializeField] int energyRecoverySpeedUp_P = 15;
    [SerializeField] int life_P = 15;
    [SerializeField] int invincible_P = 5;
    [SerializeField] int infinity_P = 5;

    // Start is called before the first frame update

    void Start()
    {
        SpriteRenderer spriteRenderer= GetComponent<SpriteRenderer>();

        //���̃A�C�e���ɂȂ邩�����_���Ɍ��߂�
        int rnd = UnityEngine.Random.Range(1, 101);//1~100�͈̔͂Ń����_���Ȑ����l

        //��������A�C�e������
        //�A�C�e�����ƂɌ����ڂ�ς���
        if (rnd <= speedUp_P)
        {
            thisItemName = ItemName.speedUp;
            spriteRenderer.sprite = speedUp_img;
        }
        else if(rnd<=speedUp_P+maxSpeedUp_P)
        {
            thisItemName = ItemName.maxSpeedUp;
            spriteRenderer.sprite = maxSpeedUp_img;
        }
        else if(rnd<= speedUp_P + maxSpeedUp_P+ maxEnergyUp_P)
        {
            thisItemName = ItemName.maxEnergyUp;
           spriteRenderer.sprite= maxEnergyUp_img;
        }
        else if(rnd<= speedUp_P + maxSpeedUp_P + maxEnergyUp_P+rapidFireUp_P)
        {
            thisItemName = ItemName.rapidFireUp;
            spriteRenderer.sprite = rapidFireUp_img;
        }
        else if(rnd<= speedUp_P + maxSpeedUp_P + maxEnergyUp_P + rapidFireUp_P+energyRecoverySpeedUp_P)
        {
            thisItemName = ItemName.energyRecoverySpeedUp;
            spriteRenderer.sprite= energyRecoverySpeedUp_img;
        }
        else if(rnd<= speedUp_P + maxSpeedUp_P + maxEnergyUp_P + rapidFireUp_P + energyRecoverySpeedUp_P+life_P)
        {
            thisItemName = ItemName.life;
            spriteRenderer.sprite=life_img;
        }
        else if(rnd<=speedUp_P + maxSpeedUp_P + maxEnergyUp_P + rapidFireUp_P + energyRecoverySpeedUp_P + life_P+invincible_P)
        {
            thisItemName = ItemName.invincible;
            spriteRenderer.sprite = invincible_img;
        }
        else if(rnd<= speedUp_P + maxSpeedUp_P + maxEnergyUp_P + rapidFireUp_P + energyRecoverySpeedUp_P + life_P+invincible_P+infinity_P)
        {
            thisItemName = ItemName.infinity;
            spriteRenderer.sprite=infinity_img;
        }
        
        //���Ԍo�߂ŏ���
        Destroy(this.gameObject, 30f);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public enum ItemName
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameObject player = collision.gameObject;

            //�A�C�e����������ɒB���Ă��Ȃ�
            //�������̓A�C�e�����X�g�ɑ��݂��Ȃ��i������Ȃ��j�Ȃ�A�C�e�����擾����
            PlayerItem playerItem=player.GetComponent<PlayerItem>();
            var ItemList = playerItem.ItemList;
            if (!ItemList.ContainsKey(thisItemName) || ItemList[thisItemName]<10)
            {
                //�A�C�e�����X�g�ɒǉ�
                if(ItemList.ContainsKey(thisItemName))
                {
                    playerItem.AddItem(thisItemName);
                }

                PlayerStatus playerStatus = player.GetComponent<PlayerStatus>();
                PlayerController playerController = player.GetComponent<PlayerController>();
                //���ʉ��E�G�t�F�N�g
                AudioSource audioSource = player.GetComponent<AudioSource>();
                audioSource.PlayOneShot(itemGetSE);
                //�A�C�e���̌���
                switch (thisItemName)
                {
                    case ItemName.speedUp: playerController.force += 2.5f; break;
                    case ItemName.maxSpeedUp: playerController.maxSpeed += 2; break;
                    case ItemName.maxEnergyUp: playerStatus.MaxEnergy += 20; break;
                    case ItemName.energyRecoverySpeedUp: playerStatus.energyNaturalRecovery += 0.04f; break;
                    case ItemName.rapidFireUp:
                        if (playerController.intervalTime > 0.02)
                            playerController.intervalTime -= 0.025f; break;
                    case ItemName.life: playerStatus.HPUpdate(20); break;
                    case ItemName.invincible: playerStatus.Invincible();break;
                    case ItemName.infinity: playerStatus.Infinity();break;
                }
                //���ł���
                Destroy(gameObject);
            }
        }
    }
}