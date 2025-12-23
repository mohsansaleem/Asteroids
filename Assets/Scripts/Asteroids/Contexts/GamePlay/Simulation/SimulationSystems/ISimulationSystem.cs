namespace PG.Asteroids.Contexts.GamePlay
{
    public interface ISimulationSystem
    {
        public void Initialize();
        public void Tick(float deltaTime);
        public void FixedTick(float deltaTime);
        public void Reset();
        public void Dispose();
    }
}