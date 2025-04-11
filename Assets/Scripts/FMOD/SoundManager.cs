//--------------------------------------------------------------------
//
// This is a Unity behaviour script that demonstrates how to use
// the FMOD Studio API in your game code. This can be more effective
// than just using the FMOD for Unity components, and provides access
// to the full capabilities of FMOD Studio.
//
//--------------------------------------------------------------------
// see full: https://fmod.com/docs/2.03/unity/examples-basic.html

using System.Collections;
using System.Collections.Generic;
using Enemy;
using Player;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    public void Awake()
    {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }

        SceneManager.sceneLoaded += OnSceneLoaded; // Subscribe to the event
    }

    void OnDestroy() {
        SceneManager.sceneLoaded -= OnSceneLoaded; // Unsubscribe to prevent memory leaks
    }

    // Runs once when a scene is loaded.
    void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        SetGameObjectsSFXVolume();
    }

    // Set the volume of all game objects that use SFX sounds.
    public void SetGameObjectsSFXVolume() {
        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1.0f);  // Default to 1 (100%) if not set
        float masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1.0f);  // Default to 1 (100%) if not set
        SetEnemiesSFXVolume(masterVolume, sfxVolume);
    }

    // Set the volume of enemies SFX.
    public void SetEnemiesSFXVolume(float masterVolume, float sfxVolume) {
        EnemyController[] enemyControllers = FindObjectsOfType<EnemyController>();
        foreach (EnemyController enemyController in enemyControllers) {
            Debug.Log("The enemy's sound was changed!");
            enemyController.SetAudioVolume(masterVolume, sfxVolume);
        }
    }
}
