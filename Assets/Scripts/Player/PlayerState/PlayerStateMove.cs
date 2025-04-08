using UnityEngine;

public class PlayerStateMove : MonoBehaviour, IPlayerState
{
    private PlayerController _playerController;
    private float _speed = 0f;
    
    public void Enter(PlayerController playerController)
    {
        _playerController = playerController;
        _playerController.Animator.SetBool("Move", true);
    }

    public void Update()
    {
        var inputVertical = Input.GetAxis("Vertical");
        var inputHorizontal = Input.GetAxis("Horizontal");

        if (inputVertical != 0 || inputHorizontal != 0)
        {
            _playerController.Rotate(inputHorizontal, inputVertical);
        }
        else
        {
            _playerController.SetState(PlayerState.Idle);
            return;
        }
        
        // 점프
        if (Input.GetButtonDown("Jump") && _playerController.IsGrounded)
        {
            _playerController.SetState(PlayerState.Jump);
            return;
        }
        
        // 공격
        if (Input.GetButtonDown("Fire1") && _playerController.IsGrounded)
        {
            _playerController.SetState(PlayerState.Attack);
            return;
        }
        
        // Left Shift 버튼 누르면 속도 증가
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (_speed < 1f)
            {
                _speed += Time.deltaTime;
                _speed = Mathf.Clamp01(_speed);
            }
        }
        else
        {
            if (_speed > 0f)
            {
                _speed -= Time.deltaTime;
                _speed = Mathf.Clamp01(_speed);
            }
        }
        _playerController.Animator.SetFloat("Speed", _speed);
    }

    public void Exit()
    {
        _playerController.Animator.SetBool("Move", false);
        _playerController = null;
    }
}