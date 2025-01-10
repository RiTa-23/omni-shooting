using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI;

public class HowToPlayButton : MonoBehaviour
{
    AudioSource audioSource;
    GameObject Panel;
    GameObject HowToPlayCanvas_;
    [SerializeField] AudioClip openWindowSE;
    [SerializeField] PlayerJoinController playerJoinController;
    [SerializeField] HowToPlayCanvas CanvasScript;
    // Start is called before the first frame update
    void Start()
    {
        HowToPlayCanvas_ = GameObject.Find("HowToPlayCanvas");
        Panel = HowToPlayCanvas_.transform.Find("HowToPanel").gameObject;
        audioSource=GetComponent<AudioSource>();
    }

    public void pushButton()
    {
        if(!Panel.activeSelf)
        {
            audioSource.PlayOneShot(openWindowSE);
            Panel.SetActive(true);
            //�p�l���A�j���[�V����
            Panel.transform.localScale = Vector3.zero;
            Panel.transform.DOScale(new Vector3(1, 1, 1), 0.3f).onKill = (() =>
            {
                //���Ԓ�~
                if (Panel.activeSelf)
                {
                    Time.timeScale = 0;
                }
            });
            //ActionMap��UI�ɕύX
            ChangeActionMap("UI");
            playerJoinController.DisablePlayerJoining();
        }
    }
    private void ChangeActionMap(string mapName)
    {
        //�S�v���C���[��ActionMap����ĕύX����
        var p = GameObject.FindGameObjectsWithTag("Player");

        for (int i = 0; i < p.Length; i++)
        {
            p[i].GetComponent<PlayerInput>().SwitchCurrentActionMap(mapName);
        }
    }
}
