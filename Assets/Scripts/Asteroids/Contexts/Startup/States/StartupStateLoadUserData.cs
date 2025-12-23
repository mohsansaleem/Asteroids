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
        public override async UniTask Enter()
        {
            await base.Enter();

            SignalBus.Subscribe<CommandExecutedSignal>(OnCommandExecuted);
            LoadUserDataSignal signal = new LoadUserDataSignal();
            SignalBus.Fire(signal);
        }

        private void OnCommandExecuted(CommandExecutedSignal signal)
        {
            if (signal.CommandType == typeof(LoadUserDataCommand))
            {
                StartupModel.LoadingProgress.Value = 40;
            }
        }
    }
}