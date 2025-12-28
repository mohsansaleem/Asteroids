using System;
using System.Collections.Generic;
using PG.Asteroids.Models;
using PG.Asteroids.Models.DataModels;
using PG.Asteroids.Models.MediatorModels;
using UnityEngine;
using Zenject;
using Zenject.SpaceFighter;
using PlayerInputState = PG.Asteroids.Models.MediatorModels.PlayerInputState;

namespace PG.Asteroids.Contexts.GamePlay
{
    public class ShipControlSystem : ISimulationSystem
    {
        [Inject] private readonly SignalBus _signalBus;
        [Inject] private readonly StaticDataModel _staticDataModel;
        [Inject] private readonly GamePlayModel _gamePlayModel;
        [Inject] private readonly SimulationModel _simulationModel;
        [Inject] private readonly AudioPlayer _audioPlayer;
        [Inject] private readonly PlayerShip _player;
        [Inject] private readonly CommandBufferMediator _commandBufferMediator;

        private float _lastFireTime;

        public void Initialize()
        {
            int entityId = _simulationModel.Register(_player, EntityMask.Movable | EntityMask.PlayerShip);
            _player.EntityId = entityId;
            
            _player.ApplyDrag();
        }

        public void Tick(float deltaTime)
        {
            if (_gamePlayModel.IsDead.Value)
            {
                return;
            }

            PlayerInputState inputState = _simulationModel.PlayerInputState;

            if (inputState.IsFiring && Time.realtimeSinceStartup - _lastFireTime > _staticDataModel.MetaData.BulletSettings.MaxShootInterval)
            {
                _lastFireTime = Time.realtimeSinceStartup;
                Fire();
            }
        }

        void Fire()
        {
            BulletSettings bulletSettings = _staticDataModel.MetaData.BulletSettings;
            Vector3 position = _player.Position + _player.transform.up * bulletSettings.BulletOffsetDistance;
            var direction = _player.transform.up.normalized;
            Quaternion rotation = _player.transform.rotation;
            
            _commandBufferMediator.RequestSpawnRocket(position, direction, rotation);
        }

        public void FixedTick(float fixedDeltaTime)
        {
            if (_gamePlayModel.IsDead.Value)
            {
                return;
            }

            PlayerInputState inputState = _simulationModel.PlayerInputState;

            float thrustInput = 0f;
            if (inputState.IsMovingUp)
                thrustInput = 0.3f;
            else if (inputState.IsSlowingDown)
                thrustInput = -0.3f;

            if (thrustInput != 0f)
                _player.ApplyThrust(thrustInput);

            float rotationInput = 0f;
            if (inputState.IsRotatingLeft)
                rotationInput = -0.4f;
            else if (inputState.IsRotatingRight)
                rotationInput = 0.4f;

            if (rotationInput != 0f)
                _player.ApplyRotation(rotationInput);
        }

        public void Reset()
        {
            _player.ResetShip();
        }
        
        public void Dispose()
        {
        }
    }
}