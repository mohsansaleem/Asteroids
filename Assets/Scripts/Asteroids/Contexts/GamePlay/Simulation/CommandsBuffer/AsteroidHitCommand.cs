using PG.Asteroids.Models;
using PG.Asteroids.Models.DataModels;
using PG.Asteroids.Models.MediatorModels;
using UnityEngine;
using Zenject;
using Zenject.SpaceFighter;

namespace PG.Asteroids.Contexts.GamePlay
{
    public class AsteroidHitCommand : IEntityCommand, IPoolable<int, IMemoryPool>
    {
        [Inject] private readonly SimulationModel _simulationModel;
        [Inject] private readonly GamePlayModel _gamePlayModel;
        [Inject] private readonly StaticDataModel _staticDataModel;
        [Inject] private readonly AudioPlayer _audioPlayer;
        [Inject] private readonly PlayerShip _player;
        [Inject] private readonly CommandBufferMediator _commandBufferMediator;
        
        private int _id;
        private IMemoryPool _commandPool;

        public void OnSpawned(int id, IMemoryPool commandPool)
        {
            _id = id;
            _commandPool = commandPool;
        }

        public void Execute()
        {
            if (_simulationModel.Masks[_id] != EntityMask.None)
            {
                Asteroid asteroid = _simulationModel.Views[_id] as Asteroid;
                _gamePlayModel.Scores.Value += _staticDataModel.MetaData.AsteroidsData.AsteroidLevels[asteroid.LevelIndex].HitPoints;
                
                _commandBufferMediator.RequestDestroy(asteroid.EntityId, asteroid.Pool);
                _simulationModel.AsteroidsCount.Value--;

                if (asteroid.LevelIndex > 0)
                {
                    RequestSpawnAsteroidAt(0, asteroid.transform.position);
                    RequestSpawnAsteroidAt(0, asteroid.transform.position);
                }
            }

            // Return this command object to its own pool!
            _commandPool.Despawn(this);
        }
        
        private void RequestSpawnAsteroidAt(int levelIndex, Vector3 position)
        {
            AsteroidLevelData level = _staticDataModel.MetaData.AsteroidsData.AsteroidLevels[levelIndex];
            var sizePx = Random.Range(0.1f, 1.0f);
            var speed = Random.Range(level.MinSpeed, level.MaxSpeed);
            var scale = Mathf.Lerp(level.MinScale, level.MaxScale, sizePx);
            var mass = Mathf.Lerp(level.MinMass, level.MaxMass, sizePx);
            var velocity = GetRandomDirection() * speed;
            
            _commandBufferMediator.RequestSpawnAsteroid(levelIndex, new RigidMovingEntity.MovingEntityModel()
            {
                Scale = scale,
                Mass = mass,
                Position = position,
                Velocity = velocity,
                MaxSpeed =  level.MaxSpeed,
            });
        }
        
        private Vector3 GetRandomDirection()
        {
            var theta = Random.Range(0, Mathf.PI * 2.0f);
            return new Vector3(Mathf.Cos(theta), Mathf.Sin(theta), 0);
        }

        public void OnDespawned()
        {
            _id = -1;
            _commandPool = null;
        }

        public class CommandFactory : PlaceholderFactory<int, AsteroidHitCommand>
        {
        }
        
        public class CommandPool : MemoryPool<int, IMemoryPool, AsteroidHitCommand>
        {
        }
    }
}