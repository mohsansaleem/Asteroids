using PG.Asteroids.Contexts.GamePlay;
using PG.Asteroids.Models.MediatorModels;
using UnityEngine;
using Zenject;
using Zenject.Asteroids;
using Zenject.SpaceFighter;
using Asteroid = PG.Asteroids.Contexts.GamePlay.Asteroid;
using Explosion = PG.Asteroids.Contexts.GamePlay.Explosion;

namespace PG.Core.Contexts.StateManagement
{
    public class SimulationInstaller : Installer<SimulationInstaller>
    {
        [Inject]  private IAssetsLoader _assetsLoader;
        
        public override void InstallBindings()
        {
            BindMisc();
            
            BindAstroidsFactory();
            BindRocketFactory();
            BindExplosionFactory();
            
            BindCommandBuffer();
            
            BindPlayerShip();
            BindSimulationSystems();
            Container.BindInterfacesAndSelfTo<Simulation>().AsSingle();
        }

        private void BindSimulationSystems()
        {
            Container.Bind<SimulationSystemFactory>().AsSingle();
            
            // Bind multiple different Simulation systems
            Container.Bind<ISimulationSystem>().To<AsteroidsSystem>().AsSingle();
            Container.Bind<ISimulationSystem>().To<PlayerInputSystem>().AsSingle();
            Container.Bind<ISimulationSystem>().To<ShipControlSystem>().AsSingle();
            Container.Bind<ISimulationSystem>().To<MovementSystem>().AsSingle();
            Container.Bind<ISimulationSystem>().To<ExplosionSystem>().AsSingle();
        }

        private void BindCommandBuffer()
        {
            // Bind command pools
            Container.BindFactory<int, RigidMovingEntity.MovingEntityModel, SpawnAsteroidsCommand, SpawnAsteroidsCommand.CommandFactory>()
                .FromPoolableMemoryPool(poolBinder => poolBinder
                    .WithInitialSize(10)
                    .FromNew());
            Container.BindFactory<int, ShipCrashedCommand, ShipCrashedCommand.CommandFactory>()
                .FromPoolableMemoryPool(poolBinder => poolBinder
                    .WithInitialSize(1)
                    .FromNew());
             Container.BindFactory<Vector3, Vector3, Quaternion, SpawnRocketCommand, SpawnRocketCommand.CommandFactory>()
                 .FromPoolableMemoryPool(poolBinder => poolBinder
                    .WithInitialSize(10)
                    .FromNew());
             Container.BindFactory<int, AsteroidHitCommand, AsteroidHitCommand.CommandFactory>()
                 .FromPoolableMemoryPool(poolBinder => poolBinder
                     .WithInitialSize(5)
                     .FromNew());
            Container.BindFactory<float, Vector3, SpawnExplosionCommand, SpawnExplosionCommand.CommandFactory>()
                .FromPoolableMemoryPool(poolBinder => poolBinder
                    .WithInitialSize(5)
                    .FromNew());
            Container.BindFactory<int, IMemoryPool, DestroyEntityCommand, DestroyEntityCommand.CommandFactory>()
                .FromPoolableMemoryPool(poolBinder => poolBinder
                    .WithInitialSize(7)
                    .FromNew());

            // Bind the generic ICommandFactory<T> interfaces to the already-existing factories
            // Use To().FromResolve() to alias the existing factory instances
            Container.Bind<ICommandFactory<SpawnAsteroidsCommand>>().To<SpawnAsteroidsCommand.CommandFactory>().FromResolve();
            Container.Bind<ICommandFactory<ShipCrashedCommand>>().To<ShipCrashedCommand.CommandFactory>().FromResolve();
            Container.Bind<ICommandFactory<SpawnRocketCommand>>().To<SpawnRocketCommand.CommandFactory>().FromResolve();
            Container.Bind<ICommandFactory<AsteroidHitCommand>>().To<AsteroidHitCommand.CommandFactory>().FromResolve();
            Container.Bind<ICommandFactory<SpawnExplosionCommand>>().To<SpawnExplosionCommand.CommandFactory>().FromResolve();
            Container.Bind<ICommandFactory<DestroyEntityCommand>>().To<DestroyEntityCommand.CommandFactory>().FromResolve();

            Container.Bind<CommandBuffer>().AsSingle();
            Container.BindInterfacesAndSelfTo<CommandBufferMediator>().AsSingle();
        }
        
        private void BindMisc()
        {
            Container.Bind<SimulationModel>().AsSingle();
            Container.Bind<LevelHelper>().AsSingle();
            Container.Bind<AudioPlayer>().AsSingle();
        }

        private void BindPlayerShip()
        {
            GameObject prefab = _assetsLoader.Load<GameObject>("Ship");
            Container.BindFactory<PlayerShip, PlayerShip.Factory>().FromComponentInNewPrefab(prefab);

            PlayerShip.Factory factory = Container.Resolve<PlayerShip.Factory>();
            PlayerShip playerShip = factory.Create();
            
            Container.BindInterfacesAndSelfTo<PlayerShip>().FromInstance(playerShip).AsSingle();
        }
        
        private void BindAstroidsFactory()
        {
            GameObject prefab = _assetsLoader.Load<GameObject>("Asteroid");

            Container.BindFactory<int, RigidMovingEntity.MovingEntityModel, Asteroid, Asteroid.Factory>()
                .FromPoolableMemoryPool<int, RigidMovingEntity.MovingEntityModel, Asteroid, Asteroid.AsteroidPool>(poolBinder => poolBinder
                    .WithInitialSize(15)
                    .FromComponentInNewPrefab(prefab)
                    .UnderTransformGroup("Asteroids"));
        }
        
        private void BindRocketFactory()
        {
            GameObject prefab = _assetsLoader.Load<GameObject>("Rocket");

            Container.BindFactory<float, Vector3, float, Rocket, Rocket.Factory>()
                .FromPoolableMemoryPool<float, Vector3, float, Rocket, Rocket.RocketPool>(poolBinder => poolBinder
                    .WithInitialSize(6)
                    .FromComponentInNewPrefab(prefab)
                    .UnderTransformGroup("Rockets"));
        }
        
        private void BindExplosionFactory()
        {
            GameObject prefab = _assetsLoader.Load<GameObject>("Explosion");

            Container.BindFactory<float, Vector3, Explosion, Explosion.Factory>()
                .FromPoolableMemoryPool<float, Vector3, Explosion, Explosion.ExplosionPool>(poolBinder => poolBinder
                    .WithInitialSize(3)
                    .FromComponentInNewPrefab(prefab)
                    .UnderTransformGroup("Explosions"));
        }
    }
}