using System;
using System.Collections.Generic;
using PG.Core.Contexts.StateManagement;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using Zenject;

namespace PG.Core.Contexts
{
    public abstract class Mediator : IInitializable, ITickable, IDisposable
    {
        protected CompositeDisposable Disposables;

        [Inject] protected DiContainer Container;
        [Inject] protected SignalBus SignalBus;
        [Inject] protected MediatorStateMachine MediatorStateMachine;
        
        public virtual void Initialize()
		{
			Disposables = new CompositeDisposable();
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
            Disposables.Dispose();
        }
    }
}
