using Zenject;

namespace PG.Core.Contexts.StateManagement
{
    public class MediatorStateMachineInstaller : Installer<MediatorStateMachineInstaller>
    {
        public override void InstallBindings()
        {
            Container.Bind<StatesFactory>().AsSingle();
            Container.Bind<MediatorStateMachine>().AsSingle();
        }
    }
}