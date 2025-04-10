using UnityEngine;

public class EnemyStatePatrol : IEnemyState
{
    private EnemyController _enemyController;
    
    public void Enter(EnemyController enemyController)
    {
        _enemyController = enemyController;
        
        var patrolPoint = FindRandomPatrolPoint();
        if (patrolPoint == _enemyController.transform.position)
        {
            _enemyController.SetState(EnemyState.Idle);
            return;
        }
        
        _enemyController.Agent.isStopped = false;
        _enemyController.Agent.SetDestination(patrolPoint);
        
        _enemyController.EnemyAnimator.SetBool("Patrol", true);
    }

    public void Update()
    {
        var detectPlayerTransform = _enemyController.DetectPlayerInCircle();
        if (detectPlayerTransform)
        {
            _enemyController.SetState(EnemyState.Trace);
            return;
        }

        var destinationDistance = Vector3.Distance(_enemyController.transform.position,
            _enemyController.Agent.destination);

        if (!_enemyController.Agent.pathPending &&
            destinationDistance < _enemyController.Agent.stoppingDistance)
        {
            _enemyController.SetState(EnemyState.Idle);
            return;
        }
    }

    public void Exit()
    {
        _enemyController.EnemyAnimator.SetBool("Patrol", false);
        _enemyController = null;
    }
    
    private Vector3 FindRandomPatrolPoint()
     {
         Vector3 randomDirection = Random.insideUnitSphere * _enemyController.DetectCircleRadius;
         randomDirection += _enemyController.transform.position;
         
         UnityEngine.AI.NavMeshHit hit;
         if (UnityEngine.AI.NavMesh.SamplePosition(randomDirection, out hit, 
                 _enemyController.DetectCircleRadius, UnityEngine.AI.NavMesh.AllAreas))
         {
             return hit.position;
         }
         else
         {
             return _enemyController.transform.position;      
         }
     }
}