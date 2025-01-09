using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class MenuController : MonoBehaviour
{
    private Selectable[] selectables; // メニューの選択可能な要素の配列
    private int currentIndex = 0; // 現在選択されている要素のインデックス
    PlayerInput _playerInput;
    private InputAction _NavigateAction;

    void Start()
    {
        _playerInput = GetComponent<PlayerInput>();
        _NavigateAction = _playerInput.actions["Navigate"];
        // アクティブなUI要素を取得して、selectables配列を更新する
        selectables = GetComponentsInChildren<Selectable>(true);

        if (selectables.Length > 0)
        {
            // 最初の要素を選択する
            currentIndex = 0;
            selectables[currentIndex].Select();
        }
    }

    // 上下左右の入力に応じてメニューをナビゲートする
    public void Navigate(Vector2 direction)
    {
        if (selectables == null || selectables.Length == 0)
        {
            // selectablesがnullまたは空の場合、処理を終了する
            return;
        }

        if (direction.y > 0) // 上向きの入力
        {
            currentIndex = (currentIndex == 0) ? selectables.Length - 1 : currentIndex - 1;
        }
        else if (direction.y < 0) // 下向きの入力
        {
            currentIndex = (currentIndex == selectables.Length - 1) ? 0 : currentIndex + 1;
        }
        else if (direction.x > 0) // 右向きの入力
        {
            currentIndex = (currentIndex == selectables.Length - 1) ? 0 : currentIndex + 1;
        }
        else if (direction.x < 0) // 左向きの入力
        {
            currentIndex = (currentIndex == 0) ? selectables.Length - 1 : currentIndex - 1;
        }

        // currentIndexが配列の範囲外の場合、適切なインデックスに補正
        if (currentIndex < 0)
        {
            currentIndex = selectables.Length - 1;
        }
        else if (currentIndex >= selectables.Length)
        {
            currentIndex = 0;
        }

        // ナビゲーションで選択状態を更新
        selectables[currentIndex].Select();
    }
}
