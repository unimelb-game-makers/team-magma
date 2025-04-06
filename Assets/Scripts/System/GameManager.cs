// Author : Peiyu Wang @ Daphatus
// 10 03 2025 03 07

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
        
    }
}