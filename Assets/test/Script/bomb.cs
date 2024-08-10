using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bomb : MonoBehaviour
{
    [SerializeField] float waitTime;//爆発までの待機時間(2f)
    [SerializeField] float lostTime;//爆発持続時間(0.5f)
    GameObject Range;
    

    bool isExplosion = false;//爆発したか

    [SerializeField] ParticleSystem ExplosionEffect;
    // Start is called before the first frame update
    void Start()
    {
        Range = gameObject.transform.Find("Range").gameObject;//子オブジェクトから取得
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

    //爆発後消滅
    IEnumerator Lost()
    {
        var ExplosionEffect_ = Instantiate(ExplosionEffect, this.transform.position, Quaternion.identity, this.transform);
        yield return new WaitForSeconds(lostTime);
        ExplosionEffect_.gameObject.transform.position = this.transform.position;
        Destroy(this.gameObject);
    }
}
