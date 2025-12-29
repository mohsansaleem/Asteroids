using PG.Asteroids.Contexts.GamePlay;
using UnityEngine;
using Zenject;

namespace PG.Asteroids.Contexts.GamePlay
{
    /// <summary>
    /// Facade for queueing commands into the command buffer.
    /// Now uses generic Enqueue method - no need to inject individual factories.
    /// </summary>
    public class CommandBufferMediator
    {
        [Inject] private CommandBuffer _buffer;

        public void Playback()
        {
            _buffer.Playback();
        }

        public void RequestSpawnAsteroid(int levelIndex, RigidMovingEntity.MovingEntityModel entityModel)
        {
            _buffer.Enqueue<SpawnAsteroidsCommand>(levelIndex, entityModel);
        }

        public void RequestShipCrash(int entityId)
        {
            _buffer.Enqueue<ShipCrashedCommand>(entityId);
        }

        public void RequestSpawnRocket(Vector3 position, Vector3 moveDirection, Quaternion rotation)
        {
            _buffer.Enqueue<SpawnRocketCommand>(position, moveDirection, rotation);
        }

        public void RequestAsteroidHit(int asteroidId)
        {
            _buffer.Enqueue<AsteroidHitCommand>(asteroidId);
        }

        public void RequestSpawnExplosion(float explosionTime, Vector3 position)
        {
            _buffer.Enqueue<SpawnExplosionCommand>(explosionTime, position);
        }

        public void RequestDestroy(int id, IMemoryPool entityPool)
        {
            _buffer.Enqueue<DestroyEntityCommand>(id, entityPool);
        }
    }
}