using UnityEngine;
using UnityEngine.AI;

public class EnemyStatePatrol : IEnemyState
{
    private EnemyController _enemyController;
    
    public void Enter(EnemyController enemyController)
    {
        _enemyController = enemyController;
        
        _enemyController.EnemyAnimator.SetBool("Patrol", true);
    }

    public void Update()
    {
        
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

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, 
                _enemyController.DetectCircleRadius, NavMesh.AllAreas))
        {
            return hit.position;
        }
        else
        {
            return _enemyController.transform.position;            
        }
    }
}