using PG.Asteroids.Models.MediatorModels;
using Zenject;

namespace PG.Asteroids.Contexts.GamePlay
{
    public class DestroyEntityCommand : IEntityCommand, IPoolable<int, IMemoryPool, IMemoryPool>
    {
        [Inject] private readonly SimulationModel _simulationModel;
        
        private int _id;
        private IMemoryPool _entityPool;
        private IMemoryPool _commandPool;

        public void OnSpawned(int id, IMemoryPool entityPool, IMemoryPool commandPool)
        {
            _id = id;
            _entityPool = entityPool;
            _commandPool = commandPool;
        }

        public void Execute()
        {
            if (_simulationModel.IsValidEntity(_id))
            {
                var view = _simulationModel.Views[_id];
                _simulationModel.Unregister(_id);
                _entityPool.Despawn(view);
            }

            // Return this command object to its own pool!
            _commandPool.Despawn(this);
        }

        public void OnDespawned()
        {
            _id = -1;
            _entityPool = null;
            _commandPool = null;
        }

        public class CommandFactory : PlaceholderFactory<int, IMemoryPool, DestroyEntityCommand>, ICommandFactory<DestroyEntityCommand>
        {
            public DestroyEntityCommand Create(params object[] args)
            {
                return base.Create(args[0] is int id ? id : -1, args[1] is IMemoryPool entityPool ? entityPool : null);
            }
        }
        
        public class CommandPool : MemoryPool<int, IMemoryPool, IMemoryPool, DestroyEntityCommand>
        {
        }
    }
}