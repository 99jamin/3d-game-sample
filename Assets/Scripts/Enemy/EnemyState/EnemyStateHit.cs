using UnityEngine;

public class EnemyStateHit : IEnemyState
{
    private EnemyController _enemyController;
    private PlayerController _attacker;
    private Vector3 _knockbackTargetPosition;
    
    public void Enter(EnemyController enemyController)
    {
        _enemyController = enemyController;
        _enemyController.EnemyAnimator.SetTrigger("Hit");

        Vector3 knockbackForce = (_enemyController.transform.position - _attacker.transform.position).normalized;
        _knockbackTargetPosition = enemyController.transform.position + knockbackForce;
    }

    public void Update()
    {
        if (_attacker != null)
        {
            _enemyController.transform.position = 
                Vector3.Lerp(_enemyController.transform.position, 
                    _knockbackTargetPosition, Time.deltaTime * 5f);
        }
    }

    public void Exit()
    {
        _enemyController = null;
    }

    public void SetAttacker(PlayerController attacker)
    {
        _attacker = attacker;
    }
}