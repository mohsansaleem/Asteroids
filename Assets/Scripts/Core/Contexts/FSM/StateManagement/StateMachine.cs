using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace PG.Core.Contexts.StateManagement
{
    public abstract class StateMachine : IStateMachine
    {
        private readonly Dictionary<System.Type, IState> _registeredStates = new();
        protected IState CurrentState;

        public async UniTask Enter<TState>() where TState : class, IState
        {
            TState newState = await ChangeState<TState>();
            await newState.Enter();
        }

        public void RegisterState<TState>(TState state) where TState : IState
        {
            _registeredStates.Add(typeof(TState), state);
        }

        protected void ClearStates()
        {
            _registeredStates.Clear();
        }
        
        private async UniTask<TState> ChangeState<TState>() where TState : class, IState
        {
            if(CurrentState != null)
                await CurrentState.Exit();
      
            TState state = GetState<TState>();
            CurrentState = state;
      
            return state;
        }
    
        private TState GetState<TState>() where TState : class, IState
        {
            return _registeredStates[typeof(TState)] as TState;
        }
    }
}