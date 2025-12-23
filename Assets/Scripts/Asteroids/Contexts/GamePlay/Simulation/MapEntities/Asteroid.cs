using System;
using ModestTree;
using UnityEngine;
using Zenject;

namespace PG.Asteroids.Contexts.GamePlay
{
    public class Asteroid : RigidMovingEntity, IPoolable<int, RigidMovingEntity.MovingEntityModel, IMemoryPool>
    {
        private IMemoryPool _pool;
        private int _levelIndex;
        
        public override void Tick(float deltaTime)
        {
            base.Tick(deltaTime);
        }

        public void OnSpawned(int levelIndex, MovingEntityModel model, IMemoryPool pool)
        {
            Initialize(model);
            _pool = pool;
            _levelIndex = levelIndex;
        }
        
        public int LevelIndex => _levelIndex;
        
        public override void Despawn()
        {
            _pool?.Despawn(this);
        }

        public void OnDespawned()
        {
        }

        public class Factory : PlaceholderFactory<int, MovingEntityModel, Asteroid>
        {
            
        }
        
        public class AsteroidPool : MonoPoolableMemoryPool<int, MovingEntityModel, IMemoryPool, Asteroid>
        {
        }
    }
}
