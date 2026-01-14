using Cysharp.Threading.Tasks;
using PG.Asteroids.Commands;
using PG.Asteroids.Models.MediatorModels;
using PG.Asteroids.Views.Startup;
using PG.Core.Installers;
using Zenject;

namespace PG.Asteroids.Contexts.Startup
{
    public class StartupStateLoadUserData : StartupState
    {
        [Inject] private LoadUserDataCommand _loadUserDataCommand;

        public override async UniTask Enter()
        {
            await base.Enter();

            ExecuteCommandAsync().Forget();
        }
        
        private async UniTaskVoid ExecuteCommandAsync()
        {
            await _loadUserDataCommand.Execute(new LoadUserDataParams());

            StartupModel.LoadingProgress = 40;
        }
    }
}