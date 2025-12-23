using Zenject;

namespace PG.Core.Contexts.StateManagement
{
    public class StatesFactory
    {
        private IInstantiator _instantiator;

        public StatesFactory(IInstantiator instantiator) => 
            this._instantiator = instantiator;

        public TState Create<TState>() where TState : IState => 
            _instantiator.Instantiate<TState>();

        public TState Create<TState>(DiContainer container) where TState : IState =>
            container.Instantiate<TState>();
    }
}