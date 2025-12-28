using PG.Asteroids.Contexts.GamePlay;
using PG.Asteroids.Models;
using PG.Asteroids.Models.DataModels;
using PG.Asteroids.Models.MediatorModels;
using UnityEngine;
using Zenject;
using Zenject.Asteroids;
using Zenject.SpaceFighter;

#pragma warning disable 649

namespace PG.Asteroids.Contexts.GamePlay
{
    public class Explosion : SimulationEntity, IPoolable<float, Vector3, IMemoryPool>
    {
        [SerializeField]
        ParticleSystem _particleSystem;

        [Inject] private SimulationModel _simulationModel;
        [Inject] private StaticDataModel _staticDataModel;
        [Inject] private AudioPlayer _audioPlayer;
        [Inject] private CommandBufferMediator _commandBufferMediator;

        private float _startTime;
        private float _lifeTime;

        public override void Tick(float deltaTime)
        {
            base.Tick(deltaTime);
            if (Time.realtimeSinceStartup - _startTime > _lifeTime)
            {
                _commandBufferMediator.RequestDestroy(EntityId, Pool);
            }
        }

        public override void Despawn()
        {
            if (Pool == null)
                throw new System.InvalidOperationException($"{nameof(Explosion)} pool is null - entity was not properly spawned");

            Pool.Despawn(this);
        }

        public void OnDespawned()
        {
            Pool = null;
        }

        public void OnSpawned(float lifeTime, Vector3 position, IMemoryPool pool)
        {
            Pool = pool ?? throw new System.ArgumentNullException(nameof(pool));
            _lifeTime = lifeTime;
            _startTime = Time.realtimeSinceStartup;

            Initialize();

            _particleSystem.Clear();
            _particleSystem.Play();

            ExplosionSettings explosionSettings = _staticDataModel.MetaData.ExplosionSettings;
            _audioPlayer.Play(explosionSettings.Explosion, explosionSettings.ExplosionVolume);

            transform.position = position;
        }

        public class Factory : PlaceholderFactory<float, Vector3, Explosion>
        {
        }
        
        public class ExplosionPool : MonoPoolableMemoryPool<float, Vector3, IMemoryPool, Explosion>
        {
        }
    }
}

