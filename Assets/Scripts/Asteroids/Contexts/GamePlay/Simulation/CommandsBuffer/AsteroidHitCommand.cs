using PG.Asteroids.Models;
using PG.Asteroids.Models.DataModels;
using PG.Asteroids.Models.SimulationModels;
using PG.Asteroids.Models.MediatorModels;
using UnityEngine;
using Zenject;
using Zenject.SpaceFighter;

namespace PG.Asteroids.Contexts.GamePlay
{
    public class AsteroidHitCommand : IEntityCommand, IPoolable<Asteroid, IMemoryPool>
    {
        [Inject] private readonly SimulationModel _simulationModel;
        [Inject] private readonly GamePlayModel _gamePlayModel;
        [Inject] private readonly StaticDataModel _staticDataModel;
        [Inject] private readonly AudioPlayer _audioPlayer;
        [Inject] private readonly PlayerShip _player;
        [Inject] private readonly CommandBufferMediator _commandBufferMediator;

        private Asteroid _asteroid;
        private IMemoryPool _commandPool;

        public void OnSpawned(Asteroid asteroid, IMemoryPool commandPool)
        {
            _asteroid = asteroid;
            _commandPool = commandPool;
        }

        public void Execute()
        {
            if (_asteroid != null)
            {
                _gamePlayModel.Scores.Value += _staticDataModel.MetaData.AsteroidsData.AsteroidLevels[_asteroid.LevelIndex].HitPoints;

                // Despawn the asteroid
                if (_asteroid.Pool != null)
                    _asteroid.Pool.Despawn(_asteroid);

                _simulationModel.AsteroidsCount.Value--;
                Debug.Log($"[AsteroidHitCommand] Asteroid destroyed. Count: {_simulationModel.AsteroidsCount.Value} (was {_simulationModel.AsteroidsCount.Value + 1})");

                // BUG FIX: Spawn smaller asteroids when large/medium asteroids are destroyed
                // Level 0 = Small, Level 1 = Medium, Level 2 = Large (reverse order!)
                // Large (2) -> spawn 2 Medium (1), Medium (1) -> spawn 2 Small (0), Small (0) -> don't spawn
                if (_asteroid.LevelIndex > 0)
                {
                    int nextLevel = _asteroid.LevelIndex - 1; // Go DOWN in level (bigger to smaller)
                    RequestSpawnAsteroidAt(nextLevel, _asteroid.transform.position);
                    RequestSpawnAsteroidAt(nextLevel, _asteroid.transform.position);
                    Debug.Log($"[AsteroidHitCommand] Spawning 2 asteroids at level {nextLevel} (from level {_asteroid.LevelIndex})");
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
            _asteroid = null;
            _commandPool = null;
        }

        public class CommandFactory : PlaceholderFactory<Asteroid, AsteroidHitCommand>, ICommandFactory<AsteroidHitCommand>
        {
            public AsteroidHitCommand Create(params object[] args)
            {
                return base.Create(args[0] as Asteroid);
            }
        }

        public class CommandPool : MemoryPool<Asteroid, IMemoryPool, AsteroidHitCommand>
        {
        }
    }
}
