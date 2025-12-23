using PG.Asteroids.Models.MediatorModels;
using UnityEngine;
using Zenject;
using Zenject.SpaceFighter;

namespace PG.Asteroids.Contexts.GamePlay
{
    public class Rocket : LinearMovingEntity, IPoolable<float, Vector3, float, IMemoryPool>
    {
        [Inject] private SignalBus _signalBus;
        [Inject] private SimulationModel _simulationModel;
        
        private float _startTime;
        private float _lifeTime;
        IMemoryPool _pool;

        public void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("asteroid"))
            {
                _signalBus.Fire<RocketHitSignal>(new RocketHitSignal(other.GetComponent<Asteroid>(), this));
                _pool.Despawn(this);
            }
        }

        public override void Tick(float deltaTime)
        {
            base.Tick(deltaTime);

            if (Time.realtimeSinceStartup - _startTime > _lifeTime)
            {
                _simulationModel.SimulationEntitiesExpired.Add(this);
            }
        }

        public void OnSpawned(float lifeTime, Vector3 direction, float speed, IMemoryPool pool)
        {
            _pool = pool;
            _lifeTime = lifeTime;

            _startTime = Time.realtimeSinceStartup;
            
            Initialize(direction, speed);
        }
        
        public override void Despawn()
        {
            _pool?.Despawn(this);
        }

        public void OnDespawned()
        {
            _pool = null;
        }

        public class Factory : PlaceholderFactory<float, Vector3, float, Rocket>
        {
        }
        
        public class RocketPool : MonoPoolableMemoryPool<float, Vector3, float, IMemoryPool, Rocket>
        {
        }
    }
}
