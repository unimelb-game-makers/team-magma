using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy;
using UnityEngine.AI;
using Player;
using Enemies.EnemyTypes;

public class AttackState : BaseEnemyState
{
    protected float outsideAttackRangeTime;
    protected float outsideAttackRangeDuration = 0.5f;
    
    public AttackState(EnemyController enemyController, NavMeshAgent navMeshAgent, PlayerController playerController) : 
                        base(enemyController, navMeshAgent, playerController) { }

    public override void EnterState() {
        // Debug.Log("Entering Attack State");
        
        if(enemyController.GetAnimator()){
            enemyController.GetAnimator().SetBool("AttackIdle", true);
        }
        navMeshAgent.SetDestination(enemyController.transform.position);
        outsideAttackRangeDuration = enemyController.GetOutsideAttackRangeDuration();
        outsideAttackRangeTime = outsideAttackRangeDuration;
    }

    public override void UpdateState() {
        if (playerController != null)
            enemyController.transform.LookAt(playerController.transform.position);

        // If the enemy is currently attacking/knockbacked, wait for the
        // attack to finish first.
        if (enemyController.IsAttacking()) return;
        // If the player was killed, transition to patrol state.
        else if (!playerController) {
            enemyController.TransitionToState(EnemyState.Patrol);
        }
        // If the player is too close to the enemy, the enemy must flee.
        // This is only if the enemy is a ranged enemy.
        else if (enemyController is RangedEnemyController rController && rController.EnemyIsInFleeRange()) {
            enemyController.TransitionToState(EnemyState.Flee);
        }
        // If the player has exited attack range, transition to chase state
        // after some time.
        else if (!enemyController.PlayerIsInAttackRange()) {
            outsideAttackRangeTime -= Time.deltaTime;
            if (outsideAttackRangeTime <= 0) {
                enemyController.TransitionToState(EnemyState.Chase);
            }
        }
        // If all the above conditions are false, then attack the player
        else {
            enemyController.Attack();
        }
    }

    public override void ExitState() {
        enemyController.GetAttackSound().stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        if(enemyController.GetAnimator()){
            enemyController.GetAnimator().SetBool("AttackIdle", false);
        }    
    }
}
