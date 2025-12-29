using PG.Asteroids.Models;
using PG.Asteroids.Models.DataModels;
using PG.Asteroids.Models.MediatorModels;
using Zenject;
using Zenject.SpaceFighter;

namespace PG.Asteroids.Contexts.GamePlay
{
    public class ShipCrashedCommand : IEntityCommand, IPoolable<int, IMemoryPool>
    {
        [Inject] private readonly SimulationModel _simulationModel;
        [Inject] private readonly GamePlayModel _gamePlayModel;
        [Inject] private readonly StaticDataModel _staticDataModel;
        [Inject] private readonly AudioPlayer _audioPlayer;
        [Inject] private readonly PlayerShip _player;
        
        private int _id;
        private IMemoryPool _commandPool;

        public void OnSpawned(int id, IMemoryPool commandPool)
        {
            _id = id;
            _commandPool = commandPool;
        }

        public void Execute()
        {
            if (_simulationModel.IsValidEntity(_id))
            {
                ShipData shipData = _staticDataModel.MetaData.ShipData;
                _audioPlayer.Play(shipData.DeathSound, shipData.DeathVolume);

                _gamePlayModel.Lives.Value--;
                if (_gamePlayModel.Lives.Value > 0)
                {
                    _player.AddShield();
                }
            }

            // Return this command object to its own pool!
            _commandPool.Despawn(this);
        }

        public void OnDespawned()
        {
            _id = -1;
            _commandPool = null;
        }

        public class CommandFactory : PlaceholderFactory<int, ShipCrashedCommand>, ICommandFactory<ShipCrashedCommand>
        {
            public ShipCrashedCommand Create(params object[] args)
            {
                return base.Create(args[0] is int id ? id : -1);
            }
        }
        
        public class CommandPool : MemoryPool<int, IMemoryPool, ShipCrashedCommand>
        {
        }
    }
}