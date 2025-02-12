using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bombDamage : MonoBehaviour
{
    float rangeRadius;//�_���[�W�͈͔��a
    float MaxDamage=30;
    float adjustValue=0.7f;//�l�����p�i���e�ƃv���C���[�̋����v�Z�j
    // Start is called before the first frame update
    void Start()
    {
        rangeRadius = GetComponent<CircleCollider2D>().radius;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            //owner
            int owner = this.transform.parent.GetComponent<bulletStatus>().Owner;
            //�G�Ƃ̋���
            Vector2 dis = collision.transform.position-this.transform.position;
            //�^����_���[�W
            float damage= (1 - (dis.magnitude-adjustValue) / rangeRadius)*MaxDamage;
            Debug.Log(damage + "�_���[�W");
            
            collision.GetComponent<PlayerStatus>().DamageProcess(damage,owner);
            //�m�b�N�o�b�N
            collision.GetComponent<Rigidbody2D>().AddForce(dis*100);
        }
    }

}