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
    //�X�|�[���n�_
    [SerializeField] GameObject spawnPos;
    private GameObject spawn1;//����
    private GameObject spawn2;//�E��
    private GameObject tempPos;
    [SerializeField] GameObject spawnEffect;
    //�v���C���[�I�u�W�F�N�g
    public GameObject[] p;
    //Canvas
    [SerializeField] GameObject frontCanvas;

    //SE&BGM
    AudioSource audioSource;
    [SerializeField] AudioClip spawnSE;
    [SerializeField] AudioClip readySE;
    [SerializeField] AudioClip goSE;
    [SerializeField] AudioSource BGM;

    //�v���C���[�l��
    int playerNum;

    //���U���g�p
    public int[] killNum;//�|�����G�̐�
    public int[] giveDamage;//�G�ɗ^�����_���[�W��
    public int[] Rank;//���ʂ�

    // Start is called before the first frame update
    void Start()
    {
        BGM.Stop();
        spawn1 = spawnPos.transform.Find("1").gameObject;
        spawn2 = spawnPos.transform.Find("2").gameObject;
        tempPos = spawnPos.transform.Find("tempPos").gameObject;
        audioSource=GetComponent<AudioSource>();

        p = GameObject.FindGameObjectsWithTag("Player");
        Debug.Log("�v���C���[�l��" + p.Length);
        playerNum = p.Length;

        killNum=new int[playerNum];
        giveDamage = new int[playerNum];
        Rank = new int[playerNum];

        //�ꎞ�I�Ƀv���C���[���X�e�[�W�O�ɑޔ�
        for(int i = 0; i < p.Length; i++)
        {
            p[i].transform.position = tempPos.transform.position;
        }
        //�����_���Ȉʒu�Ƀv���C���[���X�|�[��
        StartCoroutine(spawnPlayer());
        //�Q�[���J�n
        StartCoroutine(ReadyGo());

    }
    IEnumerator spawnPlayer()
    {
        for (int i = 0; i < p.Length; i++)
        {
            //�����_���X�|�[��
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
        //�Q�[���J�n�G�t�F�N�g(UI,SE,BGM)
        GameObject ready = frontCanvas.transform.Find("Ready").gameObject;
        GameObject go = frontCanvas.transform.Find("Go").gameObject;

        //�X�|�[���ҋ@
        yield return new WaitForSeconds(0.25f*(p.Length+1));

        //ReadyGo�G�t�F�N�g
        ready.SetActive(true);
        ready.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0, 0), 1.5f);
        audioSource.PlayOneShot(readySE);
        yield return new WaitForSeconds(readySE.length);
        ready.SetActive(false);
        go.SetActive(true);
        go.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0, 0), 0.3f);
        audioSource.PlayOneShot(goSE);
        
        //���싖��true
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

        //�v���C���[�����P�ȉ��ɂȂ�����Result�Ɉڍs
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
            //�ZP Win!�I�Ȃ̂��o�� + Result
            GameObject winner = result.transform.Find("winner").gameObject;
            int winnerNum = p[0].GetComponent<PlayerStatus>().P_Num;
            //winner�̃����N��1�ɂ���
            Rank[winnerNum] = 1;

            winner.GetComponent<Text>().text =(winnerNum+1)+"P Wins!";
            winner.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
            winner.SetActive(true);

            yield return new WaitForSeconds(3);

            //�����c���Ă�v���C���[�I�u�W�F�N�g���폜
            if (p.Length!=0)
            Destroy(p[0].transform.parent.gameObject);

            //��Ɉړ�
            winner.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0, 150), 2f)
    .SetEase(Ease.OutBack);
            yield return new WaitForSeconds(2);
        }
        else
        {
            //Draw�@�I�Ȃ̂��o�� + Result
            GameObject Draw = result.transform.Find("Draw").gameObject;
            Draw.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
            Draw.SetActive(true);
            //��Ɉړ�
            Draw.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0, 150), 2f)
    .SetEase(Ease.OutBack);
            yield return new WaitForSeconds(2);
        }

        //result���o��
        GameObject resultPanel = result.transform.Find("resultPanel").gameObject;
        resultPanel.SetActive(true);
        resultPanel.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0, 57), 1f)
.SetEase(Ease.OutBack);

        GameObject playerPanels = result.transform.Find("playerPanels").gameObject;
        GameObject[] pP = new GameObject[playerNum];
        //�v���C���[�p�l�����v���C�l���ɉ����Ċi�[
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
        //�v���C���[�p�l�����̃e�L�X�g��������
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
            pP[i].transform.Find("killNum").GetComponent<Text>().text = "���j : " + killNum[i];
            pP[i].transform.Find("giveDamage").GetComponent<Text>().text = "�^�_�� : " + giveDamage[i];
            pP[i].SetActive(true);
        }
        //�v���C���[�p�l���ړ�
        playerPanels.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0, -105), 1f)
.SetEase(Ease.OutBack);
        yield return new WaitForSeconds(7);

        //�{�^���������ꂽ�烍�r�[�ɖ߂�

        SceneManager.LoadScene("LobbyScene");
    }
}
