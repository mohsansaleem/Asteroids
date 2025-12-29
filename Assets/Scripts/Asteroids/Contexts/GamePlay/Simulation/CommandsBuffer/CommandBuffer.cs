using System.Collections.Generic;
using PG.Asteroids.Models.MediatorModels;
using Zenject;

namespace PG.Asteroids.Contexts.GamePlay
{
    public class CommandBuffer
    {
        [Inject] private readonly DiContainer _container;

        private readonly Queue<IEntityCommand> _queue = new Queue<IEntityCommand>();

        public void AddCommand(IEntityCommand cmd) => _queue.Enqueue(cmd);

        /// <summary>
        /// Generic enqueue method that resolves the factory and creates the command
        /// </summary>
        public void Enqueue<TCommand>(params object[] args)
            where TCommand : IEntityCommand
        {
            // Resolve the factory for this command type
            var factory = _container.Resolve<ICommandFactory<TCommand>>();
            var cmd = factory.Create(args);
            AddCommand(cmd);
        }

        public void Playback()
        {
            while (_queue.Count > 0)
            {
                _queue.Dequeue().Execute();
            }
        }
    }

    /// <summary>
    /// Generic command factory interface
    /// </summary>
    public interface ICommandFactory<TCommand> where TCommand : IEntityCommand
    {
        TCommand Create(params object[] args);
    }

    /// <summary>
    /// Non-generic command factory interface (legacy support)
    /// </summary>
    public interface ICommandFactory
    {
        IEntityCommand Create(params object[] args);
    }
}