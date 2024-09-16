using Enemy;

public interface IEnemyState
{
    void EnterState(EnemyController enemyController);
    void UpdateState(EnemyController enemyController);
}