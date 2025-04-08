using UnityEngine;

public class PlayerStateMove : MonoBehaviour, IPlayerState
{
    private PlayerController _playerController;
    
    public void Enter(PlayerController playerController)
    {
        _playerController = playerController;
        _playerController.Animator.SetBool("Move", true);
    }

    public void Update()
    {
    }

    public void Exit()
    {
        _playerController.Animator.SetBool("Move", false);
        _playerController = null;
    }
}