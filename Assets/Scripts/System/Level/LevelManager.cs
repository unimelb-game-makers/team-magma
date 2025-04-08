// Author : Peiyu Wang @ Daphatus
// 23 03 2025 03 35

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace System.Level
{
    /// <summary>
    /// A LevelManager that extends a PersistentSingleton, ensuring there's only one
    /// instance at all times and it is preserved across scene loads.
    /// It provides a state machine to notify the level loading sequence (Loading Level:, Level Loaded),
    /// unloads the original scene, and triggers a level loaded event when complete.
    /// </summary>
    public class LevelManager : PersistentSingleton<LevelManager>
    {
        /// <summary>
        /// Gets the current state of the level loading process.
        /// </summary>
        public string SceneName { get; private set; }


        /// <summary>
        /// Asynchronously loads a new scene by name.
        /// The process fires loading events, unloads the original scene, and sets the new scene as active.
        /// Executes an optional callback upon completion.
        /// </summary>
        /// <param name="sceneName">Name of the scene to load.</param>
        /// <param name="onComplete">Callback executed after the scene finishes loading.</param>
        internal void LoadLevel(string sceneName, Action onComplete)
        {
            StartCoroutine(LoadLevelAsyncRoutine(sceneName, onComplete));
        }

        private IEnumerator LoadLevelAsyncRoutine(string sceneName, Action onComplete)
        {
            // Load the new scene additively.
            AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            while (loadOperation is { isDone: false })
            {
                // Optionally, update a UI progress bar here using loadOperation.progress.
                yield return null;
            }

            // Get the currently active scene to later unload it.
            Scene currentScene = SceneManager.GetActiveScene();

            // Set the newly loaded scene as the active scene.
            Scene newScene = SceneManager.GetSceneByName(sceneName);
            if (newScene.IsValid())
            {
                SceneManager.SetActiveScene(newScene);
            }
            else
            {
                Debug.LogError("Loaded scene is not valid: " + sceneName);
            }

            // Unload the original scene if it is different from the new scene.
            if (currentScene.name != sceneName)
            {
                AsyncOperation unloadOperation = SceneManager.UnloadSceneAsync(currentScene);
                while (unloadOperation is { isDone: false })
                {
                    yield return null;
                }
            }

            // Update state, notify that the level has loaded, and invoke the callback.
            onComplete?.Invoke();
        }

        /// <summary>
        /// Reloads the current active scene asynchronously.
        /// </summary>
        /// <param name="onComplete">Callback executed after the scene finishes reloading.</param>
        public void ReloadCurrentLevel(Action onComplete)
        {
            StartCoroutine(ReloadCurrentLevelAsyncRoutine(onComplete));
        }

        private IEnumerator ReloadCurrentLevelAsyncRoutine(Action onComplete)
        {
            var activeScene = SceneManager.GetActiveScene();
            var sceneName = activeScene.name;
            Debug.Log("Reloading scene: " + sceneName);
            
            // Load the scene asynchronously
            AsyncOperation loadOperation = SceneManager.LoadSceneAsync(activeScene.buildIndex);
            while (loadOperation is { isDone: false })
            {
                // Optionally, update a UI progress bar here using loadOperation.progress
                yield return null;
            }
            
            // Update state and invoke the callback
            onComplete?.Invoke();
        }
        
    }
}
