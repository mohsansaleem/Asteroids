using PG.Asteroids.Contexts.GamePlay;
using PG.Asteroids.Models.MediatorModels;
using UnityEngine;
using Zenject;

#pragma warning disable 649

namespace PG.Asteroids.Contexts.GamePlay
{
    public class Explosion : SimulationEntity, IPoolable<float, Vector3, IMemoryPool>
    {
        [SerializeField]
        ParticleSystem _particleSystem;
        
        [Inject] private SignalBus _signalBus;
        [Inject] private SimulationModel _simulationModel;
        
        private float _startTime;
        private float _lifeTime;

        IMemoryPool _pool;

        public override void Tick(float deltaTime)
        {
            base.Tick(deltaTime);
            if (Time.realtimeSinceStartup - _startTime > _lifeTime)
            {
                _simulationModel.SimulationEntitiesExpired.Add(this);
            }
        }

        public override void Despawn()
        {
            _pool.Despawn(this);
        }

        public void OnDespawned()
        {
        }

        public void OnSpawned(float lifeTime, Vector3 position, IMemoryPool pool)
        {
            Initialize();
            
            _particleSystem.Clear();
            _particleSystem.Play();

            _lifeTime = lifeTime;
            _startTime = Time.realtimeSinceStartup;
            _pool = pool;
            Transform.position = position;
        }

        public class Factory : PlaceholderFactory<float, Vector3, Explosion>
        {
        }
        
        public class ExplosionPool : MonoPoolableMemoryPool<float, Vector3, IMemoryPool, Explosion>
        {
        }
    }
}

