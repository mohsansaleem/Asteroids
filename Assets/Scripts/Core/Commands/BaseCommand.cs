using Cysharp.Threading.Tasks;

namespace PG.Core.Commands
{
    public abstract class BaseCommand<TParams> where TParams : class
    {
        public abstract UniTask Execute(TParams parameters);
    }
}