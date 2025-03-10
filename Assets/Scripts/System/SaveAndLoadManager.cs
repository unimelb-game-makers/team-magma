// Author : Peiyu Wang @ Daphatus
// 10 03 2025 03 51

using UnityEngine;
using Utilities.ServiceLocator;

namespace System
{
    public class SaveAndLoadManager : PersistentSingleton<SaveAndLoadManager>
    {
        
        private SaveSystem _saveSystem;


        public void Start()
        {
            _saveSystem = new SaveSystem();
        }

        
        /// <summary>
        /// Currently, can be called from the editor
        /// Add more data to the saved
        /// </summary>
        [ContextMenu("Save Data")]
        public void SaveGame()
        {
            SavePlayer();
        }
        
        private void SavePlayer()
        {
            _saveSystem.Save<ISavePlayer>();
        }
        
        /// <summary>
        /// Currently, can be called from the editor
        /// </summary>
        [ContextMenu("Load Data")]
        public void LoadGame()
        {
            LoadPlayer();
        }
        
        /// <summary>
        /// Currently, can be called from the editor
        /// Add more data to the loaded
        /// </summary>
        private void LoadPlayer()
        {
            _saveSystem.Load<ISavePlayer>();
        }
    }
}