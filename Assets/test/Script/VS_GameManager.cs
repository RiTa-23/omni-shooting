using DG.Tweening;
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
    [SerializeField] GameObject spawnPos;
    private GameObject spawn1;//左上
    private GameObject spawn2;//右下
    private GameObject tempPos;
    [SerializeField] GameObject spawnEffect;
    //プレイヤーオブジェクト
    public GameObject[] p;
    //Canvas
    [SerializeField] GameObject frontCanvas;

    //SE&BGM
    AudioSource audioSource;
    [SerializeField] AudioClip spawnSE;
    [SerializeField] AudioClip readySE;
    [SerializeField] AudioClip goSE;
    [SerializeField] AudioSource BGM;

    //プレイヤー人数
    int playerNum;

    //リザルト用
    public int[] killNum;//倒した敵の数
    public int[] giveDamage;//敵に与えたダメージ量
    public int[] Rank;//何位か

    // Start is called before the first frame update
    void Start()
    {
        BGM.Stop();
        spawn1 = spawnPos.transform.Find("1").gameObject;
        spawn2 = spawnPos.transform.Find("2").gameObject;
        tempPos = spawnPos.transform.Find("tempPos").gameObject;
        audioSource=GetComponent<AudioSource>();

        p = GameObject.FindGameObjectsWithTag("Player");
        Debug.Log("プレイヤー人数" + p.Length);
        playerNum = p.Length;

        killNum=new int[playerNum];
        giveDamage = new int[playerNum];
        Rank = new int[playerNum];

        //一時的にプレイヤーをステージ外に退避
        for(int i = 0; i < p.Length; i++)
        {
            p[i].transform.position = tempPos.transform.position;
        }
        //ランダムな位置にプレイヤーをスポーン
        StartCoroutine(spawnPlayer());
        //ゲーム開始
        StartCoroutine(ReadyGo());

    }
    IEnumerator spawnPlayer()
    {
        for (int i = 0; i < p.Length; i++)
        {
            //ランダムスポーン
            float x = Random.Range(spawn1.transform.position.x, spawn2.transform.position.x);
            float y = Random.Range(spawn2.transform.position.y, spawn1.transform.rotation.y);
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
        yield return new WaitForSeconds(0.25f*(p.Length+1));

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
        for (int i = 0; i < p.Length; i++)
        {
            p[i].GetComponent<PlayerController>().isControllOk = true;
            p[i].GetComponent<PlayerController>().isFireOk = true;
        }

        yield return new WaitForSeconds(0.5f);
        go.SetActive(false);
        BGM.Play();
    }

    bool isResult = false;
    // Update is called once per frame
    void Update()
    {
        p = GameObject.FindGameObjectsWithTag("Player");

        //プレイヤー数が１以下になったらResultに移行
        if (p.Length <= 1&&!isResult)
        {
            isResult = true;
            StartCoroutine(Result());
        }
    }
    IEnumerator Result()
    {
        GameObject result = frontCanvas.transform.Find("Result").gameObject;
        yield return new WaitForSeconds(0.5f);
        if (p.Length == 1)
        {
            //〇P Win!的なのを出す + Result
            GameObject winner = result.transform.Find("winner").gameObject;
            int winnerNum = p[0].GetComponent<PlayerStatus>().P_Num;
            //winnerのランクを1にする
            Rank[winnerNum] = 1;

            winner.GetComponent<Text>().text =(winnerNum+1)+"P Wins!";
            winner.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
            winner.SetActive(true);

            yield return new WaitForSeconds(3);

            //生き残ってるプレイヤーオブジェクトを削除
            if (p.Length!=0)
            Destroy(p[0].transform.parent.gameObject);

            //上に移動
            winner.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0, 150), 2f)
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
            Draw.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0, 150), 2f)
    .SetEase(Ease.OutBack);
            yield return new WaitForSeconds(2);
        }

        //resultを出す
        GameObject resultPanel = result.transform.Find("resultPanel").gameObject;
        resultPanel.SetActive(true);
        resultPanel.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0, 57), 1f)
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
            string Ranktext = "";
            switch (Rank[i])
            {
                case 1: Ranktext = "1st"; break;
                case 2: Ranktext = "2nd"; break;
                case 3: Ranktext = "3rd"; break;
                case 4: Ranktext = "4th"; break;
            }
            pP[i].transform.Find("Rank").GetComponent<Text>().text = (i + 1) + "P - " + Ranktext;
            pP[i].transform.Find("killNum").GetComponent<Text>().text = "撃破 : " + killNum[i];
            pP[i].transform.Find("giveDamage").GetComponent<Text>().text = "与ダメ : " + giveDamage[i];
            pP[i].SetActive(true);
        }
        //プレイヤーパネル移動
        playerPanels.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0, -105), 1f)
.SetEase(Ease.OutBack);
        yield return new WaitForSeconds(7);

        //ボタンが押されたらロビーに戻る

        SceneManager.LoadScene("LobbyScene");
    }
}
