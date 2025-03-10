using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using Utilities.ServiceLocator;

namespace System
{
    /// <summary>
    /// Manager that handles saving and loading for all services registered via the ServiceLocator,
    /// and persists the save data to disk.
    /// </summary>
    public class SaveSystem
    {
        // Internal storage for saved data. The key is the type of the service,
        // and the value is a list of data objects saved from each service of that type.
        private Dictionary<Type, List<object>> _savedData = new Dictionary<Type, List<object>>();

        // File path for persistent save data.
        private string _filePath;

        public string FilePath
        {
            get
            {
                if (string.IsNullOrEmpty(_filePath))
                {
                    _filePath = Application.persistentDataPath + "/savedata.dat";
                }
                return _filePath;
            }
        }

        /// <summary>
        /// Saves data from all services of type T registered with the ServiceLocator and persists it to disk.
        /// </summary>
        /// <typeparam name="T">A type that implements ISaveGame.</typeparam>
        public void Save<T>() where T : ISaveGame
        {
            List<T> services = ServiceLocator.Instance.Get<T>();
            if (services == null || services.Count == 0)
            {
                Debug.LogWarning($"No services found for type {typeof(T)} to save.");
                return;
            }

            // Call OnSaveData() on each service and store the returned data.
            List<object> dataList = new List<object>();
            foreach (T service in services)
            {
                object data = service.OnSaveData();
                dataList.Add(data);
            }
            _savedData[typeof(T)] = dataList;

            // Persist the collected data to disk.
            SaveAllToDisk();
        }

        /// <summary>
        /// Loads saved data from disk and then delivers it to all services of type T registered with the ServiceLocator.
        /// </summary>
        /// <typeparam name="T">A type that implements ISaveGame.</typeparam>
        public void Load<T>() where T : ISaveGame
        {
            // Reload saved data from disk.
            LoadAllFromDisk();

            List<T> services = ServiceLocator.Instance.Get<T>();
            if (services == null || services.Count == 0)
            {
                Debug.LogWarning($"No services found for type {typeof(T)} to load.");
                return;
            }

            if (!_savedData.TryGetValue(typeof(T), out List<object> dataList))
            {
                Debug.LogWarning($"No saved data available for type {typeof(T)}.");
                return;
            }

            // Deliver each saved data object to the matching service.
            // (This assumes that the order of services corresponds to the order of saved data.)
            for (int i = 0; i < services.Count && i < dataList.Count; i++)
            {
                services[i].OnLoadData(dataList[i]);
            }
        }

        /// <summary>
        /// Deletes saved data for a specific type T and persists the updated data to disk.
        /// </summary>
        /// <typeparam name="T">A type that implements ISaveGame.</typeparam>
        public void DeleteSave<T>() where T : ISaveGame
        {
            if (_savedData.ContainsKey(typeof(T)))
            {
                _savedData.Remove(typeof(T));
                SaveAllToDisk();
                Debug.Log($"Deleted save for type {typeof(T)}.");
            }
            else
            {
                Debug.LogWarning($"No saved data available for type {typeof(T)} to delete.");
            }
        }

        /// <summary>
        /// Deletes all saved data from memory and removes the save file from disk.
        /// </summary>
        public void DeleteAllSaves()
        {
            _savedData.Clear();
            if (File.Exists(FilePath))
            {
                File.Delete(FilePath);
                Debug.Log("Deleted all save data and removed the save file from disk.");
            }
            else
            {
                Debug.LogWarning("No save file found to delete.");
            }
        }

        /// <summary>
        /// Writes the internal saved data dictionary to disk.
        /// </summary>
        private void SaveAllToDisk()
        {
            try
            {
                using (FileStream stream = new FileStream(FilePath, FileMode.Create))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(stream, _savedData);
                }
                Debug.Log("Data saved to disk at: " + FilePath);
            }
            catch (Exception ex)
            {
                Debug.LogError("Failed to save data to disk: " + ex.Message);
            }
        }

        /// <summary>
        /// Loads the saved data from disk into the internal dictionary.
        /// </summary>
        private void LoadAllFromDisk()
        {
            if (!File.Exists(FilePath))
            {
                Debug.LogWarning("No save file found at: " + FilePath);
                return;
            }
            try
            {
                using (FileStream stream = new FileStream(FilePath, FileMode.Open))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    _savedData = (Dictionary<Type, List<object>>)formatter.Deserialize(stream);
                }
                Debug.Log("Data loaded from disk.");
            }
            catch (Exception ex)
            {
                Debug.LogError("Failed to load data from disk: " + ex.Message);
            }
        }
    }
}
