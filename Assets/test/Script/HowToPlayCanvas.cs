using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;
using DG.Tweening;
using UnityEngine.UI;

public class HowToPlayCanvas : MonoBehaviour
{
    GameObject HowToPlayCanvas_;
    GameObject Panel;

    //SE
    AudioSource audioSource;
    [SerializeField] AudioClip enterSE;
    [SerializeField] AudioClip openWindowSE;
    [SerializeField] AudioClip cancelSE;

    //Sprite
    [SerializeField] Sprite rule1_Sprite;
    [SerializeField] Sprite rule2_Sprite;
    [SerializeField] Sprite operate_Sprite;
    [SerializeField] Sprite item_Sprite;

    [SerializeField] Sprite Button_Sprite;
    [SerializeField] Sprite ButtonPush_Sprite;

    Image Image;

    [SerializeField]bool isNowSprite;

    enum pageState
    {
        rule1,
        rule2,
        operate,
        item
    }

    [SerializeField] pageState state;

    // Start is called before the first frame update
    void Start()
    {
        HowToPlayCanvas_ = GameObject.Find("HowToPlayCanvas");
        Panel = HowToPlayCanvas_.transform.Find("Panel").gameObject;
        audioSource=HowToPlayCanvas_.GetComponent<AudioSource>();
        Image = Panel.transform.Find("Image").GetComponent<Image>();
    }

    void SwitchPageState(pageState state)
    {
        audioSource.PlayOneShot(enterSE);
        switch (state)
        {
            case pageState.rule1: Image.sprite = rule1_Sprite; break;
            case pageState.rule2: Image.sprite = rule2_Sprite; break;
            case pageState.operate: Image.sprite = operate_Sprite; break;
            case pageState.item: Image.sprite = item_Sprite; break;
        }
    }

    public void pushButton()
    {
        if (!isNowSprite)
        {
            SwitchPageState(state);
            foreach (Transform button in Panel.transform.Find("buttons"))
            {
                button.GetComponent<HowToPlayCanvas>().SpriteReset();
            }
            isNowSprite = true;
            GetComponent<Image>().sprite = ButtonPush_Sprite;
        }
    }


    void SpriteReset()
    {
        isNowSprite=false;
        GetComponent<Image>().sprite = Button_Sprite;
    }

    public void HowToPlayButton()
    {
        
        if (!Panel.activeSelf)
        {
            Panel.SetActive(true);
            audioSource.PlayOneShot(openWindowSE);
            Panel.transform.localScale = Vector3.zero;
            Panel.transform.DOScale(new Vector3(0.98f, 0.98f, 0.98f), 0.3f);
        }
    }
    public void Xbutton()
    {
        audioSource.PlayOneShot(cancelSE);
        Panel.SetActive(false);

    }
}
