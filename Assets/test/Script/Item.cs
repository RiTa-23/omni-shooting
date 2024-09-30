using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;

public class Item : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    enum ItemName
    {
        speedUp,//���xUP
        maxSpeedUp,//�ő呬�xUP�i��Ƀh�b�W�Ŗ𗧂j
        maxEnergyUp,//�ő�G�l���M�[�㏸
        energyRecoverySpeedUp,//�G�l���M�[�񕜃X�s�[�hUP
        rapidFireUp,//�A�˗�UP
        life,//���C�t��
        invincible,//���G�i��莞�ԃ_���[�W�����j
        infinity,//�����i��莞�ԃG�l���M�[�g������j
    }
    [SerializeField]ItemName thisItemName;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            PlayerStatus playerStatus=collision.GetComponent<PlayerStatus>();
            PlayerController playerController=collision.GetComponent<PlayerController>();
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

                    //���ʉ��ƃG�t�F�N�g�����悤��

                    
            }
            //���ł���
            Destroy(gameObject);
        }
    }
}
