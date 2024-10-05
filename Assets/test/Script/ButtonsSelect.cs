using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonsSelect : MonoBehaviour
{
    private Button button;
    bool isSelect;

    void Start()
    {
        button = GetComponent<Button>();
        EventSystem.current.SetSelectedGameObject(gameObject);
    }

    void Update()
    {
        if (EventSystem.current.currentSelectedGameObject == gameObject&&!isSelect)
        {
            isSelect = true;
            button.onClick.Invoke();
        }
        else
        {
            isSelect = false;
        }
    }
}
