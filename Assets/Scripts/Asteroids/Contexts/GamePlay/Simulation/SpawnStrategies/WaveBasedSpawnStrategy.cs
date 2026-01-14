using PG.Asteroids.Models;
using PG.Asteroids.Models.DataModels;
using PG.Asteroids.Models.SimulationModels;
using UnityEngine;
using Zenject;
using Zenject.Asteroids;

namespace PG.Asteroids.Contexts.GamePlay
{
    /// <summary>
    /// Spawns asteroids in waves. When all asteroids are destroyed,
    /// spawns a new wave with increased difficulty.
    /// </summary>
    public class WaveBasedSpawnStrategy : IAsteroidSpawnStrategy
    {
        [Inject] private readonly CommandBufferMediator _commandBufferMediator;
        [Inject] private readonly StaticDataModel _staticDataModel;
        [Inject] private readonly SimulationModel _simulationModel;
        [Inject] private readonly LevelHelper _level;

        private int _currentWave;
        private int _asteroidsPerWave;

        public void InitializeSpawns()
        {
            _currentWave = 1;
            _asteroidsPerWave = _staticDataModel.MetaData.AsteroidsData.StartingSpawns;
            SpawnWave();
        }

        public void UpdateSpawns(float deltaTime)
        {
            // When all asteroids destroyed, spawn next wave
            if (_simulationModel.AsteroidsCount == 0)
            {
                _currentWave++;
                _asteroidsPerWave = Mathf.Min(
                    _asteroidsPerWave + 2,
                    _staticDataModel.MetaData.AsteroidsData.MaxSpawns
                );
                SpawnWave();
            }
        }

        public void Reset()
        {
            _currentWave = 0;
            _asteroidsPerWave = 0;
        }

        private void SpawnWave()
        {
            for (int i = 0; i < _asteroidsPerWave; i++)
            {
                SpawnRandomAsteroid();
            }
        }

        private void SpawnRandomAsteroid()
        {
            AsteroidsData settings = _staticDataModel.MetaData.AsteroidsData;

            // Higher waves spawn higher-level asteroids more frequently
            int maxLevel = Mathf.Min(_currentWave, settings.AsteroidLevels.Length - 1);
            int levelIndex = Random.Range(0, maxLevel + 1);

            AsteroidLevelData level = settings.AsteroidLevels[levelIndex];
            var sizePx = Random.Range(0.1f, 1.0f);
            var speed = Random.Range(level.MinSpeed, level.MaxSpeed);
            var scale = Mathf.Lerp(level.MinScale, level.MaxScale, sizePx);
            var mass = Mathf.Lerp(level.MinMass, level.MaxMass, sizePx);
            var position = GetRandomStartPosition(scale);
            var velocity = GetRandomDirection() * speed;

            _commandBufferMediator.RequestSpawnAsteroid(levelIndex, new RigidMovingEntity.MovingEntityModel()
            {
                Scale = scale,
                Mass = mass,
                Position = position,
                Velocity = velocity,
                MaxSpeed = level.MaxSpeed,
            });
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
                    return new Vector3(_level.Left + rand * _level.Width, _level.Top + scale, 0);
                case Side.Bottom:
                    return new Vector3(_level.Left + rand * _level.Width, _level.Bottom - scale, 0);
                case Side.Right:
                    return new Vector3(_level.Right + scale, _level.Bottom + rand * _level.Height, 0);
                case Side.Left:
                    return new Vector3(_level.Left - scale, _level.Bottom + rand * _level.Height, 0);
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
    }
}
