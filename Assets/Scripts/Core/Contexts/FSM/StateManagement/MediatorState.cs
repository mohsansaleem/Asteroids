using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using Zenject;

namespace PG.Core.Contexts.StateManagement
{
    public abstract class MediatorState : IState
    {
        protected CompositeDisposable Disposables;
        
        [Inject]
        protected SignalBus SignalBus;

        public virtual void Tick()
        {
        }

        public virtual async UniTask Enter()
        {
            Debug.Log(string.Format("{0} , OnStateEnter()", this));

            Disposables = new CompositeDisposable();
        }

        public virtual async UniTask Exit()
        {
            Debug.Log(string.Format("{0} , OnStateExit()", this));

            Disposables.Dispose();
        }
    }
}