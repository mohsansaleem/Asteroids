using Zenject;

namespace PG.Asteroids.Contexts.GamePlay
{
    public class DestroyEntityCommand : IEntityCommand, IPoolable<SimulationEntity, IMemoryPool>
    {
        private SimulationEntity _entity;
        private IMemoryPool _commandPool;

        public void OnSpawned(SimulationEntity entity, IMemoryPool commandPool)
        {
            _entity = entity;
            _commandPool = commandPool;
        }

        public void Execute()
        {
            if (_entity != null && _entity.Pool != null)
            {
                _entity.Pool.Despawn(_entity);
            }
            else if (_entity != null)
            {
                UnityEngine.Object.Destroy(_entity.gameObject);
            }

            // Return this command object to its own pool!
            _commandPool.Despawn(this);
        }

        public void OnDespawned()
        {
            _entity = null;
            _commandPool = null;
        }

        public class CommandFactory : PlaceholderFactory<SimulationEntity, DestroyEntityCommand>, ICommandFactory<DestroyEntityCommand>
        {
            public DestroyEntityCommand Create(params object[] args)
            {
                return base.Create(args[0] as SimulationEntity);
            }
        }

        public class CommandPool : MemoryPool<SimulationEntity, IMemoryPool, DestroyEntityCommand>
        {
        }
    }
}
