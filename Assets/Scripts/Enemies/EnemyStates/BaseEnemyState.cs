using Enemy;
using Player;
using UnityEngine.AI;

public abstract class BaseEnemyState
{
    protected EnemyController enemyController;
    protected NavMeshAgent navMeshAgent;
    protected PlayerController playerController;

    public BaseEnemyState(EnemyController enemyController, NavMeshAgent navMeshAgent, PlayerController playerController) {
        this.enemyController = enemyController;
        this.navMeshAgent = navMeshAgent;
        this.playerController = playerController;
    }

    public abstract void EnterState();
    public abstract void UpdateState();
    public abstract void ExitState();
}