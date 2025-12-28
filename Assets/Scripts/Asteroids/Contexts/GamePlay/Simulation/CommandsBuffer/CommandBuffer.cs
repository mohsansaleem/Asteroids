using System.Collections.Generic;
using PG.Asteroids.Models.MediatorModels;

namespace PG.Asteroids.Contexts.GamePlay
{
    public class CommandBuffer
    {
        private readonly Queue<IEntityCommand> _queue = new Queue<IEntityCommand>();

        public void AddCommand(IEntityCommand cmd) => _queue.Enqueue(cmd);

        public void Playback()
        {
            while (_queue.Count > 0)
            {
                _queue.Dequeue().Execute();
            }
        }
    }
}