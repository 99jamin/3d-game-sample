using UnityEngine;

public class EnemyStateIdle : IEnemyState
{
    private EnemyController _enemyController;
    private float _patrolWaitTime;
    
    public void Enter(EnemyController enemyController)
    {
        _enemyController = enemyController;
        _enemyController.EnemyAnimator.SetBool("Idle", true);

        if (_enemyController.Agent.enabled == true)
        {
            _enemyController.Agent.isStopped = true;
        }
    }

    public void Update()
    {
        // 플레이어 감지
        // 일정 거리에 플레이어가 있는지 확인
        var detectPlayerTransform = _enemyController.DetectPlayerInCircle();
        if (detectPlayerTransform)
        {
            _enemyController.SetState(EnemyState.Trace);
            return;
        }
        
        // 정찰 여부 판단
        if (_patrolWaitTime > _enemyController.MaxPatrolWaitTime && Random.Range(0, 100) < 30)
        {
            _enemyController.SetState(EnemyState.Patrol);
            return;
        }
        _patrolWaitTime += Time.deltaTime;
    }

    public void Exit()
    {
        _patrolWaitTime = 0;
        
        _enemyController.EnemyAnimator.SetBool("Idle", false);
        _enemyController = null;
    }
}