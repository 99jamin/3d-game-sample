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
    
    public Animator EnemyAnimator { get; private set; }
    
    private int _currentHealth;
    private EnemyState _currentState;
    
    private NavMeshAgent _navMeshAgent;

    private void Awake()
    {
        EnemyAnimator = GetComponent<Animator>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _navMeshAgent.updatePosition = false;
        _navMeshAgent.updateRotation = false;
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
                break;
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
        switch (newState)
        {
            case EnemyState.Idle:
                break;
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

        switch (_currentState)
        {
            case EnemyState.Idle:
                break;
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
        
        _currentState = newState;
    }

    #region 동작 처리

    private void OnAnimatorMove()
    {
        var position = EnemyAnimator.rootPosition;

        position.y = _navMeshAgent.nextPosition.y;
        
        _navMeshAgent.nextPosition = position;
        transform.position = position;
    }

    #endregion

    #region 디버깅

    private void OnDrawGizmos()
    {
        
    }

    #endregion
}
