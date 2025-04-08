using UnityEngine;

public class PlayerStateAttack : MonoBehaviour, IPlayerState
{
    private PlayerController _playerController;

    public void Enter(PlayerController playerController)
    {
        _playerController = playerController;
        _playerController.Animator.SetTrigger("Attack");
    }

    public void Update()
    {
    }

    public void Exit()
    {
        _playerController = null;
    }
}
