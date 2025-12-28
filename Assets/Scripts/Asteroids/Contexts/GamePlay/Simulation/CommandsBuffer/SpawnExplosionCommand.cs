using PG.Asteroids.Contexts.GamePlay;
using PG.Asteroids.Models.MediatorModels;
using UnityEngine;
using Zenject;

namespace PG.Asteroids.Contexts.GamePlay
{
    public class SpawnExplosionCommand : IEntityCommand, IPoolable<float, Vector3, IMemoryPool>
    {
        [Inject] private readonly SimulationModel _simulationModel;
        [Inject] private readonly Explosion.Factory _asteroidFactory;
        
        private float _explosionTime; 
        private Vector3 _position;
        private IMemoryPool _commandPool;

        public void OnSpawned(float explosionTime, Vector3 position, IMemoryPool commandPool)
        {
            _explosionTime = explosionTime;
            _position = position;
            _commandPool = commandPool;
        }

        public void Execute()
        {
            var explosion = _asteroidFactory.Create(_explosionTime, _position);
            
            _simulationModel.AsteroidsCount.Value++;
            int entityId = _simulationModel.Register(explosion, EntityMask.Movable | EntityMask.Explosive);
            if (entityId != -1)
                explosion.EntityId = entityId;
            else
                Debug.LogError($"Unable to register explosion entity in simulation model.");

            // Return this command object to its own pool!
            _commandPool.Despawn(this);
        }

        public void OnDespawned()
        {
            _position = Vector3.zero;
            _commandPool = null;
        }

        public class CommandFactory : PlaceholderFactory<float, Vector3, SpawnExplosionCommand>
        {
        }
        
        public class CommandPool : MemoryPool<float, Vector3, IMemoryPool, SpawnExplosionCommand>
        {
        }
    }
}