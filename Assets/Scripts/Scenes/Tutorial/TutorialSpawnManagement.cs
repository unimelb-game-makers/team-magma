using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialSpawnManagement : MonoBehaviour
{
    public GameObject dummyEnemyInstance;
    public GameObject dummyEnemyPrefab; 
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (dummyEnemyInstance == null){
            dummyEnemyInstance = Instantiate(dummyEnemyPrefab,transform.position, transform.rotation);
        }
    }
}
