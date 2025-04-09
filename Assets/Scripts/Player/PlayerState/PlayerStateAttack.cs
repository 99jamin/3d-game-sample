using UnityEngine;

public class PlayerStateAttack : MonoBehaviour, IPlayerState
{
    private PlayerController _playerController;
    public bool IsAttacking { get; set; }

    public void Enter(PlayerController playerController)
    {
        _playerController = playerController;
        _playerController.Animator.SetTrigger("Attack");
    }

    public void Update()
    {
        if (Input.GetButtonDown("Fire1") 
            && _playerController.IsGrounded
            && !IsAttacking)
        {
            _playerController.Animator.SetTrigger("Attack");
            return;
        }
    }

    public void Exit()
    {
        _playerController = null;
    }
}
