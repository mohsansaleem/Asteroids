using PG.Core.Contexts.StateManagement;
using PG.Core.Commands;
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
            Container.Bind<StartupModel>().AsSingle();

            Container.BindInstance(StartupView);

            // Bind commands as Transient - created when needed, GC'd after use
            Container.Bind<LoadStaticDataCommand>().AsTransient();
            Container.Bind<LoadUserDataCommand>().AsTransient();
            Container.Bind<SaveUserDataCommand>().AsTransient();
            Container.Bind<CreateUserDataCommand>().AsTransient();
            Container.Bind<LoadSceneCommand>().AsTransient();

            MediatorStateMachineInstaller.Install(Container);
            Container.BindInterfacesTo<StartupMediator>().AsSingle();
        }
    }
}
