using PG.Asteroids.Contexts.GamePlay;
using PG.Asteroids.Models;
using PG.Asteroids.Models.DataModels;
using PG.Asteroids.Models.MediatorModels;
using UnityEngine;
using Zenject;

namespace PG.Asteroids.Contexts.GamePlay
{
    public class SpawnRocketCommand : IEntityCommand, IPoolable<Vector3, Vector3, Quaternion, IMemoryPool>
    {
        [Inject] private readonly StaticDataModel _staticDataModel;
        [Inject] private readonly SimulationModel _simulationModel;
        [Inject] private readonly Rocket.Factory _rocketFactory;
        
        private Vector3 _position;
        private Vector3 _moveDirection;
        private Quaternion _rotation;
        private IMemoryPool _commandPool;

        public void OnSpawned(Vector3 position, Vector3 moveDirection, Quaternion rotation, IMemoryPool commandPool)
        {
            _position = position;
            _moveDirection = moveDirection;
            _rotation = rotation;
            _commandPool = commandPool;
        }

        public void Execute()
        {
            BulletSettings bulletSettings = _staticDataModel.MetaData.BulletSettings;

            Rocket rocket = _rocketFactory.Create(bulletSettings.BulletLifetime, _moveDirection, bulletSettings.BulletSpeed);

            rocket.transform.position = _position;
            rocket.transform.rotation = _rotation;
            
            int entityId = _simulationModel.Register(rocket, EntityMask.Movable | EntityMask.Explosive);
            if (entityId != -1)
                rocket.EntityId = entityId;
            else
                Debug.LogError($"Unable to register rocket entity in simulation model.");

            // Return this command object to its own pool!
            _commandPool.Despawn(this);
        }

        public void OnDespawned()
        {
            _position = Vector3.zero;
            _commandPool = null;
        }

        public class CommandFactory : PlaceholderFactory<Vector3, Vector3, Quaternion, SpawnRocketCommand>
        {
        }
        
        public class CommandPool : MemoryPool<Vector3, Vector3, Quaternion, IMemoryPool, SpawnRocketCommand>
        {
        }
    }
}