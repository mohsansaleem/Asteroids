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
        [Inject] private MediatorStateMachine _mediatorStateMachine;
        [Inject] private LoadStaticDataCommand _loadStaticDataCommand;

        public override async UniTask Enter()
        {
            await base.Enter();

            ExecuteCommandAsync().Forget();
        }

        private async UniTaskVoid ExecuteCommandAsync()
        {
            await _loadStaticDataCommand.Execute(new LoadStaticDataParams());

            StartupModel.LoadingProgress = 50;
            await _mediatorStateMachine.Enter<StartupStateLoadAssets>();
        }
    }
}