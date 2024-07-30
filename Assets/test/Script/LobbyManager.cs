using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{
    bool[] ready=new bool[4];
    int readyNum = 0;
    [SerializeField] GameObject[] playerBord=new GameObject[4];
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        readyNum = 0;
        GameObject[] p = GameObject.FindGameObjectsWithTag("Player");
        Debug.Log("プレイヤー人数"+p.Length);
        for(int i=0; i<p.Length; i++)
        {
            //playerBordが非アクティブならアクティブにする
            if (!playerBord[i].activeSelf)
            {
                playerBord[i].SetActive(true);
            }
            ready[i]=p[i].GetComponent<PlayerController>().ready;

            if (ready[i])
            {
                print("プレイヤー" + i + "準備完了！");
                readyNum++;
                playerBord[i].transform.Find("stand-by").gameObject.SetActive(false);
                playerBord[i].transform.Find("pull the triger").gameObject.SetActive(false);
                playerBord[i].transform.Find("Ready").gameObject.SetActive(true);
            }
        }

        //プレイヤー全員が準備完了ならステージに移動させる
        if(readyNum==p.Length&&readyNum!=0)
        {
            for(int i=0; i<p.Length;i++)
            {
                DontDestroyOnLoad(p[i].transform.parent);
                p[i].GetComponent<PlayerController>().isControllOk = false;
            }
            SceneManager.LoadScene("vs_stage");
        }
    }
}
