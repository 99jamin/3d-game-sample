using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

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
    [SerializeField] private float maxPatrolWaitTime = 3f;
    [SerializeField] private float detectSightAngle = 30f;
    
    private Animator _enemyAnimator;
    private NavMeshAgent _navMeshAgent;

    // 적의 체력
    private int _currentHealth;
    
    // 적의 현재 상태
    private EnemyState _currentState;
    public EnemyState CurrentState => _currentState;
    
    // 플레이어의 추적 위치를 업데이트 하는 코루틴
    private Coroutine _updateDestinationCoroutine;

    // 플레이어의 거리를 비교하기 위한 변수
    private float _detectCircleRadiusSqr;
    // 누적 패트롤 대기 시간
    private float _patrolWaitTime;
    
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
        _detectCircleRadiusSqr = detectCircleRadius * detectCircleRadius;
        _patrolWaitTime = 0f;
        
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
                    break;
                }
                
                // 정찰 여부 판단
                if (_patrolWaitTime > maxPatrolWaitTime && Random.Range(0, 100) < 30)
                {
                    // 정찰하기로 결정
                    SetState(EnemyState.Patrol);
                    break;
                }
                _patrolWaitTime += Time.deltaTime;
                
                break;
            }
            case EnemyState.Patrol:
            {
                // 플레이어 감지
                var detectPlayer = DetectPlayerInCircle();
                if (detectPlayer)
                {
                    _playerTransform = detectPlayer;
                    SetState(EnemyState.Trace);
                    break;
                }
                
                // 패트롤 위치에 도착하면 Idle 상태로 전환
                if (!_navMeshAgent.pathPending && _navMeshAgent.remainingDistance < 0.1f)
                {
                    SetState(EnemyState.Idle);
                    break;
                }
                break;
            }
            case EnemyState.Trace:
            {
                // 플레이어와 거리 계산
                var playerDistanceSqr = (_playerTransform.position - transform.position).sqrMagnitude;
                
                // 트레이스 중 시야에 플레이어가 들어오면 속도 증가
                if (DetectPlayerInSight(_playerTransform))
                {
                    _enemyAnimator.SetFloat("Speed", 1f);
                }
                else
                {
                    _enemyAnimator.SetFloat("Speed", 0f);
                }
             
                // 일정 거리 이상으로 플레이어가 멀어지면 Idle로 전환
                if (playerDistanceSqr > _detectCircleRadiusSqr)
                {
                    SetState(EnemyState.Idle);
                }
                break;
            }
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
                // 찾아야 할 플레이어 정보 초기화
                _playerTransform = null;
                // 패트롤 대기 시간 초기화
                _patrolWaitTime = 0f;
                // Idle 상태에서는 Agent의 이동을 중지
                _navMeshAgent.isStopped = true;
                // Idle 애니메이션 재생
                _enemyAnimator.SetBool("Idle", true);
                break;   
            }
            case EnemyState.Patrol:
            {
                // 랜덤으로 정찰 위치를 구하고, 있으면 해당 위치로 이동, 없으면 다시 Idle 상태로 전환
                var patrolPoint = FindRandomPatrolPoint();
                if (patrolPoint == transform.position)
                {
                    SetState(EnemyState.Idle);
                    break;
                }
                _navMeshAgent.isStopped = false;
                _navMeshAgent.SetDestination(patrolPoint);
                
                // Patrol 애니메이션 재생
                _enemyAnimator.SetBool("Patrol", true);
                break;
            }
            case EnemyState.Trace:
            {
                // 감지된 플레이어를 향해 이동하기
                _navMeshAgent.isStopped = false;
                _updateDestinationCoroutine = StartCoroutine(UpdateDestination());                
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
                // 플레이어 위치 갱신하는 코루틴 중지
                if (_updateDestinationCoroutine != null)
                {
                    StopCoroutine(_updateDestinationCoroutine);
                    _updateDestinationCoroutine = null;
                }
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

    private Vector3 FindRandomPatrolPoint()
    {
        Vector3 randomDirection = Random.insideUnitSphere * detectCircleRadius;
        randomDirection += transform.position;
        
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, detectCircleRadius, NavMesh.AllAreas))
        {
            return hit.position;
        }
        else
        {
            return transform.position;            
        }
    }
    
    IEnumerator UpdateDestination()
    {
        while (_playerTransform)
        {
            _navMeshAgent.SetDestination(_playerTransform.position);
            yield return new WaitForSeconds(0.5f);
        }
    }

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
    
    // 일정 반경에 플레이어가 진입하면 시야에 들어왔다고 판단하는 함수
    private bool DetectPlayerInSight(Transform playerTransform)
    {
        if (playerTransform == null)
        {
            return false;
        }

        // Vector3 direction = playerTransform.position - transform.position;
        // float angle = Vector3.Angle(direction, transform.forward);
        
        var cosTheta = Vector3.Dot(transform.forward, 
            (playerTransform.position - transform.position).normalized);
        var angle = Mathf.Acos(cosTheta) * Mathf.Rad2Deg;
        
        if (angle < detectSightAngle)
        {
            return true;
        }
        else
        {
            return false;
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
        
        // 시야각
        Gizmos.color = Color.red;
        Vector3 rightDirection = Quaternion.Euler(0, detectSightAngle, 0) * transform.forward;
        Vector3 leftDirection = Quaternion.Euler(0, -detectSightAngle, 0) * transform.forward;
        Gizmos.DrawRay(transform.position, rightDirection * detectCircleRadius);
        Gizmos.DrawRay(transform.position, leftDirection * detectCircleRadius);
        Gizmos.DrawRay(transform.position, transform.forward * detectCircleRadius);
    }

    #endregion
}
