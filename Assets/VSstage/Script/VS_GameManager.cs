using DG.Tweening;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class VS_GameManager : MonoBehaviour
{
    //スポーン地点
    [SerializeField] Vector2 spawnsize;
    [SerializeField] GameObject tempPos;
    [SerializeField] GameObject spawnEffect;
    //アイテム
    [SerializeField] GameObject ItemPrefab;
    public bool isItemOn=true;//アイテムありか
    [SerializeField]float spawnItemTime = 10;
    float spawnDeltaTime = 0;
    bool isSpawnItem;
    //プレイヤーオブジェクト
    public GameObject[] p;
    //Canvas
    [SerializeField] GameObject frontCanvas;

    //SE&BGM
    AudioSource audioSource;
    [SerializeField] AudioClip spawnSE;
    [SerializeField] AudioClip readySE;
    [SerializeField] AudioClip goSE;
    [SerializeField] AudioClip winnerSE;
    [SerializeField] AudioClip resultSE;
    [SerializeField] AudioSource BGM;

    //プレイヤー人数
    int playerNum;
    public int aliveNum;//生き残りの数

    //リザルト用
    public int[] killNum;//倒した敵の数
    public float[] giveDamage;//敵に与えたダメージ量
    public int[] Rank;//何位か
    public bool isResultWait=false;//リザルト待機画面中か
    public bool[] isResultOk;//リザルト確認

    // Start is called before the first frame update
    void Start()
    {
        //初期化
        BGM.Stop();
        tempPos = GameObject.Find("tempPos").gameObject;
        audioSource=GetComponent<AudioSource>();

        p = GameObject.FindGameObjectsWithTag("Player");
        Debug.Log("プレイヤー人数" + p.Length);
        playerNum = p.Length;
        aliveNum = playerNum;

        killNum=new int[playerNum];
        giveDamage = new float[playerNum];
        Rank = new int[playerNum];
        isResultOk = new bool[playerNum];

        for(int i = 0; i < playerNum; i++)
        {
            isResultOk[i] = false;
            //ステータス、速度を初期化
            p[i].GetComponent<PlayerStatus>().ResetStatus();
            p[i].GetComponent<PlayerController>().ResetControllerStatus();
            //一時的にプレイヤーをステージ外に退避
            p[i].transform.position = tempPos.transform.position;
            //PlayerStatusの変数GMを格納
            p[i].GetComponent<PlayerStatus>().FindGM();
        }
        //ランダムな位置にプレイヤーをスポーン
        StartCoroutine(spawnPlayer());
        //ゲーム開始
        StartCoroutine(ReadyGo());

    }

    private void spawnItem()
    {
        //ランダムな位置にアイテムをスポーン
        //確率で排出率を決める
        float x=Random.Range(-spawnsize.x,spawnsize.x);
        float y = Random.Range(-spawnsize.y, spawnsize.y);
        Instantiate(ItemPrefab, new Vector3(x, y, 0), Quaternion.identity);
    }
    IEnumerator spawnPlayer()
    {
        Vector3[] temp_playerpos = new Vector3[playerNum];
        for (int i = 0; i < playerNum; i++)
        {
            float x=0, y=0;
            //ポジションチェック
            bool isPosBad = true;

            while (isPosBad)
            {
                //ランダムスポーン
                x = Random.Range(-spawnsize.x, spawnsize.x);
                y = Random.Range(-spawnsize.y, spawnsize.y);

                isPosBad = false;
                for (int j = 0; j < i; j++)
                {
                    //他のプレイヤーとの距離が近すぎる場合はもう一度
                    if ((temp_playerpos[j] - new Vector3(x, y, 0)).magnitude < 6)
                    {
                        print("再配置");
                        yield return new WaitForSeconds(0.0001f);
                        isPosBad = true;
                        break;
                    }
                }
            }

            temp_playerpos[i] = new Vector3(x, y, 0);
            Instantiate(spawnEffect, new Vector3(x, y, 0), Quaternion.identity);
            audioSource.PlayOneShot(spawnSE);
            yield return new WaitForSeconds(0.25f);
            p[i].transform.position = new Vector3(x, y, 0);
        }
    }
    IEnumerator ReadyGo()
    {
        //ゲーム開始エフェクト(UI,SE,BGM)
        GameObject ready = frontCanvas.transform.Find("Ready").gameObject;
        GameObject go = frontCanvas.transform.Find("Go").gameObject;

        //スポーン待機
        yield return new WaitForSeconds(0.25f*(playerNum+1));

        //ReadyGoエフェクト
        ready.SetActive(true);
        ready.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0, 0), 1.5f);
        audioSource.PlayOneShot(readySE);
        yield return new WaitForSeconds(readySE.length);
        ready.SetActive(false);
        go.SetActive(true);
        go.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0, 0), 0.3f);
        audioSource.PlayOneShot(goSE);
        
        //操作許可をtrue
        for (int i = 0; i < playerNum; i++)
        {
            p[i].GetComponent<PlayerController>().AllControllArrow(true,true);
        }

        yield return new WaitForSeconds(0.5f);
        go.SetActive(false);
        BGM.Play();
    }

    bool isResult = false;
    // Update is called once per frame
    void Update()
    {
        //プレイヤー数が１以下になったらResultに移行
        if (aliveNum <= 1 && !isResult)
        {
            isResult = true;
            StartCoroutine(Result());
        }

        //デバッグ用
        if (p.Length == 0)
            SceneManager.LoadScene("LobbyScene");

        //アイテムスポーン
        if (isItemOn)//アイテムありなら
        {
            if (!isSpawnItem)
            {
                isSpawnItem = true;
                spawnItem();
            }
            else//インターバル
            {
                spawnDeltaTime += Time.deltaTime;
                if (spawnDeltaTime > spawnItemTime)
                {
                    spawnDeltaTime = 0;
                    isSpawnItem = false;
                }
            }
        }
    }

    IEnumerator Result()
    {
        GameObject result = frontCanvas.transform.Find("Result").gameObject;
        BGM.Stop();
        isItemOn = false;
        string winnerColor="";
        yield return new WaitForSeconds(0.5f);
        if (aliveNum == 1)
        {
            //〇P Win!的なのを出す + Result
            GameObject winner = result.transform.Find("winner").gameObject;

            //生き残ったプレイヤーを特定
            int winnerNum=-1;
            for(int i=0;i<p.Length;i++)
            {
                if(!p[i].GetComponent<PlayerStatus>().isDead)
                {
                    winnerNum = i;
                }
            }
            //winnerのランクを1にする
            Rank[winnerNum] = 1;

            //wins!のエフェクト
            winnerColor = p[winnerNum].GetComponent<PlayerStatus>().colorCode;
            winner.GetComponent<Text>().text ="<color="+winnerColor+">"+(winnerNum+1)+"P</color> Wins!";
            audioSource.PlayOneShot(winnerSE);
            winner.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
            winner.SetActive(true);

            yield return new WaitForSeconds(3);

            //生き残ってるプレイヤーオブジェクトを操作不可、退避
            if (aliveNum !=0)
            {
                p[winnerNum].GetComponent<PlayerController>().AllControllArrow(false,false);
                p[winnerNum].transform.position=tempPos.transform.position;
                //速度を0にする
                p[winnerNum].GetComponent<Rigidbody2D>().velocity = Vector3.zero;
            }

            //上に移動
            winner.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0, 170), 2f)
    .SetEase(Ease.OutBack);
            yield return new WaitForSeconds(2);
        }
        else
        {
            //Draw　的なのを出す + Result
            GameObject Draw = result.transform.Find("Draw").gameObject;
            Draw.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
            Draw.SetActive(true);
            //上に移動
            Draw.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0, 170), 2f)
    .SetEase(Ease.OutBack);
            yield return new WaitForSeconds(2);
        }

        //resultを出す
        GameObject resultPanel = result.transform.Find("resultPanel").gameObject;
        resultPanel.SetActive(true);
        audioSource.PlayOneShot(resultSE);
        resultPanel.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0, 70), 1f)
