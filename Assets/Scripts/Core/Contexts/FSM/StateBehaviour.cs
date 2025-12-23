using System;
using UniRx;
using UnityEngine;
using Zenject;

namespace PG.Core.Contexts
{
    [Obsolete]
    public abstract class StateBehaviour
    {
        protected CompositeDisposable Disposables;
        protected SignalBus SignalBus;

        public StateBehaviour(SignalBus signalBus)
        {
            SignalBus = signalBus;
        }

        public virtual void OnStateEnter()
        {
            Debug.Log(string.Format("{0} , OnStateEnter()", this));

            Disposables = new CompositeDisposable();
        }

        public virtual void OnStateExit()
        {
            Debug.Log(string.Format("{0} , OnStateExit()", this));

            Disposables.Dispose();
        }

        public virtual bool IsValidOpenState()
        {
            return false;
        }

        public virtual void Tick()
        {
        }
    }
}