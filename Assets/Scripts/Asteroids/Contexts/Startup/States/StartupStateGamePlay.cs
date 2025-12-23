using System;
using Cysharp.Threading.Tasks;
using PG.Asteroids.Models.MediatorModels;
using PG.Asteroids.Views.Startup;
using PG.Asteroids.Misc;
using UniRx;
using Zenject;

namespace PG.Asteroids.Contexts.Startup
{
    public class StartupStateGamePlay : StartupState
    {
        public override UniTask Enter()
        {
            return base.Enter();
            View.Hide();

            /*Observable.Timer(TimeSpan.FromSeconds(Constants.SaveGameDelay)).Repeat()
                .Subscribe((interval) => SignalBus.Fire<SaveUserDataSignal>()).AddTo(Disposables);*/
        }

        public override UniTask Exit()
        {
            return base.Exit();
        }
    }
}