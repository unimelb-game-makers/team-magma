using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy;

public class EnemyAttackRangeCheck : MonoBehaviour
{
    private EnemyController enemyController;
    [SerializeField] private string playerTag = "Player";

    private void Awake() {
        enemyController = GetComponentInParent<EnemyController>();
    }

    private void OnTriggerEnter(Collider collider) {
        if (collider.gameObject.CompareTag(playerTag)) {
            enemyController.SetAttackRangeBool(true);
        }
    }

    private void OnTriggerExit(Collider collider) {
        if (collider.gameObject.CompareTag(playerTag)) {
            enemyController.SetAttackRangeBool(false);
        }
    }
}
