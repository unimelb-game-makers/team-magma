// Author : Peiyu Wang @ Daphatus
// 10 03 2025 03 07

using System.Collections.Generic;
using System.Level;
using Player;
using UnityEngine;

namespace System
{
    public class GameManager : PersistentSingleton<GameManager>
    { 
        private PlayerCharacter _playerCharacter;
        public PlayerCharacter PlayerCharacter
        {
            get
            {
                if (_playerCharacter == null)
                {
                    _playerCharacter = FindObjectOfType<PlayerCharacter>();
                    if (_playerCharacter == null)
                    {
                        throw new Exception("No PlayerCharacter found in the scene.");
                    }
                }
                return _playerCharacter;
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


        [SerializeField] private string defaultScenename = "SampleScene";
        
        /// <summary>
        /// Add Levels here
        /// </summary>
        private List<string> levelNames = new List<string>()
        {
            "Level1Rooms1-3_DEMO",
            "Level1Rooms4-6_DEMO",
            "Level1Rooms7-9_DEMO",
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
        
        public void LoadNextLevel()
        {
            if(_currentLevelIndex >= levelNames.Count)
            {
                throw new Exception("No more levels to load.");
            }
            LoadLevel(levelNames[_currentLevelIndex]);
            _currentLevelIndex++;
        }
        
        
        
        private void LoadLevel(string sceneName)
        {
            LoadingLevel();
            LevelManager.Instance.LoadLevel(sceneName, LevelLoaded);
        }
        
        public void ReloadLevel()
        {
            LoadingLevel();
            LevelManager.Instance.ReloadCurrentLevel(LevelLoaded);
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
            PlayerCharacter.gameObject.SetActive(true);
            //Load player data
        }

    }
}