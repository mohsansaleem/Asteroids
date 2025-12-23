using System.Runtime.InteropServices;
using PG.Core.Contexts.StateManagement;
using Cysharp.Threading.Tasks;
using PG.Asteroids.Commands;
using PG.Asteroids.Models.MediatorModels;
using PG.Asteroids.Views.Startup;
using PG.Core.Installers;
using UnityEngine;
using Zenject;

namespace PG.Asteroids.Contexts.Startup
{
    public class StartupStateLoadStaticData : StartupState
    {
        [Inject]
        private MediatorStateMachine _mediatorStateMachine;

        public override async UniTask Enter()
        {
            await base.Enter();

            SignalBus.Subscribe<CommandExecutedSignal>(OnCommandExecuted);
            
            LoadStaticDataSignal signal = new LoadStaticDataSignal();
            SignalBus.Fire(signal);
        }

        private void OnCommandExecuted(CommandExecutedSignal signal)
        {
            if (signal.CommandType == typeof(LoadStaticDataCommand))
            {
                StartupModel.LoadingProgress.Value = 50;
                _mediatorStateMachine.Enter<StartupStateLoadAssets>().Forget();
            }
        }
    }
}