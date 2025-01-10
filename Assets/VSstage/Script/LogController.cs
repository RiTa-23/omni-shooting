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
    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            if (!isOpen)
            {
                LogPanel.GetComponent<RectTransform>().DOAnchorPos(new Vector2(200, -440), 0.5f);
                isOpen = true;
            }
            else
            {
                LogPanel.GetComponent<RectTransform>().DOAnchorPos(new Vector2(200, -700), 0.5f);
                isOpen = false;
            }
        }
    }
}
