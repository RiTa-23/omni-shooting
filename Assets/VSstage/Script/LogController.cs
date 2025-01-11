using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class LogController : MonoBehaviour
{
    [SerializeField]GameObject LogPanel;
    TextMeshProUGUI LogText;
    bool isOpen = false;

    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        //D-pad（十字キー）の取得
        bool isDup = false;
        bool isDdown = false;
        foreach (var gamepad in Gamepad.all)
        {
            if (gamepad.dpad.up.wasPressedThisFrame)
            {
                Debug.Log("D-pad Up pressed");
                isDup = true;
            }
            else if (gamepad.dpad.down.wasPressedThisFrame)
            {
                Debug.Log("D-pad Down pressed");
                isDdown = true;
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!isOpen)
            {
                LogOpne();
            }
            else
            {
                LogClose();
            }
        }
        //コントローラー十字での操作
        if(isDup)
        {
            LogOpne();
        }
        if(isDdown)
        {
            LogClose();
        }
    }
    void LogOpne()
    {
        LogPanel.GetComponent<RectTransform>().DOAnchorPos(new Vector2(777, 60), 0.5f);
        isOpen = true;
    }
    void LogClose()
    {
        LogPanel.GetComponent<RectTransform>().DOAnchorPos(new Vector2(777, -85), 0.5f);
        isOpen = false;
    }
}