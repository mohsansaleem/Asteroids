using Cysharp.Threading.Tasks;

namespace PG.Core.Contexts.StateManagement
{
    public interface IStateMachine
    {
        UniTask Enter<TState>() where TState : class, IState;
        void RegisterState<TState>(TState state) where TState : IState;
    }
}