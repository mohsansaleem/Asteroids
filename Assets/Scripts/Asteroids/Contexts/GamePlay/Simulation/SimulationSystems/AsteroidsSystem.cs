using System.Runtime.InteropServices;
using Cysharp.Threading.Tasks;
using PG.Asteroids.Models;
using PG.Asteroids.Models.DataModels;
using PG.Asteroids.Models.MediatorModels;
using PG.Asteroids.Models.RemoteDataModels;
using UnityEngine;
using UnityEngine.Assertions;
using Zenject;
using Zenject.Asteroids;

namespace PG.Asteroids.Contexts.GamePlay
{
    public class AsteroidsSystem: ISimulationSystem
    {
        [Inject] private readonly Asteroid.Factory _asteroidFactory;
        [Inject] private readonly StaticDataModel _staticDataModel;
        [Inject] private readonly GamePlayModel _gamePlayModel;
        [Inject] private readonly SimulationModel _simulationModel;
        [Inject] private readonly LevelHelper _level;
        
        [Inject] private readonly SignalBus _signalBus;
        
        public void Initialize()
        {
            if (_simulationModel.AsteroidsCount.Value < _staticDataModel.MetaData.AsteroidsData.StartingSpawns)
            {
                SpawnNext();
            }
            _signalBus.Subscribe<RocketHitSignal>(OnRocketHit);
        }

        private void OnRocketHit(RocketHitSignal signal)
        {
            Asteroid asteroid = signal.Asteroid;
            _gamePlayModel.Scores.Value += _staticDataModel.MetaData.AsteroidsData.AsteroidLevels[asteroid.LevelIndex].HitPoints;
            _simulationModel.SimulationEntitiesExpired.Add(asteroid);
            _simulationModel.AsteroidsCount.Value--;

            if (asteroid.LevelIndex > 0)
            {
                SpawnAsteroidAt(0, signal.Asteroid.Transform.position);
                SpawnAsteroidAt(0, signal.Asteroid.Transform.position);
            }
        }

        public void Tick(float deltaTime)
        {
            if (_simulationModel.AsteroidsCount.Value < _staticDataModel.MetaData.AsteroidsData.MaxSpawns)
            {
                SpawnNext();
            }
        }
        
        private void SpawnNext()
        {
            AsteroidsData settings = _staticDataModel.MetaData.AsteroidsData;
            int levelIndex = Random.Range(0, settings.AsteroidLevels.Length);
            SpawnAsteroid(levelIndex);
        }

        private void SpawnAsteroid(int levelIndex)
        {
            AsteroidLevelData level = _staticDataModel.MetaData.AsteroidsData.AsteroidLevels[levelIndex];
            var sizePx = Random.Range(0.1f, 1.0f);
            var speed = Random.Range(level.MinSpeed, level.MaxSpeed);
            var scale = Mathf.Lerp(level.MinScale, level.MaxScale, sizePx);
            var mass = Mathf.Lerp(level.MinMass, level.MaxMass, sizePx);
            var position = GetRandomStartPosition(scale);
            var velocity = GetRandomDirection() * speed;
            SpawnAsteroidInternal(levelIndex, position, scale, mass, velocity, level);
        }
        
        private void SpawnAsteroidAt(int levelIndex, Vector3 position)
        {
            AsteroidLevelData level = _staticDataModel.MetaData.AsteroidsData.AsteroidLevels[levelIndex];
            var sizePx = Random.Range(0.1f, 1.0f);
            var speed = Random.Range(level.MinSpeed, level.MaxSpeed);
            var scale = Mathf.Lerp(level.MinScale, level.MaxScale, sizePx);
            var mass = Mathf.Lerp(level.MinMass, level.MaxMass, sizePx);
            var velocity = GetRandomDirection() * speed;
            
            SpawnAsteroidInternal(levelIndex, position, scale, mass, velocity, level);
        }

        private void SpawnAsteroidInternal(int levelIndex, Vector3 position, float scale, float mass, Vector3 velocity,
            AsteroidLevelData level)
        {
            var asteroid = _asteroidFactory.Create(levelIndex, new RigidMovingEntity.MovingEntityModel()
            {
                Scale = scale,
                Mass = mass,
                Position = position,
                Velocity = velocity,
                MaxSpeed =  level.MaxSpeed,
            });
            
            _simulationModel.AsteroidsCount.Value++;
            _simulationModel.SimulationEntitiesQueue.Add(asteroid);
        }

        private Vector3 GetRandomDirection()
        {
            var theta = Random.Range(0, Mathf.PI * 2.0f);
            return new Vector3(Mathf.Cos(theta), Mathf.Sin(theta), 0);
        }

        private Vector3 GetRandomStartPosition(float scale)
        {
            var side = (Side)Random.Range(0, (int)Side.Count);
            var rand = Random.Range(0.0f, 1.0f);

            switch (side)
            {
                case Side.Top:
                {
                    return new Vector3(_level.Left + rand * _level.Width, _level.Top + scale, 0);
                }
                case Side.Bottom:
                {
                    return new Vector3(_level.Left + rand * _level.Width, _level.Bottom - scale, 0);
                }
                case Side.Right:
                {
                    return new Vector3(_level.Right + scale, _level.Bottom + rand * _level.Height, 0);
                }
                case Side.Left:
                {
                    return new Vector3(_level.Left - scale, _level.Bottom + rand * _level.Height, 0);
                }
            }
            
            throw new System.NotImplementedException();
        }
        
        enum Side
        {
            Top,
            Bottom,
            Left,
            Right,
            Count
        }

        public void FixedTick(float deltaTime)
        {
            
        }

        public void Reset()
        {
            
        }

        public void Dispose()
        {
            _signalBus.Unsubscribe<RocketHitSignal>(OnRocketHit);
        }
    }
}