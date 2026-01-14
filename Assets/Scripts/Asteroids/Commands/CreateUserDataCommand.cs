using System;
using System.IO;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using PG.Asteroids.Contexts.Startup;
using PG.Asteroids.Utilities;
using PG.Asteroids.Models.MediatorModels;
using PG.Core.Commands;
using UnityEngine;
using Zenject;

namespace PG.Asteroids.Commands
{
    public class CreateUserDataCommand : BaseCommand<CreateUserDataParams>
    {
        [Inject] private readonly StartupModel _startupModel;

        public override UniTask Execute(CreateUserDataParams parameters)
        {
            try
            {
                string path = Path.Combine(Application.streamingAssetsPath, Constants.GameStateFile);

                StreamWriter writer = new StreamWriter(path);
                writer.Write(JsonConvert.SerializeObject(parameters.UserData, Formatting.Indented));
                writer.Flush();
                writer.Close();

                return UniTask.CompletedTask;
            }
            catch(Exception ex)
            {
                Debug.LogError(ex.Message);
                throw;
            }
        }
    }

}
