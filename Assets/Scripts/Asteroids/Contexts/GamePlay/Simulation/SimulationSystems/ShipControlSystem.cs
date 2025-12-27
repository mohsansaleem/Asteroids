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
        [Inject] private readonly Rocket.Factory _rocketFactory;

        private float _lastFireTime;

        public void Initialize()
        {
            _simulationModel.SimulationEntitiesQueue.Add(_player);
            _signalBus.Subscribe<PlayerCrashedSignal>(OnShipCrashed);
        }

        private void OnShipCrashed(PlayerCrashedSignal signal)
        {
            ShipData shipData = _staticDataModel.MetaData.ShipData;
            _audioPlayer.Play(shipData.DeathSound, shipData.DeathVolume);
            
            _gamePlayModel.Lives.Value--;
            if (_gamePlayModel.Lives.Value > 0)
            {
                _player.AddShield();
            }
        }

        public void Tick(float deltaTime)
        {
            if (_gamePlayModel.IsDead.Value)
            {
                return;
            }

            PlayerInputState inputState = _simulationModel.PlayerInputState;

            if (inputState.IsMovingUp)
                _player.Thrust(0.3f);
            else if (inputState.IsSlowingDown)
                _player.Thrust(-0.3f);


            if (inputState.IsRotatingLeft)
                _player.Rotate(-0.4f);
            else if (inputState.IsRotatingRight)
                _player.Rotate(0.4f);

            if (inputState.IsFiring && Time.realtimeSinceStartup - _lastFireTime > _staticDataModel.MetaData.BulletSettings.MaxShootInterval)
            {
                _lastFireTime = Time.realtimeSinceStartup;
                Fire();
            }
        }

        void Fire()
        {
            BulletSettings bulletSettings = _staticDataModel.MetaData.BulletSettings;

            Rocket bullet = _rocketFactory.Create(bulletSettings.BulletLifetime, _player.Transform.up.normalized, bulletSettings.BulletSpeed);

            bullet.transform.position = _player.Position + _player.transform.up * bulletSettings.BulletOffsetDistance;
            bullet.transform.rotation = _player.Transform.rotation;

            _simulationModel.SimulationEntitiesQueue.Add(bullet);
        }

        public void FixedTick(float deltaTime)
        {
        }

        public void Reset()
        {
            _player.ResetShip();
        }
        
        public void Dispose()
        {
            _signalBus.Unsubscribe<PlayerCrashedSignal>(OnShipCrashed);
        }
    }
}