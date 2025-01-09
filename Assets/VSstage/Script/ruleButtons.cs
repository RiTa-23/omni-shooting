using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HowToPlayCanvas;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ruleButtons : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] HowToPlayCanvas CanvasScript;
    [SerializeField]bool isNowSprit;

    [SerializeField]HowToPlayCanvas.pageState state;

    [SerializeField] Sprite buttonSprite;
    [SerializeField] Sprite pushedSprite;
    Image spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<Image>();

        if (state == pageState.rule1)
            EventSystem.current.SetSelectedGameObject(gameObject);
    }

    private void Update()
    {
        if(EventSystem.current != null)
        {
            if(isNowSprit)
            {
                EventSystem.current.SetSelectedGameObject(gameObject);
            }
        }
    }
    public void pushButton()
    {
        if(!isNowSprit)
        {
            CanvasScript.pushButton(state);
            isNowSprit = true;
            EventSystem.current.SetSelectedGameObject(gameObject);
            spriteRenderer.sprite = pushedSprite;
        }
        
    }

    public void SpriteReset()
    {
        isNowSprit = false;
        spriteRenderer.sprite = buttonSprite;
    }
}
