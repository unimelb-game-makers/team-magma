using System;
using UnityEngine;

namespace Player
{
    /// <summary>
    /// Keeping base stats as a struct on the scriptable keeps it flexible and easily editable.
    /// We can pass this struct to the spawned prefab unit and alter them depending on conditions.
    /// </summary>
    [CreateAssetMenu(fileName = "GameData", menuName = "GameData/PlayerData", order = 1)]
    [Serializable]
    public class PlayerStats : ScriptableObject
    {
        public float health;
        public float healthMax;
        public float batteryLevel;
        public float batteryMax;
    }
}