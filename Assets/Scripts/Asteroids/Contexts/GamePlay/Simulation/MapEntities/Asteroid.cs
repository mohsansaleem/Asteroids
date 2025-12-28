using System;
using ModestTree;
using UnityEngine;
using Zenject;

namespace PG.Asteroids.Contexts.GamePlay
{
    public class Asteroid : RigidMovingEntity, IPoolable<int, RigidMovingEntity.MovingEntityModel, IMemoryPool>
    {
        private const float RotationSpeed = 30f;

        public int LevelIndex { get; private set; }
        
        public void OnSpawned(int levelIndex, MovingEntityModel model, IMemoryPool pool)
        {
            Pool = pool ?? throw new System.ArgumentNullException(nameof(pool));
            LevelIndex = levelIndex;
            Initialize(model);
        }

        public override void FixedTick(float deltaTime)
        {
            base.FixedTick(deltaTime);
            transform.RotateAround(transform.position, Vector3.up, RotationSpeed * deltaTime);
        }

        public override void Despawn()
        {
            if (Pool == null)
                throw new System.InvalidOperationException($"{nameof(Asteroid)} pool is null - entity was not properly spawned");

            Pool.Despawn(this);
        }

        public void OnDespawned()
        {
            Pool = null;
        }

        public class Factory : PlaceholderFactory<int, MovingEntityModel, Asteroid>
        {
            
        }
        
        public class AsteroidPool : MonoPoolableMemoryPool<int, MovingEntityModel, IMemoryPool, Asteroid>
        {
        }
    }
}
