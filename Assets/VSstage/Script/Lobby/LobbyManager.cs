using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LobbyManager : MonoBehaviour
{
    //ready状態か
    public bool[] ready=new bool[4];
    int readyNum = 0;
    public bool isAllReady = false;
    //playerBord
    [SerializeField] GameObject[] playerBord=new GameObject[4];
    //SE
    AudioSource audioSource;
    [SerializeField] AudioClip ReadySE;
    [SerializeField] AudioClip ExitSE;
    //player
    GameObject[] p;

    [SerializeField] GameObject playerInputManager;
    //Stage
    [SerializeField] TextMeshProUGUI stageSelectText;
    [SerializeField] string[] stageNames = new string[3] { "Small", "Medium", "Large" };
    static int selectStageNum = 0;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        stageSelectText.text = stageNames[selectStageNum];
        //Playerの初期化
        p = GameObject.FindGameObjectsWithTag("Player");
        for(int i=0; i<p.Length; i++)
        {
            //位置を戻す
            p[i].transform.position = Vector3.zero;
            //操作可能にする
            p[i].GetComponent<PlayerController>().AllControllArrow(true,false);
            //HPとEnergy全回復
            p[i].GetComponent<PlayerStatus>().ResetStatus();
            //LobbyManager代入
            p[i].GetComponent<PlayerController>().lobbyManager = this;
            //PlayerControllerステータスも初期化
            p[i].GetComponent<PlayerController>().ResetControllerStatus();
            //ItemListの初期化
            p[i].GetComponent<PlayerItem>().ResetItemList();

        }
    }
    [SerializeField] GameObject defaultGameObject;

    // Update is called once per frame
    void Update()
    {
        ////確認用
        //if (Input.anyKeyDown)
        //{
        //    foreach (KeyCode code in System.Enum.GetValues(typeof(KeyCode)))
        //    {
        //        if (Input.GetKeyDown(code))
        //        {
        //            Debug.Log(code);
        //            break;
        //        }
        //    }
        //}

        //UI操作
        UIController();

        p = GameObject.FindGameObjectsWithTag("Player");
        Debug.Log("プレイヤー人数"+p.Length);

        //人数以上のplayerBordがあれば非アクティブにする
        int pBactive = 0;
        for(int i=0;i<4;i++)
        {
            if (playerBord[i].activeSelf)
                pBactive++;
        }
        if(pBactive>p.Length)
        {
            for(int i=pBactive;i>p.Length;i--)
            {
                playerBord[i-1].SetActive(false);
            }
        }

        for (int i=0; i<p.Length; i++)
        {
            //playerBordが非アクティブならアクティブにする
            if (!playerBord[i].activeSelf)
            {
                playerBord[i].SetActive(true);
            }
            //最初の一回だけ行う準備完了の処理
            if (ready[i]&&!playerBord[i].transform.Find("Ready").gameObject.activeSelf)
            {
                print("プレイヤー" + i + "準備完了！");
                audioSource.PlayOneShot(ReadySE);
                readyNum++;
                ReadyOrStandby(playerBord[i],true);
            }
            //準備完了をオフにする（誰かが退室したときに実行される）
            if (!ready[i]&& playerBord[i].transform.Find("Ready").gameObject.activeSelf)
            {
                ReadyOrStandby(playerBord[i], false);
            }
        }

        //プレイヤー全員が準備完了ならステージに移動させる
        if(readyNum==p.Length&&readyNum>1)
        {
            isAllReady = true;
            for(int i=0; i<p.Length;i++)
            {
                DontDestroyOnLoad(p[i].transform.parent);
                p[i].GetComponent<PlayerController>().isControllOk = false;
            }
            StartCoroutine(LoadScene());
        }
    }
    //時間差でシーンを読み込む
    IEnumerator LoadScene()
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(stageNames[selectStageNum]);
    }

    //Readyまたはstandby状態に変化したときの処理（trueならReady）
    void ReadyOrStandby(GameObject playerBord,bool isReady)
    {
        playerBord.transform.Find("pull the triger").gameObject.SetActive(!isReady);
        playerBord.transform.Find("stand-by").gameObject.SetActive(!isReady);
        playerBord.transform.Find("Ready").gameObject.SetActive(isReady);
    }

    //退室
    public void ExitLobby(int playerNum)
    {
        audioSource.PlayOneShot(ExitSE);
        //退室するプレイヤーを消去
        Destroy(p[playerNum].transform.parent.gameObject);

        //プレイヤーのリセット
        p = GameObject.FindGameObjectsWithTag("Player");
        for(int i=0; i<p.Length;i++)
        {
            if (i!=playerNum)
            p[i].GetComponent<PlayerStatus>().ResetPlayer();
        }

        //readyのリセット
        for(int i=0;i<4;i++)
        {
            ready[i] = false;
        }
        readyNum = 0;

    }

    //ロビーでのUI操作
    [SerializeField] GameObject buttons;
    [SerializeField] EventSystem eventSystem;
    GameObject HowToPanel;
    GameObject[] button;
    bool isMenuOpen;
    void OpenMenu()
    {
        //遊び方オープン
        GameObject menuButton = GameObject.FindWithTag("Menu");
        if (menuButton != null && HowToPanel == null)
        {
            menuButton.GetComponent<Button>().onClick.Invoke();
        }
    }
    void CloseMenu()
    {
        //遊び方クローズ
        GameObject XButton = GameObject.FindWithTag("returnPlayer");
        if (XButton != null && HowToPanel != null)
        {
            XButton.GetComponent<Button>().onClick.Invoke();
        }
    }
    string previousKey="";
    void UIController()
    {

        //gamepadの押されているキーの取得
        string keyName = "";
        foreach (var gamepad in Gamepad.all)
        {
            foreach (var control in gamepad.allControls)
            {
                if (control is ButtonControl button && button.wasPressedThisFrame)
                {
                    keyName = control.name;
                    Debug.Log($"押されたボタン: {keyName}");
                    break; // 最初に押されたボタンを検出したらループを抜ける
                }
            }
        }
        //D-pad（十字キー）の取得
        //D-padとスティックどちらも同じ名前で出力されてしまうため
        bool isDup = false;
        bool isDdown = false;
        bool isDright = false;  
        bool isDleft = false;
        foreach(var gamepad in Gamepad.all)
        {
            if (gamepad.dpad.up.wasPressedThisFrame)
            {
                Debug.Log("D-pad Up pressed");
                isDup = true;
            }
            else if (gamepad.dpad.down.wasPressedThisFrame)
            {
                Debug.Log("D-pad Down pressed");
                isDdown = true;
            }
            else if (gamepad.dpad.right.wasPressedThisFrame)
            {
                Debug.Log("D-pad Right pressed");
                isDright = true;
            }
            else if (gamepad.dpad.left.wasPressedThisFrame)
            {
                Debug.Log("D-pad Left pressed");
                isDleft = true;
            }

        }


        //現在選択されているオブジェクト確認
        print(eventSystem.currentSelectedGameObject);

        HowToPanel = GameObject.Find("HowToPanel");
        //スペースキーで遊び方
        if(Input.GetKeyDown(KeyCode.Space)&&!isAllReady)
        {
            OpenMenu();
            CloseMenu();
        }
        //コントローラー十字キーでのメニューopen/close
        if(isDup&&!isAllReady)
        {
            OpenMenu();
        }
        if (isDdown)
        {
            CloseMenu();
        }
        if(HowToPanel==null)
        {
            if(isDright)
            {
                selectStageNum++;
                if (selectStageNum >= stageNames.Length)
                {
                    selectStageNum = 0;
                }
                stageSelectText.text = stageNames[selectStageNum];
            }
            if(isDleft)
            {
                selectStageNum--;
                if (selectStageNum < 0)
                {
                    selectStageNum = stageNames.Length - 1;
                }
                stageSelectText.text = stageNames[selectStageNum];
            }
        }
        //ページ移動
        if (HowToPanel != null && buttons != null)
        {
            int selectNum = 0;
            GameObject[] button = new GameObject[buttons.transform.childCount];
            for (int i = 0; i < buttons.transform.childCount; i++)
            {
                button[i] = buttons.transform.GetChild(i).gameObject;
                if (button[i] == eventSystem.currentSelectedGameObject)
                {
                    selectNum = i;
                    print("selectNum：" + i);
                }
                print(button[i] + "：" + i);
            }
            //右に移動
            if (keyName == "right")
            {
                print("右矢印");
                if (selectNum != buttons.transform.childCount - 1)
                {
                    button[selectNum + 1].GetComponent<Button>().onClick.Invoke();
                }
                else
                {
                    print("右端から左端へ");
                    button[0].GetComponent<Button>().onClick.Invoke();
                }
            }
            //左に移動
            if (keyName == "left")
            {
                print("左矢印");
                if (selectNum != 0)
                {
                    button[selectNum - 1].GetComponent<Button>().onClick.Invoke();
                }
                else
                {
                    //一番左端
                    print("左端から右端へ");
                    button[buttons.transform.childCount - 1].GetComponent<Button>().onClick.Invoke();
                }
            }
        }
        previousKey = keyName;
    }
}
