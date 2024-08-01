using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{
    bool[] ready=new bool[4];
    int readyNum = 0;
    [SerializeField] GameObject[] playerBord=new GameObject[4];
    //SE
    AudioSource audioSource;
    [SerializeField] AudioClip ReadySE;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        GameObject[] p = GameObject.FindGameObjectsWithTag("Player");
        Debug.Log("�v���C���[�l��"+p.Length);
        for(int i=0; i<p.Length; i++)
        {
            //playerBord����A�N�e�B�u�Ȃ�A�N�e�B�u�ɂ���
            if (!playerBord[i].activeSelf)
            {
                playerBord[i].SetActive(true);
            }
            ready[i]=p[i].GetComponent<PlayerController>().ready;

            //ready��true����playerBord��Ready��false�Ȃ�i�ŏ��̈�񂾂��s�����������̏����j
            if (ready[i]&&!playerBord[i].transform.Find("Ready").gameObject.activeSelf)
            {
                print("�v���C���[" + i + "���������I");
                audioSource.PlayOneShot(ReadySE);
                readyNum++;
                playerBord[i].transform.Find("stand-by").gameObject.SetActive(false);
                playerBord[i].transform.Find("pull the triger").gameObject.SetActive(false);
                playerBord[i].transform.Find("Ready").gameObject.SetActive(true);
            }
        }

        //�v���C���[�S�������������Ȃ�X�e�[�W�Ɉړ�������
        if(readyNum==p.Length&&readyNum>1)
        {
            for(int i=0; i<p.Length;i++)
            {
                DontDestroyOnLoad(p[i].transform.parent);
                p[i].GetComponent<PlayerController>().isControllOk = false;
                //�̗́A���x��������
                p[i].GetComponent<Rigidbody2D>().velocity = Vector3.zero;
                var playerStatus = p[i].GetComponent<PlayerStatus>();
                playerStatus.Energy = playerStatus.MaxEnergy;
            }
            StartCoroutine(LoadScene());
        }
    }
    //���ԍ��ŃV�[����ǂݍ���
    IEnumerator LoadScene()
    {
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene("vs_stage");
    }
}
