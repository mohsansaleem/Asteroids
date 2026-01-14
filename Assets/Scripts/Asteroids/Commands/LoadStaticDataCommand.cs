using System;
using System.IO;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using PG.Asteroids.Contexts.Startup;
using PG.Asteroids.Utilities;
using PG.Asteroids.Models;
using PG.Asteroids.Models.DataModels;
using PG.Asteroids.Services;
using PG.Core.Commands;
using UnityEngine;
using Zenject;

namespace PG.Asteroids.Commands
{
    public class LoadStaticDataCommand : BaseCommand<LoadStaticDataParams>
    {
        [Inject] private readonly IDataService _dataService;
        [Inject] private readonly StaticDataModel _staticDataModel;

        public override async UniTask Execute(LoadStaticDataParams parameters)
        {
            try
            {
                MetaData metaData = await _dataService.GetMetaData();
                _staticDataModel.SeedMetaData(metaData);
            }
            catch(Exception ex)
            {
                Debug.LogError(ex.Message);
                throw;
            }
        }
    }
}