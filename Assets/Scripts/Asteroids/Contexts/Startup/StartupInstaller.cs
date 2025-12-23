using PG.Core.Contexts.StateManagement;
using PG.Asteroids.Commands;
using PG.Asteroids.Models.MediatorModels;
using PG.Asteroids.Views.Startup;
using UnityEngine;
using Zenject;

namespace PG.Asteroids.Contexts.Startup
{
    public class StartupInstaller : MonoInstaller
    {
        [SerializeField]
        public StartupView StartupView;

        public override void InstallBindings()
        {
            BindSignals();

            Container.Bind<StartupModel>().AsSingle();

            Container.BindInstance(StartupView);
            
            MediatorStateMachineInstaller.Install(Container);
            Container.BindInterfacesTo<StartupMediator>().AsSingle();
        }

        private void BindSignals()
        {
            Container.DeclareSignal<LoadStaticDataSignal>();
            Container.BindSignal<LoadStaticDataSignal>()
                .ToMethod<LoadStaticDataCommand>((x) => x.Execute)
                .FromNew();

            Container.DeclareSignal<LoadUserDataSignal>();
            Container.BindSignal<LoadUserDataSignal>()
                .ToMethod<LoadUserDataCommand>((x) => x.Execute)
                .FromNew();

            Container.DeclareSignal<SaveUserDataSignal>();
            Container.BindSignal<SaveUserDataSignal>()
                .ToMethod<SaveUserDataCommand>((x) => x.Execute)
                .FromNew();

            Container.DeclareSignal<CreateUserDataSignal>();
            Container.BindSignal<CreateUserDataSignal>()
                .ToMethod<CreateUserDataCommand>((x) => x.Execute)
                .FromNew();
        }
    }
}
