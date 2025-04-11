using UnityEngine;

public class EnemyStateDead : IEnemyState
{
    private EnemyController _enemyController;
    
    public void Enter(EnemyController enemyController)
    {
        _enemyController = enemyController;
        _enemyController.EnemyAnimator.SetTrigger("Dead");
    }

    public void Update()
    {
        
    }

    public void Exit()
    {
        _enemyController = null;
    }
}