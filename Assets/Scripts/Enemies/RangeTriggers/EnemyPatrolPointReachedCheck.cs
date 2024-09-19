using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy;
using Unity.VisualScripting;

public class EnemyPatrolPointReachedCheck: MonoBehaviour
{
    private EnemyController enemyController;

    private void Awake() {
        enemyController = GetComponentInParent<EnemyController>();
    }

    private void OnTriggerEnter(Collider collider) {
        if (collider.gameObject == enemyController.GetCurrentPatrolPoint().gameObject) {
            enemyController.SetHasReachedPatrolPointBool(true);
        }
    }
}
