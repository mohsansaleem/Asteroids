using PG.Core.Commands;
using Zenject;

namespace PG.Core.Installers
{
    public class CoreContextInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            SignalBusInstaller.Install(Container);
            
            Container.DeclareSignal<CommandExecutedSignal>();
            
            Container.DeclareSignal<LoadSceneSignal>();
            Container.BindSignal<LoadSceneSignal>()
                .ToMethod<LoadSceneCommand>(x => x.Execute)
                .FromNew();

            Container.DeclareSignal<UnloadSceneSignal>();
            Container.BindSignal<UnloadSceneSignal>()
                .ToMethod<UnloadSceneCommand>(x => x.Execute)
                .FromNew();

            Container.BindInterfacesTo<AsyncSceneLoader>().AsTransient();
            Container.BindInterfacesAndSelfTo<AssetsLoader>().AsSingle();
        }
    }
}