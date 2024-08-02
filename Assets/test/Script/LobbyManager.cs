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
        //Player�̏�����
        GameObject[] p = GameObject.FindGameObjectsWithTag("Player");
        for(int i=0; i<p.Length; i++)
        {
            //�ʒu��߂�
            p[i].transform.position = Vector3.zero;
            //����\�ɂ���
            p[i].GetComponent<PlayerController>().AllControllArrow(true,false);
            //ready�̏�Ԃ�false�ɂ���
            p[i].GetComponent<PlayerController>().ready = false;
            //HP��Energy�S��
            p[i].GetComponent<PlayerStatus>().ResetStatus();

        }
    }

    // Update is called once per frame
    void Update()
    {
        GameObject[] p = GameObject.FindGameObjectsWithTag("Player");
        Debug.Log("�v���C���[�l��"+p.Length);

        //playerInputManager�̃A�N�e�B�u�E��A�N�e�B�u����
        //�v���C���[���l�l�ȏ�Ȃ��A�N�e�B�u�ɂ���
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
