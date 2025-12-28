using PG.Asteroids.Models;
using PG.Asteroids.Models.DataModels;
using PG.Asteroids.Models.MediatorModels;
using UnityEngine;
using Zenject;
using Zenject.SpaceFighter;

namespace PG.Asteroids.Contexts.GamePlay
{
    public class Rocket : LinearMovingEntity, IPoolable<float, Vector3, float, IMemoryPool>
    {
        [Inject] private SimulationModel _simulationModel;
        [Inject] private StaticDataModel _staticDataModel;
        [Inject] private AudioPlayer _audioPlayer;
        [Inject] private CommandBufferMediator _commandBufferMediator;

        private float _startTime;
        private float _lifeTime;
        
        public void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("asteroid"))
            {
                var asteroid = other.GetComponent<Asteroid>();
                if (asteroid != null)
                {
                    _commandBufferMediator.RequestSpawnExplosion(_staticDataModel.MetaData.ExplosionSettings.ExplosionTimeout, Position);
                    _commandBufferMediator.RequestDestroy(EntityId, Pool);
                    _commandBufferMediator.RequestAsteroidHit(asteroid.EntityId);
                }
            }
        }

        public override void Tick(float deltaTime)
        {
            base.Tick(deltaTime);

            if (Time.realtimeSinceStartup - _startTime > _lifeTime)
            {
                _commandBufferMediator.RequestDestroy(EntityId, Pool);
            }
        }

        public void OnSpawned(float lifeTime, Vector3 direction, float speed, IMemoryPool pool)
        {
            Pool = pool ?? throw new System.ArgumentNullException(nameof(pool));
            _lifeTime = lifeTime;
            _startTime = Time.realtimeSinceStartup;

            BulletSettings bulletSettings = _staticDataModel.MetaData.BulletSettings;
            _audioPlayer.Play(bulletSettings.Laser, bulletSettings.LaserVolume);

            Initialize(direction, speed);
        }

        public override void Despawn()
        {
            if (Pool == null)
                throw new System.InvalidOperationException($"{nameof(Rocket)} pool is null - entity was not properly spawned");

            Pool.Despawn(this);
        }

        public void OnDespawned()
        {
            Pool = null;
        }

        public class Factory : PlaceholderFactory<float, Vector3, float, Rocket>
        {
        }
        
        public class RocketPool : MonoPoolableMemoryPool<float, Vector3, float, IMemoryPool, Rocket>
        {
        }
    }
}
