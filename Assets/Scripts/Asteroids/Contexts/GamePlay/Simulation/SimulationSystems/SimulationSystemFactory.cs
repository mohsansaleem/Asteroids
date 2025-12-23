using Zenject;

namespace PG.Asteroids.Contexts.GamePlay
{
    public class SimulationSystemFactory
    {
        private IInstantiator _instantiator;

        public SimulationSystemFactory(IInstantiator instantiator)
        {
            this._instantiator = instantiator;
        }

        public TSystem Create<TSystem>() where TSystem : ISimulationSystem
        {
            return _instantiator.Instantiate<TSystem>();
        }

        public TSystem Create<TSystem>(DiContainer container) where TSystem : ISimulationSystem
        {
            return container.Instantiate<TSystem>();
        }
    }
}