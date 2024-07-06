using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStatus : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]int HP;
    [SerializeField]int Energy;
    [SerializeField]int MaxEnergy;
    PlayerInput playerInput;

    //SE&BGM
    AudioSource audioSource;
    [SerializeField] AudioClip deadSE;
    [SerializeField] AudioClip hitSE;

    private void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("bullet"))
        {
            bulletStatus bulletStatus_;
            bulletStatus_ = collision.GetComponent<bulletStatus>();
            if(bulletStatus_.Owner!= playerInput.user.index)
            {
                HP -= bulletStatus_.Damage;
                Destroy(collision);
                audioSource.PlayOneShot(hitSE);
            }
            
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (HP <= 0)
        {
            print($"プレイヤー#{playerInput.user.index}が撃墜！");
            AudioSource.PlayClipAtPoint(deadSE, new Vector3(0, 0, -10));
            Destroy(gameObject);
        }
    }
}
