using System;
using System.IO;
using Newtonsoft.Json;
using PG.Asteroids.Contexts.Startup;
using PG.Asteroids.Misc;
using PG.Asteroids.Models.DataModels;
using PG.Asteroids.Models.MediatorModels;
using PG.Asteroids.Models.RemoteDataModels;
using PG.Asteroids.Service;
using PG.Core.Commands;
using UnityEngine;
using Zenject;

namespace PG.Asteroids.Commands
{
    public class LoadUserDataCommand : BaseCommand
    {
        [Inject] private RemoteDataModel _remoteDataModel;
        [Inject] private readonly StartupModel _startupModel;
        [Inject] private readonly IDataService _dataService;

        public async void Execute(LoadUserDataSignal signal)
        {
            try
            {
                UserData userData = await _dataService.GetUserData();
                _remoteDataModel.SeedUserData(userData);
                
                PostExecute();
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
            }
        }
    }
}