using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bomb : MonoBehaviour
{
    [SerializeField] float waitTime;//�����܂ł̑ҋ@����(2f)
    [SerializeField] float lostTime;//������������(0.5f)
    GameObject Range;
    

    bool isExplosion = false;//����������

    [SerializeField] ParticleSystem ExplosionEffect;
    // Start is called before the first frame update
    void Start()
    {
        Range = gameObject.transform.Find("Range").gameObject;//�q�I�u�W�F�N�g����擾
        Range.SetActive(false);
        StartCoroutine(waitExplosion());
    }

    private void Explosion()
    {
        if(!isExplosion)
        {
            isExplosion = true;
            Range.SetActive(true);
            StartCoroutine(Lost());
        }
    }

    IEnumerator waitExplosion()
    {
        yield return new WaitForSeconds(waitTime);
        Explosion();
    }

    //���������
    IEnumerator Lost()
    {
        var ExplosionEffect_ = Instantiate(ExplosionEffect, this.transform.position, Quaternion.identity, this.transform);
        yield return new WaitForSeconds(lostTime);
        ExplosionEffect_.gameObject.transform.position = this.transform.position;
        Destroy(this.gameObject);
    }
}
