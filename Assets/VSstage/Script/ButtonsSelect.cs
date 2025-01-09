using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonsSelect : MonoBehaviour
{
    private Button button;
    bool isSelect;
    [SerializeField] EventSystem eventSystem;

    void Start()
    {
        button = GetComponent<Button>();
    }

    void Update()
    {
        if (eventSystem.currentSelectedGameObject == gameObject&&!isSelect)
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
