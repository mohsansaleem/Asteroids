using Zenject;

namespace PG.Asteroids.Contexts.GamePlay
{
    /// <summary>
    /// System responsible for managing asteroid spawning using a pluggable strategy.
    /// The spawning behavior is determined by the injected IAsteroidSpawnStrategy implementation.
    /// </summary>
    public class AsteroidsSystem : ISimulationSystem
    {
        [Inject] private readonly IAsteroidSpawnStrategy _spawnStrategy;

        public void Initialize()
        {
            _spawnStrategy.InitializeSpawns();
        }

        public void Tick(float deltaTime)
        {
            _spawnStrategy.UpdateSpawns(deltaTime);
        }

        public void FixedTick(float fixedDeltaTime)
        {
            // No fixed update logic needed
        }

        public void Reset()
        {
            _spawnStrategy.Reset();
        }

        public void Dispose()
        {
            // No cleanup needed
        }
    }
}
