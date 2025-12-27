using PG.Asteroids.Models;
using PG.Asteroids.Models.MediatorModels;
using UnityEngine;
using Zenject;
using Zenject.Asteroids;

namespace PG.Asteroids.Contexts.GamePlay
{
    public class ExplosionSystem: ISimulationSystem
    {
        [Inject] private readonly SimulationModel _simulationModel;
        [Inject] private readonly Explosion.Factory _explosionFactory;
        [Inject] private readonly StaticDataModel _staticDataModel;
        [Inject] private readonly SignalBus _signalBus;
        
        public void Initialize()
        {
            _signalBus.Subscribe<RocketHitSignal>(OnRocketHit);
            _signalBus.Subscribe<PlayerCrashedSignal>(OnShipCrashed);
        }

        public void Tick(float deltaTime)
        {
            for (var index = 0; index < _simulationModel.SimulationEntities.Count; index++)
            {
                var simulationEntity = _simulationModel.SimulationEntities[index];
                if (simulationEntity is Explosion movingEntity)
                {
                    movingEntity.Tick(deltaTime);
                }
            }
        }

        public void FixedTick(float deltaTime)
        {
            
        }

        private void CreateExplosion(Vector3 position)
        {
            Explosion explosion = _explosionFactory.Create(_staticDataModel.MetaData.ExplosionSettings.ExplosionTimeout, position);
            _simulationModel.SimulationEntitiesQueue.Add(explosion);
        }
        
        private void OnRocketHit(RocketHitSignal signal)
        {
            CreateExplosion(signal.Asteroid.Position);
        }

        private void OnShipCrashed(PlayerCrashedSignal signal)
        {
            CreateExplosion(signal.Asteroid.Position);
        }
        
        public void Reset()
        {
            
        }

        public void Dispose()
        {
            _signalBus.Unsubscribe<RocketHitSignal>(OnRocketHit);
            _signalBus.Unsubscribe<PlayerCrashedSignal>(OnShipCrashed);
        }
    }
}