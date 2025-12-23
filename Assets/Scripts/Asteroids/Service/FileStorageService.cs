using System;
using System.IO;
using System.Linq;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using PG.Asteroids.Misc;
using PG.Asteroids.Models;
using PG.Asteroids.Models.DataModels;
using UnityEngine;
using Zenject;

namespace PG.Asteroids.Service
{
    public class FileStorageService : IDataService
    {
        // Needed these to have the data to Authorize. 
        // For Server state it will be authorized on Server.
        [Inject] private StaticDataModel _staticDataModel;

        private UserData _userData;
        private UserData UserData
        {
            get
            {
                if (_userData == null)
                {
                    if (!TryGetUserData(out _userData))
                    {
                        Debug.LogError("Something went wrong. Unable to get the UserData.");
                    }
                }

                return _userData;
            }
        }

        public async UniTask<MetaData> GetMetaData()
        {
            try
            {
                // TODO: MS: Encrypt the Data. For now saving plain to read and change.
                string path = Constants.MetaDataFile;
                if (File.Exists(path))
                {
                    using (var reader = new StreamReader(path))
                    {
                        MetaData metaData = JsonConvert.DeserializeObject<MetaData>(reader.ReadToEnd());
                        return (metaData);
                    }
                }
                else
                {
                    Debug.LogError("MetaData File not found.");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Something went wrong. Unable to get the MetaData.");
            }

            return null;
        }

        public async UniTask<UserData> SaveUserData(UserData userData)
        {
            try
            {
                // TODO: MS: Encrypt the Data. For now saving plain to read and change.
                using (var writer = new StreamWriter(Constants.GameStateFile))
                {
                    writer.Write(JsonConvert.SerializeObject(userData, Formatting.Indented));
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Something went wrong. Unable to write the UserData.");
            }

            return userData;
        }

        private bool TryGetUserData(out UserData userData)
        {
            // TODO: MS: Encrypt the Data. For now saving plain to read and change.
            string path = Constants.GameStateFile;

            if (File.Exists(path))
            {
                using (StreamReader reader = new StreamReader(path))
                {
                    userData = JsonConvert.DeserializeObject<UserData>(reader.ReadToEnd());
                    return true;
                }
            }

            userData = null;

            return false;
        }
        
        public async UniTask<UserData> GetUserData()
        {
            try
            {
                if (TryGetUserData(out UserData userData))
                {
                    return (userData);
                }
                else
                {
                    Debug.LogError("GameState File not found.");
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }

            return null;
        }
    }
}