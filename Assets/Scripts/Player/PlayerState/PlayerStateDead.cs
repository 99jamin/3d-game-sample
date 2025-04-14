using UnityEngine;

public class PlayerStateDead : MonoBehaviour, IPlayerState
{
    private PlayerController _playerController;

    public void Enter(PlayerController playerController)
    {
        _playerController = playerController;
        _playerController.Animator.SetTrigger("Dead");
    }

    public void Update()
    {
    }

    public void Exit()
    {
        _playerController = null;
    }
}
