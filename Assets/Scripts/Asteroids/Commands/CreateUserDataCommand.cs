using System;
using System.IO;
using Newtonsoft.Json;
using PG.Asteroids.Contexts.Startup;
using PG.Asteroids.Misc;
using PG.Asteroids.Models.MediatorModels;
using PG.Core.Commands;
using UnityEngine;
using Zenject;

namespace PG.Asteroids.Commands
{
    public class CreateUserDataCommand : BaseCommand
    {
        [Inject] private readonly StartupModel _startupModel;

        public void Execute(CreateUserDataSignal commandParams)
        {
            try
            {
                string path = Path.Combine(Application.streamingAssetsPath, Constants.GameStateFile);

                StreamWriter writer = new StreamWriter(path);
                writer.Write(JsonConvert.SerializeObject(commandParams.UserData, Formatting.Indented));
                writer.Flush();
                writer.Close();
                
                PostExecute();
            }
            catch(Exception ex)
            {
                Debug.LogError(ex.Message);
            }
        }
    }

}
