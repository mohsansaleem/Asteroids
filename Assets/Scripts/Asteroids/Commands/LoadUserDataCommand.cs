using System;
using System.IO;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using PG.Asteroids.Contexts.Startup;
using PG.Asteroids.Utilities;
using PG.Asteroids.Models.DataModels;
using PG.Asteroids.Models.MediatorModels;
using PG.Asteroids.Models.RemoteDataModels;
using PG.Asteroids.Services;
using PG.Core.Commands;
using UnityEngine;
using Zenject;

namespace PG.Asteroids.Commands
{
    public class LoadUserDataCommand : BaseCommand<LoadUserDataParams>
    {
        [Inject] private RemoteDataModel _remoteDataModel;
        [Inject] private readonly StartupModel _startupModel;
        [Inject] private readonly IDataService _dataService;

        public override async UniTask Execute(LoadUserDataParams parameters)
        {
            try
            {
                UserData userData = await _dataService.GetUserData();
                _remoteDataModel.SeedUserData(userData);
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
                throw;
            }
        }
    }
}