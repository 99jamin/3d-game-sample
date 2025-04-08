using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState { None, Idle, Move, Jump, Attack, Hit, Dead }

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int attackPower = 10;
    
    [Header("Movement")]
    [SerializeField] private float jumpSpeed = 2f;
    [SerializeField] private float rotationSpeed = 100f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float maxGroundCheckDistance = 10f;

    [Header("Attach Points")] 
    [SerializeField] private Transform leftHandTransform;
    [SerializeField] private Transform headTransform;

    // -----
    // 상태 관련
    private PlayerStateIdle _playerStateIdle;
    private PlayerStateMove _playerStateMove;
    private PlayerStateJump _playerStateJump;
    private PlayerStateAttack _playerStateAttack;
    private PlayerStateHit _playerStateHit;
    private PlayerStateDead _playerStateDead;
    
    public PlayerState CurrentState { get; private set; }
    private Dictionary<PlayerState, IPlayerState> _playerStates;
    
    // -----
    // 외부 접근 가능 변수
    public Animator Animator { get; private set; }
    public bool IsGrounded
    {
        get
        {
            return GetDistanceToGround() < 0.2f;
        }
    }

    // -----
    // 내부에서만 사용되는 변수
    private CharacterController _characterController;
    private const float _gravity = -9.81f;
    private Vector3 _velocity = Vector3.zero;
    private int _currentHealth = 0;

    private void Awake()
    {
        Animator = GetComponent<Animator>();
        _characterController = GetComponent<CharacterController>();
    }

    private void Start()
    {
        // 상태 초기화
        _playerStateIdle = new PlayerStateIdle();
        _playerStateMove = new PlayerStateMove();
        _playerStateJump = new PlayerStateJump();
        _playerStateAttack = new PlayerStateAttack();
        _playerStateHit = new PlayerStateHit();
        _playerStateDead = new PlayerStateDead();

        _playerStates = new Dictionary<PlayerState, IPlayerState>
        {
            { PlayerState.Idle, _playerStateIdle },
            { PlayerState.Move, _playerStateMove },
            { PlayerState.Jump, _playerStateJump },
            { PlayerState.Attack, _playerStateAttack },
            { PlayerState.Hit, _playerStateHit },
            { PlayerState.Dead, _playerStateDead }
        };
        SetState(PlayerState.Idle);
        
        // 체력 초기화
        _currentHealth = maxHealth;
    }

    private void Update()
    {
        if (CurrentState != PlayerState.None)
        {
            _playerStates[CurrentState].Update();
        }
    }

    public void SetState(PlayerState state)
    {
        if (CurrentState != PlayerState.None)
        {
            _playerStates[CurrentState].Exit();
        }
        CurrentState = state;
        _playerStates[CurrentState].Enter(this);
    }

    #region 애니메이터 관련

    private void OnAnimatorMove()
    {
        Vector3 movePosition;

        if (IsGrounded)
        {
            movePosition = Animator.deltaPosition;
        }
        else
        {
            movePosition = _characterController.velocity * Time.deltaTime;
        }
        
        // 중력 적용
        _velocity.y += _gravity * Time.deltaTime;
        movePosition.y = _velocity.y * Time.deltaTime;
        
        _characterController.Move(movePosition);
    }
    
    public void MeleeAttackStart()
    {
    }

    public void MeleeAttackEnd()
    {
    }

    #endregion
    
    #region 계산관련

    // 바닥과 거리를 계산하는 함수
    private float GetDistanceToGround()
    {
        float maxDistance = 10f;
        if (Physics.Raycast(transform.position, 
                Vector3.down, out RaycastHit hit, maxDistance, groundLayer))
        {
            return hit.distance;
        }
        else
        {
            return maxDistance;
        }
    }    

    #endregion

}
