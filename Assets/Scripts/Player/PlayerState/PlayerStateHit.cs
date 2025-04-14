using UnityEngine;

public class PlayerStateHit : MonoBehaviour, IPlayerState
{
    private PlayerController _playerController;

    public void Enter(PlayerController playerController)
    {
        _playerController = playerController;
        _playerController.Animator.SetTrigger("Hit");
    }

    public void Update()
    {
    }

    public void Exit()
    {
        _playerController = null;
    }
}