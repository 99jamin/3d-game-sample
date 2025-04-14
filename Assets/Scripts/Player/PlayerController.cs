using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState { None, Idle, Move, Jump, Attack, Hit, Dead }

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour, IObserver<GameObject>
{
    [Header("Player")]
    [SerializeField] private int maxHealth = 100;

    public int AttackPower => attackPower;
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
    private WeaponController _weaponController;

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
        
        // 무기 할당
        var staffObject = Resources.Load<GameObject>("Player/Weapon/Staff");
        var staff = Instantiate(staffObject, leftHandTransform).GetComponent<WeaponController>();
        staff.Subscribe(this);
        _weaponController = staff;
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

    public void SetHit(EnemyController enemyController, Vector3 direction)
    {
        if (CurrentState != PlayerState.Hit)
        {
            var attackPower = enemyController.AttackPower;
            _currentHealth -= attackPower;
            
            // TODO: UI에 HP 갱신
            
            if (_currentHealth <= 0)
            {
                SetState(PlayerState.Dead);
            }
            else
            {
                SetState(PlayerState.Hit);
                Animator.SetFloat("HitPosX", -direction.x);
                Animator.SetFloat("HitPosZ", -direction.z);
            }
        }
    }

    #region 동작 관련

    public void Rotate(float x, float z)
    {
        // 카메라 설정
        var cameraTransform = Camera.main.transform;
        var cameraForward = cameraTransform.forward;
        var cameraRight = cameraTransform.right;
        
        // Y 값을 0로 설정해서 수평 방향만 고려
        cameraForward.y = 0;
        cameraRight.y = 0;
        
        // 입력 방향에 따라 카메라 기준으로 이동 방향 계산
        var moveDirection = cameraForward * z + cameraRight * x;
        
        // 이동 방향이 있을 경우에만 회전
        if (moveDirection != Vector3.zero)
        {
            moveDirection.Normalize();
            transform.rotation = Quaternion.LookRotation(moveDirection);
        }
    }

    public void Jump()
    {
        _velocity.y = Mathf.Sqrt(jumpSpeed * -2f * _gravity);
    }

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
        if (CurrentState == PlayerState.Attack)
        {
            _playerStateAttack.IsAttacking = true;
            _weaponController.AttackStart();
        }
    }

    public void MeleeAttackEnd()
    {
        if (CurrentState == PlayerState.Attack)
        {
            _playerStateAttack.IsAttacking = false;
            _weaponController.AttackEnd();
        }
    }

    #endregion
    
    #region 계산관련

    // 바닥과 거리를 계산하는 함수
    public float GetDistanceToGround()
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

    #region 무기 관련

    public void OnNext(GameObject value)
    {
        var enemyController = value.GetComponent<EnemyController>();
        if (enemyController)
        {
            enemyController.SetHit(this);
        }
    }

    public void OnError(Exception error) { }

    public void OnCompleted()
    {
        _weaponController.Unsubscribe(this);
    }
    
    #endregion
}
