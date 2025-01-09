using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    //ready��Ԃ�
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

    [SerializeField]GameObject playerInputManager;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        //Player�̏�����
        p = GameObject.FindGameObjectsWithTag("Player");
        for(int i=0; i<p.Length; i++)
        {
            //�ʒu��߂�
            p[i].transform.position = Vector3.zero;
            //����\�ɂ���
            p[i].GetComponent<PlayerController>().AllControllArrow(true,false);
            //HP��Energy�S��
            p[i].GetComponent<PlayerStatus>().ResetStatus();
            //LobbyManager���
            p[i].GetComponent<PlayerController>().lobbyManager = this;
            //PlayerController�X�e�[�^�X��������
            p[i].GetComponent<PlayerController>().ResetControllerStatus();
            //ItemList�̏�����
            p[i].GetComponent<PlayerItem>().ResetItemList();

        }
    }
    [SerializeField] GameObject defaultGameObject;

    // Update is called once per frame
    void Update()
    {
        //UI����
        UIController();

        p = GameObject.FindGameObjectsWithTag("Player");
        Debug.Log("�v���C���[�l��"+p.Length);

        //�l���ȏ��playerBord������Δ�A�N�e�B�u�ɂ���
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
            //playerBord����A�N�e�B�u�Ȃ�A�N�e�B�u�ɂ���
            if (!playerBord[i].activeSelf)
            {
                playerBord[i].SetActive(true);
            }
            //�ŏ��̈�񂾂��s�����������̏���
            if (ready[i]&&!playerBord[i].transform.Find("Ready").gameObject.activeSelf)
            {
                print("�v���C���[" + i + "���������I");
                audioSource.PlayOneShot(ReadySE);
                readyNum++;
                ReadyOrStandby(playerBord[i],true);
            }
            //�����������I�t�ɂ���i�N�����ގ������Ƃ��Ɏ��s�����j
            if (!ready[i]&& playerBord[i].transform.Find("Ready").gameObject.activeSelf)
            {
                ReadyOrStandby(playerBord[i], false);
            }
        }

        //�v���C���[�S�������������Ȃ�X�e�[�W�Ɉړ�������
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
    //���ԍ��ŃV�[����ǂݍ���
    IEnumerator LoadScene()
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("vs_stage");
    }

    //Ready�܂���standby��Ԃɕω������Ƃ��̏����itrue�Ȃ�Ready�j
    void ReadyOrStandby(GameObject playerBord,bool isReady)
    {
        playerBord.transform.Find("pull the triger").gameObject.SetActive(!isReady);
        playerBord.transform.Find("stand-by").gameObject.SetActive(!isReady);
        playerBord.transform.Find("Ready").gameObject.SetActive(isReady);
    }

    //�ގ�
    public void ExitLobby(int playerNum)
    {
        audioSource.PlayOneShot(ExitSE);
        //�ގ�����v���C���[������
        Destroy(p[playerNum].transform.parent.gameObject);

        //�v���C���[�̃��Z�b�g
        p = GameObject.FindGameObjectsWithTag("Player");
        for(int i=0; i<p.Length;i++)
        {
            if (i!=playerNum)
            p[i].GetComponent<PlayerStatus>().ResetPlayer();
        }

        //ready�̃��Z�b�g
        for(int i=0;i<4;i++)
        {
            ready[i] = false;
        }
        readyNum = 0;

    }

    //���r�[�ł�UI����
    [SerializeField] GameObject buttons;
    [SerializeField] EventSystem eventSystem;
    GameObject HowToPanel;
    GameObject[] button;
    bool isMenuOpen;
    void UIController()
    {
        //���ݑI������Ă���I�u�W�F�N�g�m�F
        print(eventSystem.currentSelectedGameObject);

        HowToPanel = GameObject.Find("HowToPanel");
        //�X�y�[�X�L�[�ŗV�ѕ�
        if(Input.GetKeyDown(KeyCode.Space))
        {
            //�V�ѕ��I�[�v��
            GameObject menuButton = GameObject.FindWithTag("Menu");
            if (menuButton != null&& HowToPanel==null)
            {
                menuButton.GetComponent<Button>().onClick.Invoke();
            }
            //�V�ѕ��N���[�Y
            GameObject XButton = GameObject.FindWithTag("returnPlayer");
            if (XButton != null&& HowToPanel!=null)
            {
                XButton.GetComponent<Button>().onClick.Invoke();
            }
        }
        //�y�[�W�ړ�
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
                    print("selectNum�F" + i);
                }
                print(button[i] + "�F" + i);
            }
            //�E�Ɉړ�
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                print("�E���");
                if (selectNum != buttons.transform.childCount - 1)
                {
                    button[selectNum + 1].GetComponent<Button>().onClick.Invoke();
                }
                else
                {
                    print("�E�[���獶�[��");
                    button[0].GetComponent<Button>().onClick.Invoke();
                }

            }
            //���Ɉړ�
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                print("�����");
                if (selectNum != 0)
                {
                    button[selectNum - 1].GetComponent<Button>().onClick.Invoke();
                }
                else
                {
                    //��ԍ��[
                    print("���[����E�[��");
                    button[buttons.transform.childCount - 1].GetComponent<Button>().onClick.Invoke();
                }
            }
        }
    }
}
