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
    //�X�|�[���n�_
    [SerializeField] Vector2 spawnsize;
    [SerializeField] GameObject tempPos;
    [SerializeField] GameObject spawnEffect;
    //�A�C�e��
    [SerializeField] GameObject ItemPrefab;
    public bool isItemOn=true;//�A�C�e�����肩
    [SerializeField]float spawnItemTime = 10;
    float spawnDeltaTime = 0;
    bool isSpawnItem;
    //�v���C���[�I�u�W�F�N�g
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

    //�v���C���[�l��
    int playerNum;
    public int aliveNum;//�����c��̐�

    //���U���g�p
    public int[] killNum;//�|�����G�̐�
    public float[] giveDamage;//�G�ɗ^�����_���[�W��
    public int[] Rank;//���ʂ�
    public bool isResultWait=false;//���U���g�ҋ@��ʒ���
    public bool[] isResultOk;//���U���g�m�F

    // Start is called before the first frame update
    void Start()
    {
        //������
        BGM.Stop();
        tempPos = GameObject.Find("tempPos").gameObject;
        audioSource=GetComponent<AudioSource>();

        p = GameObject.FindGameObjectsWithTag("Player");
        Debug.Log("�v���C���[�l��" + p.Length);
        playerNum = p.Length;
        aliveNum = playerNum;

        killNum=new int[playerNum];
        giveDamage = new float[playerNum];
        Rank = new int[playerNum];
        isResultOk = new bool[playerNum];

        for(int i = 0; i < playerNum; i++)
        {
            isResultOk[i] = false;
            //�X�e�[�^�X�A���x��������
            p[i].GetComponent<PlayerStatus>().ResetStatus();
            p[i].GetComponent<PlayerController>().ResetControllerStatus();
            //�ꎞ�I�Ƀv���C���[���X�e�[�W�O�ɑޔ�
            p[i].transform.position = tempPos.transform.position;
            //PlayerStatus�̕ϐ�GM���i�[
            p[i].GetComponent<PlayerStatus>().FindGM();
        }
        //�����_���Ȉʒu�Ƀv���C���[���X�|�[��
        StartCoroutine(spawnPlayer());
        //�Q�[���J�n
        StartCoroutine(ReadyGo());

    }

    private void spawnItem()
    {
        //�����_���Ȉʒu�ɃA�C�e�����X�|�[��
        //�m���Ŕr�o�������߂�
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
            //�|�W�V�����`�F�b�N
            bool isPosBad = true;

            while (isPosBad)
            {
                //�����_���X�|�[��
                x = Random.Range(-spawnsize.x, spawnsize.x);
                y = Random.Range(-spawnsize.y, spawnsize.y);

                isPosBad = false;
                for (int j = 0; j < i; j++)
                {
                    //���̃v���C���[�Ƃ̋������߂�����ꍇ�͂�����x
                    if ((temp_playerpos[j] - new Vector3(x, y, 0)).magnitude < 6)
                    {
                        print("�Ĕz�u");
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
        //�Q�[���J�n�G�t�F�N�g(UI,SE,BGM)
        GameObject ready = frontCanvas.transform.Find("Ready").gameObject;
        GameObject go = frontCanvas.transform.Find("Go").gameObject;

        //�X�|�[���ҋ@
        yield return new WaitForSeconds(0.25f*(playerNum+1));

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
        //�v���C���[�����P�ȉ��ɂȂ�����Result�Ɉڍs
        if (aliveNum <= 1 && !isResult)
        {
            isResult = true;
            StartCoroutine(Result());
        }

        //�f�o�b�O�p
        if (p.Length == 0)
            SceneManager.LoadScene("LobbyScene");

        //�A�C�e���X�|�[��
        if (isItemOn)//�A�C�e������Ȃ�
        {
            if (!isSpawnItem)
            {
                isSpawnItem = true;
                spawnItem();
            }
            else//�C���^�[�o��
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
            //�ZP Win!�I�Ȃ̂��o�� + Result
            GameObject winner = result.transform.Find("winner").gameObject;

            //�����c�����v���C���[�����
            int winnerNum=-1;
            for(int i=0;i<p.Length;i++)
            {
                if(!p[i].GetComponent<PlayerStatus>().isDead)
                {
                    winnerNum = i;
                }
            }
            //winner�̃����N��1�ɂ���
            Rank[winnerNum] = 1;

            //wins!�̃G�t�F�N�g
            winnerColor = p[winnerNum].GetComponent<PlayerStatus>().colorCode;
            winner.GetComponent<Text>().text ="<color="+winnerColor+">"+(winnerNum+1)+"P</color> Wins!";
            audioSource.PlayOneShot(winnerSE);
            winner.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
            winner.SetActive(true);

            yield return new WaitForSeconds(3);

            //�����c���Ă�v���C���[�I�u�W�F�N�g�𑀍�s�A�ޔ�
            if (aliveNum !=0)
            {
                p[winnerNum].GetComponent<PlayerController>().AllControllArrow(false,false);
                p[winnerNum].transform.position=tempPos.transform.position;
                //���x��0�ɂ���
                p[winnerNum].GetComponent<Rigidbody2D>().velocity = Vector3.zero;
            }

            //��Ɉړ�
            winner.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0, 170), 2f)
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
            Draw.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0, 170), 2f)
    .SetEase(Ease.OutBack);
            yield return new WaitForSeconds(2);
        }

        //result���o��
        GameObject resultPanel = result.transform.Find("resultPanel").gameObject;
        resultPanel.SetActive(true);
        audioSource.PlayOneShot(resultSE);
        resultPanel.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0, 70), 1f)
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
            GameObject resultStatus = pP[i].transform.Find("resultStatus").gameObject;

            //Rank�̃}�[�N������
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
        //�v���C���[�p�l���ړ�
        playerPanels.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0, -105), 1f)
.SetEase(Ease.OutBack);

        //�S���̉E�{�^���������ꂽ�烍�r�[�ɖ߂�
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
                    //���U���g�m�FOK!�̏���
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
