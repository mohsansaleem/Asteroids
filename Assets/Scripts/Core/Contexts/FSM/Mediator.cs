using System;
using System.Collections.Generic;
using PG.Core.Contexts.StateManagement;
using Cysharp.Threading.Tasks;
using Zenject;

namespace PG.Core.Contexts
{
    public abstract class Mediator : IInitializable, ITickable, IDisposable
    {
        [Inject] protected DiContainer Container;
        [Inject] protected MediatorStateMachine MediatorStateMachine;

        public virtual void Initialize()
		{
        }

        protected void AddState<T>() where T : IState
        {
            MediatorStateMachine.AddState<T>();
        }

        protected async UniTask GoToState<T>() where T : MediatorState
        {
            await MediatorStateMachine.Enter<T>();
        }

        public virtual void Tick()
        {
            MediatorStateMachine.Tick();
        }

        public virtual void Dispose()
        {
        }
    }
}
