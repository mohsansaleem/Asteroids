using PG.Asteroids.Contexts.GamePlay;
using PG.Asteroids.Models.MediatorModels;
using UnityEngine;
using Zenject;

namespace PG.Asteroids.Contexts.GamePlay
{
    public class SpawnAsteroidsCommand : IEntityCommand, IPoolable<int, RigidMovingEntity.MovingEntityModel, IMemoryPool>
    {
        [Inject] private readonly SimulationModel _simulationModel;
        [Inject] private readonly Asteroid.Factory _asteroidFactory;
        
        private int _levelIndex;
        private RigidMovingEntity.MovingEntityModel _entityModel;
        private IMemoryPool _commandPool;

        public void OnSpawned(int levelIndex, RigidMovingEntity.MovingEntityModel entityModel, IMemoryPool commandPool)
        {
            _commandPool = commandPool ?? throw new System.ArgumentNullException(nameof(commandPool));
            _levelIndex = levelIndex;
            _entityModel = entityModel;
            _commandPool = commandPool;
        }

        public void Execute()
        {
            var asteroid = _asteroidFactory.Create(_levelIndex, _entityModel);
            
            _simulationModel.AsteroidsCount.Value++;
            int entityId = _simulationModel.Register(asteroid, EntityMask.Movable | EntityMask.Explosive);
            
            if (entityId != -1)
                asteroid.EntityId = entityId;
            else
                Debug.LogError($"Unable to register asteroid entity in simulation model.");

            // Return this command object to its own pool!
            _commandPool.Despawn(this);
        }

        public void OnDespawned()
        {
            _levelIndex = -1;
            _entityModel = null;
            _commandPool = null;
        }

        public class CommandFactory : PlaceholderFactory<int, RigidMovingEntity.MovingEntityModel, SpawnAsteroidsCommand>
        {
        }
        
        public class CommandPool : MemoryPool<int, RigidMovingEntity.MovingEntityModel, IMemoryPool, SpawnAsteroidsCommand>
        {
        }
    }
}