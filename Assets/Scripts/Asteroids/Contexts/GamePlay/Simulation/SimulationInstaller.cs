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
            
            DeclareSignals();
            
            BindPlayerShip();
            BindAstroidsFactory();
            BindRocketFactory();
            BindExplosionFactory();
                
            BindSimulationSystems();
            Container.BindInterfacesAndSelfTo<Simulation>().AsSingle();
        }

        private void BindSimulationSystems()
        {
            Container.Bind<SimulationSystemFactory>().AsSingle();
            
            // Bind multiple different Simulation systems
            Container.Bind<ISimulationSystem>().To<EntitiesQueueSystem>().AsSingle();
            Container.Bind<ISimulationSystem>().To<AsteroidsSystem>().AsSingle();
            Container.Bind<ISimulationSystem>().To<PlayerInputSystem>().AsSingle();
            Container.Bind<ISimulationSystem>().To<ShipControlSystem>().AsSingle();
            Container.Bind<ISimulationSystem>().To<MovementSystem>().AsSingle();
            Container.Bind<ISimulationSystem>().To<ExplosionSystem>().AsSingle();
        }

        private void DeclareSignals()
        {
            Container.DeclareSignal<PlayerCrashedSignal>();
            Container.DeclareSignal<RocketHitSignal>();
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
            
            Container.BindInterfacesAndSelfTo<PlayerShip>()
                .FromComponentInNewPrefab(prefab)
                .AsSingle();
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