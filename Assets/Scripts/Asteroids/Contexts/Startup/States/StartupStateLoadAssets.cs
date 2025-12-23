using System.Runtime.InteropServices;
using PG.Core.Contexts.StateManagement;
using Cysharp.Threading.Tasks;
using PG.Asteroids.Models.MediatorModels;
using PG.Asteroids.Views.Startup;
using PG.Core;
using PG.Core.Installers;
using Zenject;

namespace PG.Asteroids.Contexts.Startup
{
    public class StartupStateLoadAssets : StartupState
    {
        [Inject] private IAssetsLoader _assetsLoader;
        [Inject]
        private MediatorStateMachine _mediatorStateMachine;
        
        public override async UniTask Enter()
        {
            await base.Enter();

            await _assetsLoader.Initialize();
            StartupModel.LoadingProgress.Value = 100;
            _mediatorStateMachine.Enter<StartupStateLoadGamePlay>();
        }
    }
}