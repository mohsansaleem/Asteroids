using PG.Core.Commands;
using Zenject;

namespace PG.Core.Installers
{
    public class CoreContextInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<LoadSceneCommand>().AsTransient();
            Container.Bind<UnloadSceneCommand>().AsTransient();
            
            Container.BindInterfacesTo<AsyncSceneLoader>().AsTransient();
            Container.BindInterfacesAndSelfTo<AssetsLoader>().AsSingle();
        }
    }
}