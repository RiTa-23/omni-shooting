using UnityEngine.InputSystem;
using UnityEngine;

public class PlayerJoinController : MonoBehaviour
{
    public PlayerInputManager playerInputManager;

    // �v���C���[�̓������֎~
    public void DisablePlayerJoining()
    {
        playerInputManager.DisableJoining();
    }

    // �v���C���[�̓���������
    public void EnablePlayerJoining()
    {
        playerInputManager.EnableJoining();
    }
}
