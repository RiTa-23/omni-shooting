using UnityEngine.InputSystem;
using UnityEngine;

public class PlayerJoinController : MonoBehaviour
{
    public PlayerInputManager playerInputManager;

    // プレイヤーの入室を禁止
    public void DisablePlayerJoining()
    {
        playerInputManager.DisableJoining();
    }

    // プレイヤーの入室を許可
    public void EnablePlayerJoining()
    {
        playerInputManager.EnableJoining();
    }
}
