using PG.Asteroids.Models;
using PG.Asteroids.Models.DataModels;
using PG.Asteroids.Models.MediatorModels;
using UnityEngine;
using Zenject;

namespace PG.Asteroids.Contexts.GamePlay
{
    public class PlayerShip : MovingEntity
    {
        [SerializeField] GameObject _sheildGameObject;
        [SerializeField] MeshRenderer _meshRenderer;

#if UNITY_2018_1_OR_NEWER
        [SerializeField] ParticleSystem _particleSystem;
#else
        [SerializeField]
        ParticleEmitter _particleEmitter;
#endif

        [Inject] private GamePlayModel _gamePlayModel;
        [Inject] private StaticDataModel _staticDataModel;
        [Inject] private SignalBus _signalBus;
        
        private float _shieldDuration;
        private float _movementSpeed;
        private Vector3 _movementDirection;
        private float _rotationSpeed;
        private Vector3 _lastPosition;
        IMemoryPool _pool;

        public void OnTriggerEnter(Collider other)
        {
            if (_shieldDuration > 0 || _gamePlayModel.IsDead.Value)
                return;
            
            if (other.CompareTag("asteroid"))
            {
                Asteroid asteroid = other.GetComponent<Asteroid>();
                _signalBus.Fire<PlayerCrashedSignal>(new PlayerCrashedSignal(asteroid, this));
            }
        }

        public override void Tick(float deltaTime)
        {
            base.Tick(deltaTime);
            
            Move(deltaTime);
            UpdateThruster();

            if (_shieldDuration > 0)
            {
                _shieldDuration -= deltaTime;
                if (_shieldDuration <= 0)
                    _sheildGameObject.SetActive(false);
            }
        }

        public override void Despawn()
        {
            _pool?.Despawn(this);
        }

        private void Move(float deltaTime)
        {
            _lastPosition = Position;
            _movementDirection += Transform.up * _movementSpeed * deltaTime;
            _movementDirection = new Vector3(_movementDirection.x, _movementDirection.y, 0);
            transform.position += _movementDirection * deltaTime;
            transform.Rotate(-Vector3.forward, _rotationSpeed * deltaTime);
            _rotationSpeed = 0;
            
            ShipData shipData = _staticDataModel.MetaData.ShipData;
            float movementMagnitude = Mathf.Clamp(_movementDirection.magnitude - (shipData.ShipDeacceleration * deltaTime), 0, shipData.ShipMaxMovementSpeed);
            _movementDirection = _movementDirection.normalized * movementMagnitude;
            
            if (_movementSpeed > 0)
            {
                _movementSpeed -= shipData.ShipDeacceleration;
            }
            else
            {
                _movementSpeed = 0;
            }
        }

        public void AddShield()
        {
            _shieldDuration = _staticDataModel.MetaData.ShipData.ShipShieldDuration;
            _sheildGameObject.SetActive(true);
        }
        
        public void Thrust(float amount)
        {
            if (_movementSpeed < _staticDataModel.MetaData.ShipData.ShipMaxMovementSpeed)
            {
                _movementSpeed += amount * _staticDataModel.MetaData.ShipData.ShipMovementAcceleration;
                if (_movementSpeed < 0)
                    _movementSpeed = 0;
            }
        }

        public void Rotate(float delta)
        {
            _rotationSpeed += delta * _staticDataModel.MetaData.ShipData.ShipRotationSpeed;
        }

        public void ResetShip()
        {
            _movementSpeed = 0;
            _rotationSpeed = 0;
            AddShield();
            Transform.position = Vector3.zero;
            Transform.rotation = Quaternion.identity;
        }
        
        void UpdateThruster()
        {
            var speed = (Transform.position - _lastPosition).magnitude / Time.deltaTime;
            var speedPx = Mathf.Clamp(speed / _staticDataModel.MetaData.ShipData.SpeedForMaxEmisssion, 0.0f, 1.0f);

#if UNITY_2018_1_OR_NEWER
            var emission = _particleSystem.emission;
            emission.rateOverTime = _staticDataModel.MetaData.ShipData.MaxEmission * speedPx;
#else
            _particleEmitter.maxEmission = _staticDataModel.MetaData.ShipData.MaxEmission * speedPx;
#endif
        }

        protected override bool IsMovingInDirection(Vector3 dir)
        {
            return Vector3.Dot(dir, _movementDirection) > 0;
        }

        public void OnSpawned(IMemoryPool pool)
        {
            _pool = pool;
            AddShield();
        }

        public void OnDespawned()
        {
            _pool = null;
        }

        public class Factory : PlaceholderFactory<PlayerShip>
        {
        }
    }
}