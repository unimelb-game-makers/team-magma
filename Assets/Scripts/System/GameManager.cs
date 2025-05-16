// Author : Peiyu Wang @ Daphatus
// 10 03 2025 03 07

// Update: Lyu

using System.Collections.Generic;
using System.Collections;
using System.Level;
using Player;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utilities.ServiceLocator;

namespace System
{
    public class GameManager : PersistentSingleton<GameManager>
    {
        [SerializeField] private PlayerCharacter _playerCharacter;
        public PlayerCharacter PlayerCharacter
        {
            get
            {
                if (!_playerCharacter)
                {
                    throw new Exception("Add player manually.");
                }
                return _playerCharacter;
            }
        }

        private PlayerController _playerController;
        public PlayerController PlayerController
        {
            get
            {
                if (!_playerController)
                {
                    if (PlayerCharacter)
                    {
                        _playerController = PlayerCharacter.GetComponent<PlayerController>();
                    }
                }
                return _playerController;
            }
        }

        private BeatSpawner _beatSpawner;



        public BeatSpawner BeatSpawner
        {
            get
            {
                if (!_beatSpawner)
                {
                    _beatSpawner = FindObjectOfType<BeatSpawner>();
                    if (!_beatSpawner)
                    {
                        throw new Exception("No BeatSpawner found in the scene.");
                    }
                }
                return _beatSpawner;
            }
        }

        /// <summary>
        /// Add Levels here
        /// </summary>
        private List<string> levelNames = new List<string>()
        {
            "Room1",
            "Room2",
            "Room3",
            "Room4",
            "Room5",
            "Room6",
            "Room7",
            "Room8-9",
        };


        private int _currentLevelIndex = 0;

        protected override void Awake()
        {
            base.Awake();
        }

        private void Start()
        {
            //check if player data exists
            DontDestroyOnLoad(PlayerCharacter.gameObject);
            //if the player exist
            //load the current scene
            //load the player data
            //else
            //load the first scene
            //create a new player
        }

        public void LoadTutorial(string sceneName)
        {
            LoadingLevel();
            Debug.Log("Loading level: " + sceneName);
            LevelManager.Instance.LoadLevel(sceneName, LevelLoaded);
            PlayerCharacter.PlayerStats.OnReset();
        }

        public void LoadNextLevel()
        {
            if (_currentLevelIndex >= levelNames.Count)
            {
                // TODO: add success screen
                throw new Exception("No more levels to load.");
            }
            _currentLevelIndex++;
            LoadLevel(levelNames[_currentLevelIndex]);

        }

        public void LoadNewGame()
        {
            Scene currentScene = SceneManager.GetActiveScene();
            if (currentScene.name != levelNames[0])
            {
                LoadLevel(levelNames[0]);
                _currentLevelIndex = 0;
                PlayerCharacter.PlayerStats.OnReset();
            }
            else
            {
                ReloadLevel();
            }
        }

        public void LoadContinueGame()
        {
            PlayerCharacter.PlayerStats.OnReset();
            LoadLevel(levelNames[Math.Max(_currentLevelIndex, 0)]);
            _currentLevelIndex = Math.Max(_currentLevelIndex, 0);
        }

        private void LoadLevel(string sceneName)
        {
            LoadingLevel();
            Debug.Log("Loading level: " + sceneName);
            LevelManager.Instance.LoadLevel(sceneName, LevelLoaded);
        }

        public void LoadLevelNumber(int number)
        {
            Scene currentScene = SceneManager.GetActiveScene();
            if (currentScene.name != levelNames[number - 1])
            {
                LoadLevel(levelNames[number - 1]);
                _currentLevelIndex = number - 1;
                PlayerCharacter.PlayerStats.OnReset();
            }
            else
            {
                ReloadLevel();
            }
        }

        public void ReloadLevel()
        {
            LoadingLevel();
            LevelManager.Instance.ReloadCurrentLevel(LevelReloaded);
        }

        private void LoadingLevel()
        {
            Debug.Log("Level loaded.");
            PlayerCharacter.gameObject.SetActive(false);
            //Freeze all actions
        }
        private void LevelLoaded()
        {
            Debug.Log("Level loaded.");

            PlayerCharacter.transform.position = SubGameManager.Instance.LevelSpawnPoint.position;
            PlayerCharacter.transform.rotation = SubGameManager.Instance.LevelSpawnPoint.rotation;
            PlayerCharacter.gameObject.SetActive(true);

            var cameraComponent = PlayerCharacter.GetComponent<PlayerCamera>();
            cameraComponent.FindActiveCamera();
            cameraComponent.SetYaw(SubGameManager.Instance.LevelSpawnPoint.eulerAngles.y);
        }

        private void LevelReloaded()
        {
            Debug.Log("Level reloaded.");

            PlayerCharacter.transform.position = SubGameManager.Instance.LevelSpawnPoint.position;
            PlayerCharacter.transform.rotation = SubGameManager.Instance.LevelSpawnPoint.rotation;
            PlayerCharacter.gameObject.SetActive(true);

            var cameraComponent = PlayerCharacter.GetComponent<PlayerCamera>();
            cameraComponent.FindActiveCamera();
            cameraComponent.SetYaw(SubGameManager.Instance.LevelSpawnPoint.eulerAngles.y);

            PlayerCharacter.PlayerStats.OnReset();
        }

    }
}