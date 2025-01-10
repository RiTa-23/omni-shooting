using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] AudioClip itemGetSE;
    [SerializeField] Sprite speedUp_img;
    [SerializeField] Sprite maxSpeedUp_img;
    [SerializeField] Sprite maxEnergyUp_img;
    [SerializeField] Sprite rapidFireUp_img;
    [SerializeField] Sprite energySpeedUp_img;
    [SerializeField] Sprite lifeRecovery_img;
    [SerializeField] Sprite invincible_img;
    [SerializeField] Sprite infinity_img;

    [SerializeField] int speedUp_P = 15;
    [SerializeField] int maxSpeedUp_P = 15;
    [SerializeField] int maxEnergyUp_P = 15;
    [SerializeField] int rapidFireUp_P = 15;
    [SerializeField] int energySpeedUp_P = 15;
    [SerializeField] int lifeRecovery_P = 15;
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
        else if(rnd<= speedUp_P + maxSpeedUp_P + maxEnergyUp_P + rapidFireUp_P+energySpeedUp_P)
        {
            thisItemName = ItemName.energySpeedUp;
            spriteRenderer.sprite= energySpeedUp_img;
        }
        else if(rnd<= speedUp_P + maxSpeedUp_P + maxEnergyUp_P + rapidFireUp_P + energySpeedUp_P+lifeRecovery_P)
        {
            thisItemName = ItemName.lifeRecovery;
            spriteRenderer.sprite=lifeRecovery_img;
        }
        else if(rnd<=speedUp_P + maxSpeedUp_P + maxEnergyUp_P + rapidFireUp_P + energySpeedUp_P + lifeRecovery_P+invincible_P)
        {
            thisItemName = ItemName.invincible;
            spriteRenderer.sprite = invincible_img;
        }
        else if(rnd<= speedUp_P + maxSpeedUp_P + maxEnergyUp_P + rapidFireUp_P + energySpeedUp_P + lifeRecovery_P+invincible_P+infinity_P)
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
        speedUp,//���xUP
        maxSpeedUp,//�ő呬�xUP�i��Ƀh�b�W�Ŗ𗧂j
        maxEnergyUp,//�ő�G�l���M�[�㏸
        rapidFireUp,//�A�˗�UP
        energySpeedUp,//�G�l���M�[�񕜃X�s�[�hUP
        lifeRecovery,//���C�t��
        invincible,//���G�i��莞�ԃ_���[�W�����j
        infinity,//�����i��莞�ԃG�l���M�[�g������j
    }
    [SerializeField]ItemName thisItemName;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //���O�̒ǉ�
            TextMeshProUGUI LogText;
            LogText = GameObject.Find("LogText").GetComponent<TextMeshProUGUI>();

            GameObject player = collision.gameObject;
            int playerNum = player.GetComponent<PlayerStatus>().P_Num + 1;

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
                    //�A�C�e���Q�b�g�̃��O�ǉ�
                    LogText.text += "player" + playerNum + " �F " + thisItemName + " Lv." + ItemList[thisItemName] + "\n";
                }
                else
                {
                    //�A�C�e�����O�ǉ�
                    LogText.text += "player" + playerNum + " �F " + thisItemName +"\n";
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
                    case ItemName.energySpeedUp: playerStatus.energyNaturalRecovery += 0.04f; break;
                    case ItemName.rapidFireUp:
                        if (playerController.intervalTime > 0.02)
                            playerController.intervalTime -= 0.025f; break;
                    case ItemName.lifeRecovery: playerStatus.HPUpdate(20); break;
                    case ItemName.invincible: playerStatus.Invincible();break;
                    case ItemName.infinity: playerStatus.Infinity();break;
                }
                //���ł���
                Destroy(gameObject);
            }
        }
    }
}