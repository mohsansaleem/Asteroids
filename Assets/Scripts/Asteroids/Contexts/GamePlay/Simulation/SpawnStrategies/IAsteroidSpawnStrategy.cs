namespace PG.Asteroids.Contexts.GamePlay
{
    /// <summary>
    /// Strategy interface for different asteroid spawning behaviors.
    /// Implementations define how and when asteroids are spawned during gameplay.
    /// </summary>
    public interface IAsteroidSpawnStrategy
    {
        /// <summary>
        /// Called during system initialization to spawn initial asteroids.
        /// </summary>
        void InitializeSpawns();

        /// <summary>
        /// Called each frame to determine if new asteroids should spawn.
        /// </summary>
        /// <param name="deltaTime">Time since last frame</param>
        void UpdateSpawns(float deltaTime);

        /// <summary>
        /// Resets the strategy state (called when game restarts).
        /// </summary>
        void Reset();
    }
}
