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
            if (_simulationModel.Masks[_id] != EntityMask.None)
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

        public class CommandFactory : PlaceholderFactory<int, IMemoryPool, DestroyEntityCommand>
        {
        }
        
        public class CommandPool : MemoryPool<int, IMemoryPool, IMemoryPool, DestroyEntityCommand>
        {
        }
    }
}