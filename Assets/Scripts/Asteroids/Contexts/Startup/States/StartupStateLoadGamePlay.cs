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
        public override async UniTask Enter()
        {
            await base.Enter();
            
            SignalBus.Subscribe<CommandExecutedSignal>(OnCommandExecuted);
            SignalBus.Fire(new LoadSceneSignal()
            {
                Scene = ProjectScenes.Game
            });
        }

        private void OnCommandExecuted(CommandExecutedSignal signal)
        {
            if (signal.CommandType == typeof(LoadSceneCommand))
            {
                StartupModel.LoadingProgress.Value = 100;
            }
        }
    }
}