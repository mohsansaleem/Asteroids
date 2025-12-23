using System;
using System.IO;
using Newtonsoft.Json;
using PG.Asteroids.Misc;
using PG.Asteroids.Models.RemoteDataModels;
using PG.Asteroids.Service;
using PG.Core.Commands;
using UnityEngine;
using Zenject;

namespace PG.Asteroids.Commands
{
    public class SaveUserDataCommand : BaseCommand
    {
        [Inject] private RemoteDataModel _remoteDataModel;
        [Inject] private IDataService _dataService;

        public void Execute()
        {
            try
            {
                _dataService.SaveUserData(_remoteDataModel.UserData);
                PostExecute();
            }
            catch(Exception ex)
            {
                Debug.LogError("Error while Saving User: "+ ex.ToString());
            }
        }
    }

}
