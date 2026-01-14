using Cysharp.Threading.Tasks;
using PG.Asteroids.Models.MediatorModels;
using PG.Asteroids.Views.Startup;
using PG.Core.Commands;
using PG.Core.Installers;
using Zenject;

namespace PG.Asteroids.Contexts.Startup
{
    public class StartupStateLoadGamePlay : StartupState
    {
        [Inject] private LoadSceneCommand _loadSceneCommand;

        public override async UniTask Enter()
        {
            await base.Enter();

            LoadGameplayAsync().Forget();
        }
        
        private async UniTaskVoid LoadGameplayAsync()
        {
            await _loadSceneCommand.Execute(new LoadSceneParams()
            {
                Scene = ProjectScenes.Game
            });

            StartupModel.LoadingProgress = 100;
        }
    }
}