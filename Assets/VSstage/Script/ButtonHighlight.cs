using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonHighlight : MonoBehaviour
{
    Color normalColor = Color.white;
    Color highlightedColor = Color.gray;
    private Button button;

    void Start()
    {
        button = GetComponent<Button>();
        EventSystem.current.SetSelectedGameObject(gameObject);
    }

    void Update()
    {
        if (EventSystem.current.currentSelectedGameObject == gameObject)
        {
            button.image.color = highlightedColor;
        }
        else
        {
            button.image.color = normalColor;
        }
    }
}
