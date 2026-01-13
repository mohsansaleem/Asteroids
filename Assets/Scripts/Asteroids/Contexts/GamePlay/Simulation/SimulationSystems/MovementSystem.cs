using Zenject;

namespace PG.Asteroids.Contexts.GamePlay
{
    /// <summary>
    /// MovementSystem is no longer needed - entities handle their own movement through Unity's Update/FixedUpdate
    /// Kept for compatibility with existing installer bindings
    /// </summary>
    public class MovementSystem: ISimulationSystem
    {
        public void Initialize()
        {
        }

        public void Tick(float deltaTime)
        {
            // Entities now handle their own Tick through MonoBehaviour Update()
        }

        public void FixedTick(float fixedDeltaTime)
        {
            // Entities now handle their own FixedTick through MonoBehaviour FixedUpdate()
        }

        public void Reset()
        {

        }

        public void Dispose()
        {

        }
    }
}