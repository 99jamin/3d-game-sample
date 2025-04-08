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
        var inputVertical = Input.GetAxis("Vertical");
        var inputHorizontal = Input.GetAxis("Horizontal");
        
        // 이동
        if (inputVertical != 0 || inputHorizontal != 0)
        {
            _playerController.Rotate(inputHorizontal, inputVertical);
            _playerController.SetState(PlayerState.Move);
            return;
        }
        
        // 점프
        if (Input.GetButtonDown("Jump"))
        {
            _playerController.SetState(PlayerState.Jump);
            return;
        }
        
        // 공격
        if (Input.GetButtonDown("Fire1"))
        {
            _playerController.SetState(PlayerState.Attack);
            return;
        }
    }

    public void Exit()
    {
        _playerController.Animator.SetBool("Idle", false);
        _playerController = null;
    }
}