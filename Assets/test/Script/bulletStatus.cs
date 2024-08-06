using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class bulletStatus : MonoBehaviour
{
    [SerializeField]public int Damage;//�e���^����_���[�W
    [SerializeField]public int Owner;//�e���o�����l����
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //���������I�u�W�F�N�g���e�𔭎˂����@�̂Ȃ���ł��Ȃ�
        if(collision.CompareTag("Player"))
        {
            var playerStatus=collision.GetComponent<PlayerStatus>();
            if(playerStatus.P_Num !=Owner)
            {
                Destroy(this.gameObject);
            }
        }
        else
        {
            Destroy(this.gameObject);
        }
        
    }
}
