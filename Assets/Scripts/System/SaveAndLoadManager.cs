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

        [ContextMenu("Save Data")]
        public void SaveGame()
        {
            SavePlayer();
        }

        private void SavePlayer()
        {
            _saveSystem.Save<ISavePlayer>();
        }
        
        [ContextMenu("Load Data")]
        public void LoadGame()
        {
            LoadPlayer();
        }
        
        private void LoadPlayer()
        {
            _saveSystem.Load<ISavePlayer>();
        }
    }
}