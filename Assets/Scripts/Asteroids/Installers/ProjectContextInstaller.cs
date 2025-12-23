using PG.Asteroids.Models;
using PG.Asteroids.Models.RemoteDataModels;
using PG.Core.Installers;
using PG.Asteroids.Models.MediatorModels;
using PG.Asteroids.Service;
using PG.Core;

namespace PG.Asteroids.Installers
{
    public class ProjectContextInstaller : CoreContextInstaller
    {
        public override void InstallBindings()
        {
            base.InstallBindings();

            Container.Bind<StaticDataModel>().AsSingle();
            Container.Bind<RemoteDataModel>().AsSingle();
            Container.BindInterfacesAndSelfTo<ScriptableStorageService>().AsSingle();
        }
    }
}