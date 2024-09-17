using System.Collections;
using System.Collections.Generic;
using Enemy;
using UnityEngine;

public class EnemyAggroRangeCheck : MonoBehaviour
{
    private EnemyController enemyController;
    [SerializeField] private string playerTag = "Player";

    private void Awake() {
        enemyController = GetComponentInParent<EnemyController>();
    }

    private void OnTriggerEnter(Collider collider) {
        if (collider.gameObject.CompareTag(playerTag)) {
            enemyController.SetAggroRangeBool(true);
        }
    }

    private void OnTriggerExit(Collider collider) {
        if (collider.gameObject.CompareTag(playerTag)) {
            enemyController.SetAggroRangeBool(false);
        }
    }
}
