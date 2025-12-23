using Zenject;

namespace PG.Core.Contexts.StateManagement
{
    public class MediatorStateMachine : StateMachine
    {
        private StatesFactory _statesFactory;
        
        [Inject]
        public MediatorStateMachine(StatesFactory statesFactory)
        {
            _statesFactory = statesFactory;
        }

        public void AddState<TState>() where TState : IState
        {
            RegisterState<TState>(_statesFactory.Create<TState>());
        }

        public void Tick()
        {
            (CurrentState as MediatorState)?.Tick();
        }
    }
}