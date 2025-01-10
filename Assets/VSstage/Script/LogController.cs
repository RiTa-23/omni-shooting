using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LogController : MonoBehaviour
{
    [SerializeField]GameObject LogPanel;
    TextMeshProUGUI LogText;
    bool isOpen = false;

    float previousVertical=0f;
    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        float vertical = Input.GetAxisRaw("crossVertical");
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
        if(vertical >= 0.8 && previousVertical < 0.8)
        {
            LogOpne();
        }
        if(vertical <= -0.8 && previousVertical > -0.8)
        {
            LogClose();
        }
        previousVertical = vertical;
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