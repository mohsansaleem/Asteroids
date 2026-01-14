using System;
using System.IO;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using PG.Asteroids.Contexts.Startup;
using PG.Asteroids.Utilities;
using PG.Asteroids.Models.RemoteDataModels;
using PG.Asteroids.Services;
using PG.Core.Commands;
using UnityEngine;
using Zenject;

namespace PG.Asteroids.Commands
{
    public class SaveUserDataCommand : BaseCommand<SaveUserDataParams>
    {
        [Inject] private RemoteDataModel _remoteDataModel;
        [Inject] private IDataService _dataService;

        public override UniTask Execute(SaveUserDataParams parameters)
        {
            try
            {
                _dataService.SaveUserData(_remoteDataModel.UserData);
                return UniTask.CompletedTask;
            }
            catch(Exception ex)
            {
                Debug.LogError("Error while Saving User: "+ ex.ToString());
                throw;
            }
        }
    }

}
