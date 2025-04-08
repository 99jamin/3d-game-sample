using UnityEngine;

public class PlayerStateJump : MonoBehaviour, IPlayerState
{
    private PlayerController _playerController;

    public void Enter(PlayerController playerController)
	{
   		_playerController = playerController;
	    _playerController.Animator.SetBool("Jump", true);
	}

	public void Update()
	{
	}

	public void Exit()
	{
		_playerController.Animator.SetBool("Jump", false);
    	_playerController = null;
	}
}
