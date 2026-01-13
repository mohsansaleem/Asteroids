using Zenject;
using UnityEngine;
using NUnit.Framework;
using PG.Asteroids.Contexts.GamePlay;
using PG.Asteroids.Models;
using PG.Asteroids.Models.MediatorModels;
using PG.Asteroids.Models.SimulationModels;
using PG.Core;
using Zenject.Asteroids;
using Asteroid = PG.Asteroids.Contexts.GamePlay.Asteroid;

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
        Container.Bind<GamePlayModel>().AsSingle();
        Container.Bind<StaticDataModel>().AsSingle();
        SignalBusInstaller.Install(Container);

        // Setup camera and level helper for AsteroidsSystem
        GameObject obj = new GameObject();
        Camera camera = obj.AddComponent<Camera>();
        obj.tag = "MainCamera";
        Container.BindInstances(camera);
        var levelHelper = new LevelHelper(camera);
        Container.BindInstance<LevelHelper>(levelHelper).AsSingle();

        // Bind command buffer for AsteroidsSystem
        BindCommandBuffer();

        // Bind simulation systems
        Container.Bind<ISimulationSystem>().To<PlayerInputSystem>().AsSingle();
        Container.Bind<ISimulationSystem>().To<AsteroidsSystem>().AsSingle();

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
        
        BindCommandBuffer();

        AssetsLoader assetsLoader = Container.Resolve<AssetsLoader>();
        
        GameObject prefab = assetsLoader.Load<GameObject>("Ship");

        Container.BindFactory<PlayerShip, PlayerShip.Factory>()
            .FromComponentInNewPrefab(prefab);
        
        PlayerShip.Factory factory = Container.Resolve<PlayerShip.Factory>();
        var asteroid = factory.Create();
        Assert.IsNotNull(asteroid);
    }

    private void BindCommandBuffer()
    {
        Container.Bind<CommandBuffer>().AsSingle();

        // Updated command factory bindings to match new reference-based signatures
        Container.BindFactory<int, RigidMovingEntity.MovingEntityModel, SpawnAsteroidsCommand, SpawnAsteroidsCommand.CommandFactory>().FromNew();
        Container.BindFactory<int, ShipCrashedCommand, ShipCrashedCommand.CommandFactory>().FromNew();
        Container.BindFactory<Vector3, Vector3, Quaternion, SpawnRocketCommand, SpawnRocketCommand.CommandFactory>().FromNew();
        Container.BindFactory<Asteroid, AsteroidHitCommand, AsteroidHitCommand.CommandFactory>().FromNew();
        Container.BindFactory<float, Vector3, SpawnExplosionCommand, SpawnExplosionCommand.CommandFactory>().FromNew();
        Container.BindFactory<SimulationEntity, DestroyEntityCommand, DestroyEntityCommand.CommandFactory>().FromNew();

        Container.BindInterfacesAndSelfTo<CommandBufferMediator>().AsSingle();
    }
}