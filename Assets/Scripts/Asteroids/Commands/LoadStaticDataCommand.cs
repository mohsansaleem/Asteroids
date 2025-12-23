using System;
using System.IO;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using PG.Asteroids.Contexts.Startup;
using PG.Asteroids.Misc;
using PG.Asteroids.Models;
using PG.Asteroids.Models.DataModels;
using PG.Asteroids.Service;
using PG.Core.Commands;
using UnityEngine;
using Zenject;

namespace PG.Asteroids.Commands
{
    public class LoadStaticDataCommand : BaseCommand
    {
        [Inject] private readonly IDataService _dataService;
        [Inject] private readonly StaticDataModel _staticDataModel;

        public async void Execute(LoadStaticDataSignal signal)
        {
            await LoadMetaJson(Constants.MetaDataFile);
            
            PostExecute();
        }
        
        private async UniTask LoadMetaJson(string metaFileName)
        {
            try
            {
                MetaData metaData = await _dataService.GetMetaData();
                _staticDataModel.SeedMetaData(metaData);
            }
            catch(Exception ex)
            {
                Debug.LogError(ex.Message);
            }
        }
    }
}