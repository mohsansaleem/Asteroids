using PG.Asteroids.Contexts.GamePlay;
using UnityEngine;
using Zenject;

namespace PG.Asteroids.Contexts.GamePlay
{
    public class CommandBufferMediator
    {
        [Inject] private SpawnAsteroidsCommand.CommandFactory _spawnAsteroidCommandFactory;
        [Inject] private ShipCrashedCommand.CommandFactory _shipCrashedCommandFactory;
        [Inject] private SpawnRocketCommand.CommandFactory _spawnRocketCommandFactory;
        [Inject] private AsteroidHitCommand.CommandFactory _asteroidHitCommandFactory;
        [Inject] private SpawnExplosionCommand.CommandFactory _spawnExplosionCommandFactory;
        [Inject] private DestroyEntityCommand.CommandFactory _destroyCommandFactory;
        
        [Inject] private CommandBuffer _buffer;

        public void Playback()
        {
            _buffer.Playback();
        }
        
        public void RequestSpawnAsteroid(int levelIndex, RigidMovingEntity.MovingEntityModel entityModel)
        {
            _buffer.AddCommand(_spawnAsteroidCommandFactory.Create(levelIndex, entityModel));
        }
        
        public void RequestShipCrash(int entityId)
        {
            _buffer.AddCommand(_shipCrashedCommandFactory.Create(entityId));
        }
        
        public void RequestSpawnRocket(Vector3 position, Vector3 moveDirection, Quaternion rotation)
        {
            _buffer.AddCommand(_spawnRocketCommandFactory.Create(position, moveDirection, rotation));
        }
        
        public void RequestAsteroidHit(int asteroidId)
        {
            _buffer.AddCommand(_asteroidHitCommandFactory.Create(asteroidId));
        }
        
        public void RequestSpawnExplosion(float explosionTime, Vector3 position)
        {
            _buffer.AddCommand(_spawnExplosionCommandFactory.Create(explosionTime, position));
        }

        public void RequestDestroy(int id, IMemoryPool entityPool)
        {
            _buffer.AddCommand(_destroyCommandFactory.Create(id, entityPool));
        }
    }
}