using UnityEngine;

public class PlayerStateJump : MonoBehaviour, IPlayerState
{
    private PlayerController _playerController;

    public void Enter(PlayerController playerController)
	{
   		_playerController = playerController;
	    _playerController.Animator.SetBool("Jump", true);
	    _playerController.Jump();
	}

	public void Update()
	{
		_playerController.Animator.SetFloat("GroundDistance", _playerController.GetDistanceToGround());
	}

	public void Exit()
	{
		_playerController.Animator.SetBool("Jump", false);
    	_playerController = null;
	}
}
