using Zenject;
using UnityEngine;
using NUnit.Framework;
using PG.Asteroids.Contexts.GamePlay;
using PG.Asteroids.Models;
using PG.Asteroids.Models.MediatorModels;
using PG.Core;
using Zenject.Asteroids;

[TestFixture]
public class SimulationTests : ZenjectUnitTestFixture
{
    [Test]
    public void SimulationSystemsFactory()
    {
        Container.Bind<SimulationSystemFactory>().AsSingle();

        Assert.IsNotNull(Container.Resolve<SimulationSystemFactory>());
    }
    
    [Test]
    public void SimulationSystemsInjection()
    {
        Container.Bind<SimulationSystemFactory>().AsSingle();
        Container.Bind<SimulationModel>().AsSingle();
        
        // Bind multiple different Simulation systems
        Container.Bind<ISimulationSystem>().To<MovementSystem>().AsSingle();
        Container.Bind<ISimulationSystem>().To<PlayerInputSystem>().AsSingle();
        
        var list = Container.ResolveAll<ISimulationSystem>();
        Assert.IsTrue(list?.Count == 2);
    }

    [Test]
    public void SimulationAssetLoader()
    {
        Container.BindInterfacesAndSelfTo<AssetsLoader>().AsSingle();
        Container.Bind<SimulationModel>().AsSingle();

        GameObject obj = new GameObject();
        Camera camera = obj.AddComponent<Camera>();
        obj.tag = "MainCamera";
        Container.BindInstances(camera);

        var levelHelper = new LevelHelper(camera);
        Container.BindInstance<LevelHelper>(levelHelper).AsSingle();

        AssetsLoader assetsLoader = Container.Resolve<AssetsLoader>();
        GameObject prefab = assetsLoader.Load<GameObject>("Asteroid");
        
        var asteroid = Container.InstantiatePrefab(prefab);
        Assert.IsNotNull(asteroid);
    }

    [Test]
    public void ShipFactory()
    {
        Container.BindInterfacesAndSelfTo<AssetsLoader>().AsSingle();
        Container.Bind<GamePlayModel>().AsSingle();
        Container.Bind<SimulationModel>().AsSingle();
        Container.Bind<StaticDataModel>().AsSingle();
        SignalBusInstaller.Install(Container);

        GameObject obj = new GameObject();
        Camera camera = obj.AddComponent<Camera>();
        obj.tag = "MainCamera";
        Container.BindInstances(camera);

        var levelHelper = new LevelHelper(camera);
        Container.BindInstance<LevelHelper>(levelHelper).AsSingle();
        
        AssetsLoader assetsLoader = Container.Resolve<AssetsLoader>();
        
        GameObject prefab = assetsLoader.Load<GameObject>("Ship");

        Container.BindFactory<PlayerShip, PlayerShip.Factory>()
            .FromComponentInNewPrefab(prefab);
        
        PlayerShip.Factory factory = Container.Resolve<PlayerShip.Factory>();
        var asteroid = factory.Create();
        Assert.IsNotNull(asteroid);
    }
}