.SetEase(Ease.OutBack);

        GameObject playerPanels = result.transform.Find("playerPanels").gameObject;
        GameObject[] pP = new GameObject[playerNum];
        //プレイヤーパネルをプレイ人数に応じて格納
        switch (playerNum)
        {
            case 4:
                pP[3] = playerPanels.transform.Find("4p_Panel").gameObject;
                goto case 3;
            case 3:
                pP[2] = playerPanels.transform.Find("3p_Panel").gameObject;
                goto case 2;
            case 2:
                pP[1] = playerPanels.transform.Find("2p_Panel").gameObject;
                pP[0] = playerPanels.transform.Find("1p_Panel").gameObject;
                break;
        }
        //プレイヤーパネル内のテキスト書き換え
        for (int i = 0; i < pP.Length; i++)
        {
            GameObject resultStatus = pP[i].transform.Find("resultStatus").gameObject;

            //Rankのマークをつける
            GameObject rank = pP[i].transform.Find("Rank").gameObject;
            if(Rank[i]<4)
            {
                rank.transform.Find(Rank[i].ToString()).gameObject.SetActive(true);
            }


            GameObject killNum_ = resultStatus.transform.Find("killNum").gameObject;
            GameObject giveDamage_ = resultStatus.transform.Find("giveDamage").gameObject;

            killNum_.GetComponent<Text>().text = "Kill : " + killNum[i];
            giveDamage_.GetComponent<Text>().text = "Attack : " + (int)giveDamage[i];

            if (UnityEngine.ColorUtility.TryParseHtmlString(p[i].GetComponent<PlayerStatus>().colorCode, out Color color))
            {
                killNum_.GetComponent<Text>().color = color;
                giveDamage_.GetComponent<Text>().color = color;
            }         
            pP[i].SetActive(true);
        }
        //プレイヤーパネル移動
        playerPanels.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0, -105), 1f)
.SetEase(Ease.OutBack);

        //全員の右ボタンが押されたらロビーに戻る
        isResultWait = true;

        bool isResultAllOk = false;
        while (!isResultAllOk)
        {
            int ResultOkNum = 0;
            for(int i = 0; i < playerNum; i++)
            {
                if (isResultOk[i])
                {
                    ResultOkNum++;
                    //リザルト確認OK!の処理
                    if (!pP[i].transform.Find("OK!").gameObject.activeSelf)
                    {
                        pP[i].transform.Find("pullRight").gameObject.SetActive(false);
                        pP[i].transform.Find("OK!").gameObject.SetActive(true);
                    }
                }
                
                if(ResultOkNum==playerNum)
                {
                    isResultAllOk=true;
                    break;
                }
            }
            yield return new WaitForSeconds(0.1f);
        }
        SceneManager.LoadScene("LobbyScene");
    }
}
