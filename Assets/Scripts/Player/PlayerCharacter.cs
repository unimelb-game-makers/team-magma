using System;
using Player.Stats;
using Unity.Collections;
using UnityEngine;
using Utilities.ServiceLocator;

namespace Player
{
    public class PlayerCharacter : MonoBehaviour, ISavePlayer
    {
        private PlayerStats _playerStats; public PlayerStats PlayerStats => _playerStats;
        private void Awake()
        {
            _playerStats = GetComponent<PlayerStats>();
            ServiceLocator.Instance.Register<ISavePlayer>(this);
        }

        private void OnDisable()
        {
            if(!Application.isPlaying)
            {
                ServiceLocator.Instance.Unregister<ISavePlayer>(this);
            }
        }

        public object OnSaveData()
        {
            Debug.Log("Saving player data Position: " + transform.position);
            return new PlayerData(
                _playerStats.HealthStat.CurrentValue,
                0,
                transform.position,
                transform.rotation
            );
        }

        public void OnLoadData(object data)
        {
            var playerData = (PlayerData) data;
            if (playerData != null)
            {
                _playerStats.HealthStat.CurrentValue = playerData.health;
                transform.position = playerData.position;
                transform.rotation = playerData.rotation;
                
                Debug.Log("Position loaded: " + playerData.position);
            }
            else
            {
                //do nothing
                LoadDefaultData();
            }
        }

        public void LoadDefaultData()
        {
            //should remove this as there will always be a default data
        }
    }
    
    [Serializable]
    public struct SerializableVector3
    {
        public float x, y, z;

        public SerializableVector3(float rX, float rY, float rZ)
        {
            x = rX;
            y = rY;
            z = rZ;
        }

        public override string ToString() => $"({x}, {y}, {z})";

        // Implicit conversion from SerializableVector3 to Vector3
        public static implicit operator Vector3(SerializableVector3 sVector)
        {
            return new Vector3(sVector.x, sVector.y, sVector.z);
        }

        // Implicit conversion from Vector3 to SerializableVector3
        public static implicit operator SerializableVector3(Vector3 vector)
        {
            return new SerializableVector3(vector.x, vector.y, vector.z);
        }
    }

    [Serializable]
    public struct SerializableQuaternion
    {
        public float x, y, z, w;

        public SerializableQuaternion(float rX, float rY, float rZ, float rW)
        {
            x = rX;
            y = rY;
            z = rZ;
            w = rW;
        }

        public override string ToString() => $"({x}, {y}, {z}, {w})";

        // Implicit conversion from SerializableQuaternion to Quaternion
        public static implicit operator Quaternion(SerializableQuaternion sQuat)
        {
            return new Quaternion(sQuat.x, sQuat.y, sQuat.z, sQuat.w);
        }

        // Implicit conversion from Quaternion to SerializableQuaternion
        public static implicit operator SerializableQuaternion(Quaternion quat)
        {
            return new SerializableQuaternion(quat.x, quat.y, quat.z, quat.w);
        }
    }

    [Serializable]
    public class PlayerData
    {
        public float health;
        public float battery;
        public SerializableVector3 position;
        public SerializableQuaternion rotation;
        
        public PlayerData(float health, float battery, Vector3 position, Quaternion rotation)
        {
            this.health = health;
            this.battery = battery;
            // Implicitly converts Vector3 and Quaternion to our serializable versions.
            this.position = position;
            this.rotation = rotation;
        }
    }
}