using PG.Core.Installers;
using Zenject;

namespace PG.Core.Commands
{
    public abstract class BaseCommand
    {
        [Inject] protected SignalBus SignalBus;

        protected virtual void PostExecute()
        {
            SignalBus.Fire(new CommandExecutedSignal(){ CommandType = GetType()} );
        }
    }
}