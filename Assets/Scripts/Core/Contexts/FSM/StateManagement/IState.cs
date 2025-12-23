using Cysharp.Threading.Tasks;

namespace PG.Core.Contexts.StateManagement
{
    public interface IState
    {
        UniTask Enter();
        UniTask Exit();
    }
}