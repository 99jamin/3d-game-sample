using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState { Idle, Patrol, Trace, Attack, Hit, Dead }

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyControllerOld : MonoBehaviour
{
    [Header("Enemy")] 
    [SerializeField] private int attackPower = 1; 
    [SerializeField] private int maxHealth = 100;

    [Header("AI")] 
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private float detectCircleRadius = 10f;
    
    private Animator _enemyAnimator;
    private NavMeshAgent _navMeshAgent;

    private int _currentHealth;
    
    private EnemyState _currentState;
    public EnemyState CurrentState => _currentState;
    
    // -----
    // AI
    private Transform _playerTransform;     // 감지된 플레이어 트랜스폼
    
    private void Awake()
    {
        _enemyAnimator = GetComponent<Animator>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _navMeshAgent.updatePosition = false;
        _navMeshAgent.updateRotation = true;
    }

    private void Start()
    {
        _currentHealth = maxHealth;
        
        SetState(EnemyState.Idle);
    }
    
    private void Update()
    {
        switch (_currentState)
        {
            case EnemyState.Idle:
            {
                // 플레이어 감지
                var detectPlayer = DetectPlayerInCircle();
                if (detectPlayer)
                {
                    _playerTransform = detectPlayer;
                    SetState(EnemyState.Trace);
                }
                
                break;
            }
            case EnemyState.Patrol:
                break;
            case EnemyState.Trace:
                break;
            case EnemyState.Attack:
                break;
            case EnemyState.Hit:
                break;
            case EnemyState.Dead:
                break;
        }
    }

    public void SetState(EnemyState newState)
    {
        // Enter
        switch (newState)
        {
            case EnemyState.Idle:
            {
                // Idle 상태에서는 Agent의 이동을 중지
                _navMeshAgent.isStopped = true;
                // Idle 애니메이션 재생
                _enemyAnimator.SetBool("Idle", true);
                break;   
            }
            case EnemyState.Patrol:
            {
                // Patrol 애니메이션 재생
                _enemyAnimator.SetBool("Patrol", true);
                break;
            }
            case EnemyState.Trace:
            {
                // 감지된 플레이어를 향해 이동하기
                _navMeshAgent.isStopped = false;
                _navMeshAgent.SetDestination(_playerTransform.position);
                
                // Trace 애니메이션 재생
                _enemyAnimator.SetBool("Trace", true);
                break;
            }
            case EnemyState.Attack:
                break;
            case EnemyState.Hit:
                break;
            case EnemyState.Dead:
                break;
        }

        // Exit
        switch (_currentState)
        {
            case EnemyState.Idle:
            {
                // Idle 애니메이션 끄기
                _enemyAnimator.SetBool("Idle", false);
                break;
            }
            case EnemyState.Patrol:
            {
                // Patrol 애니메이션 끄기
                _enemyAnimator.SetBool("Patrol", false);
                break;
            }
            case EnemyState.Trace:
            {
                // Trace 애니메이션 끄기
                _enemyAnimator.SetBool("Trace", false);
                break;
            }
            case EnemyState.Attack:
                break;
            case EnemyState.Hit:
                break;
            case EnemyState.Dead:
                break;
        }
        
        _currentState = newState;
    }

    #region 적 감지

    // 일정 반경에 플레이어가 진입하면 플레이어 소리를 감지했다고 판단
    private Transform DetectPlayerInCircle()
    {
        var hitColliders = Physics.OverlapSphere(transform.position, detectCircleRadius, targetLayer);
        if (hitColliders.Length > 0)
        {
            return hitColliders[0].transform;
        }
        else
        {
            return null;
        }
    }
    

    #endregion

    #region 동작 처리

    private void OnAnimatorMove()
    {
        var position = _enemyAnimator.rootPosition;

        position.y = _navMeshAgent.nextPosition.y;
        
        _navMeshAgent.nextPosition = position;
        transform.position = position;
    }

    #endregion

    #region 디버깅

    private void OnDrawGizmos()
    {
        // Circle 감지 범위
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectCircleRadius);
    }

    #endregion
}
