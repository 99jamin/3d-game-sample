using UnityEngine;

public class PlayerStateIdle : MonoBehaviour, IPlayerState
{
    private PlayerController _playerController;
    
    public void Enter(PlayerController playerController)
    {
        _playerController = playerController;
        _playerController.Animator.SetBool("Idle", true);
    }

    public void Update()
    {
    }

    public void Exit()
    {
        _playerController.Animator.SetBool("Idle", false);
        _playerController = null;
    }
}