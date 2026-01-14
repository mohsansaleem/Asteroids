using PG.Asteroids.Models;
using PG.Asteroids.Models.DataModels;
using PG.Asteroids.Models.SimulationModels;
using UnityEngine;
using Zenject;
using Zenject.Asteroids;

namespace PG.Asteroids.Contexts.GamePlay
{
    /// <summary>
    /// Continuously maintains a target number of asteroids on screen.
    /// Spawns new asteroids whenever count drops below max.
    /// This is the original spawning behavior.
    /// </summary>
    public class ContinuousSpawnStrategy : IAsteroidSpawnStrategy
    {
        [Inject] private readonly CommandBufferMediator _commandBufferMediator;
        [Inject] private readonly StaticDataModel _staticDataModel;
        [Inject] private readonly SimulationModel _simulationModel;
        [Inject] private readonly LevelHelper _level;

        public void InitializeSpawns()
        {
            // Spawn initial asteroids up to StartingSpawns count
            int startingCount = _staticDataModel.MetaData.AsteroidsData.StartingSpawns;
            for (int i = _simulationModel.AsteroidsCount; i < startingCount; i++)
            {
                SpawnRandomAsteroid();
            }
        }

        public void UpdateSpawns(float deltaTime)
        {
            // Maintain MaxSpawns asteroids continuously
            int maxSpawns = _staticDataModel.MetaData.AsteroidsData.MaxSpawns;
            for (int i = _simulationModel.AsteroidsCount; i < maxSpawns; i++)
            {
                SpawnRandomAsteroid();
            }
        }

        public void Reset()
        {
            // No state to reset for continuous strategy
        }

        private void SpawnRandomAsteroid()
        {
            AsteroidsData settings = _staticDataModel.MetaData.AsteroidsData;
            int levelIndex = Random.Range(0, settings.AsteroidLevels.Length);

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
