using UnityEngine;

public class EnemyStateAttack : IEnemyState
{
    private EnemyController _enemyController;
    
    public void Enter(EnemyController enemyController)
    {
        _enemyController = enemyController;
        _enemyController.EnemyAnimator.SetTrigger("Attack");
    }

    public void Update()
    {
        
    }

    public void Exit()
    {
        _enemyController = null;
    }
}