using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class HowToPlayCanvas : MonoBehaviour
{
    [SerializeField]PlayerJoinController playerJoinController;

    GameObject HowToPlayCanvas_;
    GameObject Panel;

    //SE
    [SerializeField]AudioSource audioSource;
    [SerializeField] AudioClip enterSE;
    
    [SerializeField] AudioClip cancelSE;

    //Sprite
    [SerializeField] Sprite rule1_Sprite;
    [SerializeField] Sprite rule2_Sprite;
    [SerializeField] Sprite operate_Sprite;
    [SerializeField] Sprite item_Sprite;

    Image Image;

    public enum pageState
    {
        none,
        rule1,
        rule2,
        operate,
        item
    }
    pageState nowState;

    GameObject buttons;

    GameObject rule1;
    GameObject rule2;
    GameObject operate;
    GameObject item;

    // Start is called before the first frame update
    void Start()
    {
        HowToPlayCanvas_ = GameObject.Find("HowToPlayCanvas");
        Panel = HowToPlayCanvas_.transform.Find("Panel").gameObject;
        Image = Panel.transform.Find("Image").GetComponent<Image>();

        buttons = Panel.transform.Find("buttons").gameObject;
        rule1 = buttons.transform.Find("Rule(1)").gameObject;
        rule2 = buttons.transform.Find("Rule(2)").gameObject;
        operate = buttons.transform.Find("Operate").gameObject;
        item = buttons.transform.Find("Item").gameObject;

        nowState = pageState.rule1;

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

    public void pushButton(pageState state)
    {
        SwitchPageState(state);
        foreach (Transform button in Panel.transform.Find("buttons"))
        {
            button.GetComponent<ruleButtons>().SpriteReset();
        }
        nowState = state;
    }
    bool isButtonInterval = false;
    float buttonIntervalTime = 0.3f;
    float buttonNowTime = 0;

    private void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        if (horizontal != 0&&!isButtonInterval)
        {
            print("horizontal");
            print(nowState);
            print(horizontal);
            if (horizontal > 0.5)
            {
                switch (nowState)
                {
                    case pageState.rule1: rule2.GetComponent<ruleButtons>().pushButton(); break;
                    case pageState.rule2: operate.GetComponent<ruleButtons>().pushButton(); break;
                    case pageState.operate: item.GetComponent<ruleButtons>().pushButton(); break;
                    case pageState.item: rule1.GetComponent<ruleButtons>().pushButton(); break;
                }
                isButtonInterval = true;
                print("interval");
            }
            else if(horizontal < -0.5)
            {
                switch (nowState)
                {
                    case pageState.rule1: item.GetComponent<ruleButtons>().pushButton(); break;
                    case pageState.rule2: rule1.GetComponent<ruleButtons>().pushButton(); break;
                    case pageState.operate: rule2.GetComponent<ruleButtons>().pushButton(); break;
                    case pageState.item: operate.GetComponent<ruleButtons>().pushButton(); break;
                }
                isButtonInterval=true;
            }
        }

        if(isButtonInterval)
        {
            buttonNowTime += Time.deltaTime;
            if(buttonNowTime>buttonIntervalTime)
            {
                buttonNowTime = 0;
                isButtonInterval = false;
            }
        }
    }

    public void Xbutton()
    {
        if (Panel.activeSelf)
        {
            audioSource.PlayOneShot(cancelSE);
            Panel.SetActive(false);
            ChangeActionMap("Player");
            //時間のスピードを戻す
            Time.timeScale = 1.0f;
            playerJoinController.EnablePlayerJoining();
        }
    }

    private void ChangeActionMap(string mapName)
    {
        //全プレイヤーのActionMapを一斉変更する
        var p = GameObject.FindGameObjectsWithTag("Player");

        for (int i = 0; i < p.Length; i++)
        {
            p[i].GetComponent<PlayerInput>().SwitchCurrentActionMap(mapName);
        }
    }
}
