using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountChildrenEnemies : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (transform.childCount == 0) {
            GetComponent<TriggerInstructionAndSpawnEnemy>().ShowInstructionScreen();
            Destroy(gameObject);
        }
    }
}
