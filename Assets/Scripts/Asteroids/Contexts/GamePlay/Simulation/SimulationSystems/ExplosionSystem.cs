using Zenject;

namespace PG.Asteroids.Contexts.GamePlay
{
    /// <summary>
    /// ExplosionSystem is no longer needed - explosions handle their own lifecycle through MonoBehaviour Update()
    /// Kept for compatibility with existing installer bindings
    /// </summary>
    public class ExplosionSystem: ISimulationSystem
    {
        public void Initialize()
        {
        }

        public void Tick(float deltaTime)
        {
            // Explosions now handle their own Tick through MonoBehaviour Update()
        }

        public void FixedTick(float fixedDeltaTime)
        {
            // Explosions now handle their own FixedTick through MonoBehaviour FixedUpdate()
        }


        public void Reset()
        {
        }

        public void Dispose()
        {
        }
    }
}