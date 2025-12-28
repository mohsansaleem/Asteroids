using PG.Asteroids.Models;
using PG.Asteroids.Models.DataModels;
using PG.Asteroids.Models.MediatorModels;
using UnityEngine;
using Zenject;

namespace PG.Asteroids.Contexts.GamePlay
{
    public class PlayerShip : RigidMovingEntity
    {
        [SerializeField] GameObject _sheildGameObject;
        [SerializeField] MeshRenderer _meshRenderer;
        [SerializeField] Rigidbody _rigidbody;

#if UNITY_2018_1_OR_NEWER
        [SerializeField] ParticleSystem _particleSystem;
#else
        [SerializeField]
        ParticleEmitter _particleEmitter;
#endif

        [Inject] private GamePlayModel _gamePlayModel;
        [Inject] private StaticDataModel _staticDataModel;
        [Inject] private CommandBufferMediator _commandBufferMediator;

        private float _shieldDuration;

        public Rigidbody Rigidbody => _rigidbody;
        public bool HasShield => _shieldDuration > 0;

        public void Initialize()
        {
            var settings = new MovingEntityModel()
            {
                Mass = 1f,
                Scale = 1,
                Velocity = Vector3.zero,
                Position = Vector3.zero,
                MaxSpeed = _staticDataModel.MetaData.ShipData.ShipMaxMovementSpeed
            };
            base.Initialize(settings);
        }

        public void OnTriggerEnter(Collider other)
        {
            if (_shieldDuration > 0 || _gamePlayModel.IsDead.Value)
                return;

            if (other.CompareTag("asteroid"))
            {
                var asteroid = other.GetComponent<Asteroid>();
                if (asteroid != null)
                {
                    _commandBufferMediator.RequestSpawnExplosion(_staticDataModel.MetaData.ExplosionSettings.ExplosionTimeout, Position);
                    _commandBufferMediator.RequestShipCrash(EntityId);
                }
            }
        }

        public override void Tick(float deltaTime)
        {
            base.Tick(deltaTime);

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
            if (Pool == null)
                throw new System.InvalidOperationException($"{nameof(PlayerShip)} pool is null - entity was not properly spawned");

            Pool.Despawn(this);
        }

        public void AddShield()
        {
            _shieldDuration = _staticDataModel.MetaData.ShipData.ShipShieldDuration;
            _sheildGameObject.SetActive(true);
        }
        
        public void ApplyThrust(float thrustInput)
        {
            if (_rigidbody == null) return;

            ShipData shipData = _staticDataModel.MetaData.ShipData;
            Vector3 thrustDirection = transform.up;
            float thrustForce = thrustInput * shipData.ShipMovementAcceleration;

            _rigidbody.AddForce(thrustDirection * thrustForce, ForceMode.Force);

            // Clamp velocity to max speed
            if (_rigidbody.velocity.magnitude > shipData.ShipMaxMovementSpeed)
            {
                _rigidbody.velocity = _rigidbody.velocity.normalized * shipData.ShipMaxMovementSpeed;
            }
        }

        public void ApplyRotation(float rotationInput)
        {
            if (_rigidbody == null) return;

            ShipData shipData = _staticDataModel.MetaData.ShipData;
            Vector3 torque = -Vector3.forward * rotationInput * shipData.ShipRotationSpeed;
            _rigidbody.AddTorque(torque, ForceMode.Force);
        }

        public void ApplyDrag()
        {
            if (_rigidbody == null) return;

            ShipData shipData = _staticDataModel.MetaData.ShipData;
            _rigidbody.drag = shipData.ShipDeacceleration;
        }

        public void ResetShip()
        {
            if (_rigidbody != null)
            {
                _rigidbody.velocity = Vector3.zero;
                _rigidbody.angularVelocity = Vector3.zero;
            }
            AddShield();
            transform.position = Vector3.zero;
            transform.rotation = Quaternion.identity;
        }
        
        void UpdateThruster()
        {
            float speed = _rigidbody != null ? _rigidbody.velocity.magnitude : 0;
            var speedPx = Mathf.Clamp(speed / _staticDataModel.MetaData.ShipData.SpeedForMaxEmisssion, 0.0f, 1.0f);

#if UNITY_2018_1_OR_NEWER
            var emission = _particleSystem.emission;
            emission.rateOverTime = _staticDataModel.MetaData.ShipData.MaxEmission * speedPx;
#else
            _particleEmitter.maxEmission = _staticDataModel.MetaData.ShipData.MaxEmission * speedPx;
#endif
        }

        public void OnSpawned()
        {
            Initialize();
            AddShield();
        }

        public void OnDespawned()
        {
            Pool = null;
        }

        public class Factory : PlaceholderFactory<PlayerShip>
        {
            public override PlayerShip Create()
            {
                var instance = base.Create();
                instance.OnSpawned();
                return instance;
            }
        }
    }
}