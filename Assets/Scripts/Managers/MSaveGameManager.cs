using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utilities.ServiceLocator;

namespace Managers
{
    /**
     * MSaveGameManager is a generic save and load management system for saving and loading game data.
     * It uses the ServiceLocator to get all the ISaveGame services (need to specify the children interface)
     *  and calls their OnSaveData and OnLoadData methods.
     * 
     * The Save method will save the data from the implemented ISaveGame services.
     * They will be passed to this class as void pointers and saved using the Easy Save 3 plugin.
     */
    public class MSaveGameManager : PersistentSingleton<MSaveGameManager>
    {
        /**
         * Call the Save method for each ISaveGame service
         * Currently not implemented
         */
        public void SaveGame(string saveName)
        {
            // Save<ISavePlayer>(SaveName);
            // Save<ISaveLevel>(SaveName);
            //Save<ISaveInventory>(SaveName);
        }

        /**
         * Call the Load method for each ISaveGame service
         * Currently not implemented
         */
        public void LoadGame_Player(string saveName)
        {
            //LoadGame<ISavePlayer>(SaveName);
            //LoadGame<ISaveInventory>(SaveName);
        }

        /**
         * Call the Load method for each ISaveGame service
         * Currently not implemented
         */
        public void LoadGame_Level(string saveName)
        {
            //LoadGame<ISaveLevel>(SaveName);
        }
        
        /**
         * Load the game data for the specified type of ISaveGame service
         */
        public void LoadGame<T>(string saveName) where T : ISaveGame
        {
            // Check if the save data exists
            if (IsNewSave<T>(saveName))
            {
                //Debug.Log($"load default {SaveName}_{typeof(T).Name}");
                LoadDefaultData<T>();

            }
            // Load the save data
            else
            {
                //Debug.Log($"load Saved {SaveName}_{typeof(T).Name}");
                Load<T>(saveName);
            }
        }
        
        /**
         * Get the internal name for the save data
         */
        private string GetInternalName<T>(string saveName)
        {
            return $"{saveName}_{typeof(T).Name}";
        }

        /**
         * Check if the save data is new
         */
        private bool IsNewSave<T>(string saveName)
        {
            // Check if the key exists
            return !ES3.KeyExists(GetInternalName<T>(saveName));
        }

        /**
         * Load the default data for the specified type of ISaveGame service
         */
        private void LoadDefaultData<T>() where T : ISaveGame
        {
            Debug.Log("target " + (typeof(T)) + "has no initial data and was able to load, hence load default data from scriptableObject");
            var saveServices = GetSaveGameServices<T>();
            foreach (var o in saveServices)
            {
                o.LoadDefaultData();
            }
        }

        /**
         * Save the game data for the specified type of ISaveGame service
         */
        private void Save<T>(string saveName) where T : ISaveGame
        {
            //Create Index for Each Type of Data
            var saveServices = GetSaveGameServices<T>();
            var fileIndices = new List<string>();
        
            //for each SaveGame interface, save the data
            foreach (var service in saveServices)
            {
                var key = GenerateUniqueKeyForService(service); // Generate unique keys
                var data = service.OnSaveData();
                ES3.Save(key, data); // Save the data
                fileIndices.Add(key);
            }

            ES3.Save(GetInternalName<T>(saveName), fileIndices); // Save the index of all keys
        }
    
        /**
         * Load the game data for the specified type of ISaveGame service
         */
        private void Load<T>(string saveName) where T : ISaveGame
        {   
            // construct the type-specific save name
            // Example: SaveName_ISavePlayer
            string typeSpecificSaveName = $"{saveName}_{typeof(T).Name}";
        
            // Check if the save data exists
            if (!ES3.KeyExists(typeSpecificSaveName))
            {
                Debug.LogWarning($"No saved data found for {typeSpecificSaveName}");
                return;
            }
            
            // Load the save data
            var fileIndices = ES3.Load<List<string>>(typeSpecificSaveName);
            // Get the save game services
            var saveServices = GetSaveGameServices<T>();

            // Load the data for each service
            foreach (var key in fileIndices)
            {
                if (!ES3.KeyExists(key)) continue; // Consider specifying the file if not using the default
                var data = ES3.Load<object>(key); // Consider specifying a more specific type if possible
                var service = saveServices.FirstOrDefault(s => GenerateUniqueKeyForService(s) == key);
                if (service != null)
                {
                    service.OnLoadData(data);
                }
            }
        }
    
        
        private List<T> GetSaveGameServices<T>() where T : ISaveGame
        {
            return ServiceLocator.Instance.Get<T>();
        }

        private string GenerateUniqueKeyForService(ISaveGame service)
        {
            return GetDataTypeSaveName(service) + GetDataNameSaveName(service);
        }
    
        private string GetDataTypeSaveName(ISaveGame service)
        {
            // Example: Use service type name + unique identifier
            return service.GetType().Name; // Customize as needed
        }
    
        private string GetDataNameSaveName(ISaveGame service)
        {
            // Example: Use service type name + unique identifier
            return service.ToString(); // Customize as needed
        }

    }
}

