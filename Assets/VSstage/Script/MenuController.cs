using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class MenuController : MonoBehaviour
{
    private Selectable[] selectables; // ���j���[�̑I���\�ȗv�f�̔z��
    private int currentIndex = 0; // ���ݑI������Ă���v�f�̃C���f�b�N�X
    PlayerInput _playerInput;
    private InputAction _NavigateAction;

    void Start()
    {
        _playerInput = GetComponent<PlayerInput>();
        _NavigateAction = _playerInput.actions["Navigate"];
        // �A�N�e�B�u��UI�v�f���擾���āAselectables�z����X�V����
        selectables = GetComponentsInChildren<Selectable>(true);

        if (selectables.Length > 0)
        {
            // �ŏ��̗v�f��I������
            currentIndex = 0;
            selectables[currentIndex].Select();
        }
    }

    // �㉺���E�̓��͂ɉ����ă��j���[���i�r�Q�[�g����
    public void Navigate(Vector2 direction)
    {
        if (selectables == null || selectables.Length == 0)
        {
            // selectables��null�܂��͋�̏ꍇ�A�������I������
            return;
        }

        if (direction.y > 0) // ������̓���
        {
            currentIndex = (currentIndex == 0) ? selectables.Length - 1 : currentIndex - 1;
        }
        else if (direction.y < 0) // �������̓���
        {
            currentIndex = (currentIndex == selectables.Length - 1) ? 0 : currentIndex + 1;
        }
        else if (direction.x > 0) // �E�����̓���
        {
            currentIndex = (currentIndex == selectables.Length - 1) ? 0 : currentIndex + 1;
        }
        else if (direction.x < 0) // �������̓���
        {
            currentIndex = (currentIndex == 0) ? selectables.Length - 1 : currentIndex - 1;
        }

        // currentIndex���z��͈̔͊O�̏ꍇ�A�K�؂ȃC���f�b�N�X�ɕ␳
        if (currentIndex < 0)
        {
            currentIndex = selectables.Length - 1;
        }
        else if (currentIndex >= selectables.Length)
        {
            currentIndex = 0;
        }

        // �i�r�Q�[�V�����őI����Ԃ��X�V
        selectables[currentIndex].Select();
    }
}
