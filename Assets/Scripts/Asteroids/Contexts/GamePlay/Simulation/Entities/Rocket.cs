using PG.Asteroids.Models;
using PG.Asteroids.Models.DataModels;
using PG.Asteroids.Models.SimulationModels;
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
            Debug.Log($"[Rocket] OnTriggerEnter with {other.gameObject.name}, tag: {other.tag}");

            if (other.CompareTag("asteroid"))
            {
                var asteroid = other.GetComponent<Asteroid>();
                if (asteroid != null)
                {
                    Debug.Log($"[Rocket] HIT ASTEROID at {Position}! Requesting explosion and asteroid hit");

                    _commandBufferMediator.RequestSpawnExplosion(_staticDataModel.MetaData.ExplosionSettings.ExplosionTimeout, Position);
                    _commandBufferMediator.RequestDestroy(this);
                    _commandBufferMediator.RequestAsteroidHit(asteroid);
                }
                else
                {
                    Debug.LogWarning($"[Rocket] Collider has 'asteroid' tag but no Asteroid component!");
                }
            }
        }

        protected override void Tick(float deltaTime)
        {
            base.Tick(deltaTime);

            if (Time.realtimeSinceStartup - _startTime > _lifeTime)
            {
                _commandBufferMediator.RequestDestroy(this);
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
