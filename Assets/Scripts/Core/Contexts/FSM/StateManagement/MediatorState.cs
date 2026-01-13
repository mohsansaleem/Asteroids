using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace PG.Core.Contexts.StateManagement
{
    public abstract class MediatorState : IState
    {
        [Inject] protected SignalBus SignalBus;

        public virtual void Tick()
        {
        }

        public virtual async UniTask Enter()
        {
            Debug.Log(string.Format("{0} , OnStateEnter()", this));
        }

        public virtual async UniTask Exit()
        {
            Debug.Log(string.Format("{0} , OnStateExit()", this));
        }
    }
}