using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{
    bool[] ready=new bool[4];
    int readyNum = 0;
    [SerializeField] GameObject[] playerBord=new GameObject[4];
    //SE
    AudioSource audioSource;
    [SerializeField] AudioClip ReadySE;

    [SerializeField]GameObject playerInputManager;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        //Playerの初期化
        GameObject[] p = GameObject.FindGameObjectsWithTag("Player");
        for(int i=0; i<p.Length; i++)
        {
            //位置を戻す
            p[i].transform.position = Vector3.zero;
            //操作可能にする
            p[i].GetComponent<PlayerController>().AllControllArrow(true,false);
            //readyの状態をfalseにする
            p[i].GetComponent<PlayerController>().ready = false;
            //HPとEnergy全回復
            p[i].GetComponent<PlayerStatus>().ResetStatus();

        }
    }

    // Update is called once per frame
    void Update()
    {
        GameObject[] p = GameObject.FindGameObjectsWithTag("Player");
        Debug.Log("プレイヤー人数"+p.Length);

        //playerInputManagerのアクティブ・非アクティブ制御
        //プレイヤーが四人以上なら非アクティブにする
        if(p.Length==4&&playerInputManager.activeSelf)
        {
            playerInputManager.SetActive(false);
        }
        else if(p.Length<4&&!playerInputManager.activeSelf)
        {
            playerInputManager.SetActive(true);
        }

        for(int i=0; i<p.Length; i++)
        {
            //playerBordが非アクティブならアクティブにする
            if (!playerBord[i].activeSelf)
            {
                playerBord[i].SetActive(true);
            }
            ready[i]=p[i].GetComponent<PlayerController>().ready;

            //readyがtrueかつplayerBordのReadyがfalseなら（最初の一回だけ行う準備完了の処理）
            if (ready[i]&&!playerBord[i].transform.Find("Ready").gameObject.activeSelf)
            {
                print("プレイヤー" + i + "準備完了！");
                audioSource.PlayOneShot(ReadySE);
                readyNum++;
                playerBord[i].transform.Find("stand-by").gameObject.SetActive(false);
                playerBord[i].transform.Find("pull the triger").gameObject.SetActive(false);
                playerBord[i].transform.Find("Ready").gameObject.SetActive(true);
            }
        }

        //プレイヤー全員が準備完了ならステージに移動させる
        if(readyNum==p.Length&&readyNum>1)
        {
            for(int i=0; i<p.Length;i++)
            {
                DontDestroyOnLoad(p[i].transform.parent);
                p[i].GetComponent<PlayerController>().isControllOk = false;
                //体力、速度を初期化
                p[i].GetComponent<Rigidbody2D>().velocity = Vector3.zero;
                var playerStatus = p[i].GetComponent<PlayerStatus>();
                playerStatus.Energy = playerStatus.MaxEnergy;
            }
            StartCoroutine(LoadScene());
        }
    }
    //時間差でシーンを読み込む
    IEnumerator LoadScene()
    {
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene("vs_stage");
    }
}
