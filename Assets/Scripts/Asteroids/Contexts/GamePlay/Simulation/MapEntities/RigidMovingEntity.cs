using System;
using ModestTree;
using UnityEngine;
using Zenject;
using Zenject.Asteroids;

namespace PG.Asteroids.Contexts.GamePlay
{
    public abstract class RigidMovingEntity : MovingEntity
    {
        [Inject] private LevelHelper _level;
        
        protected Rigidbody RigidBody;
        private MovingEntityModel _settings;

        public void Initialize(MovingEntityModel settings)
        {
            Initialize();
            RigidBody = GetComponent<Rigidbody>();
            
            _settings = settings;
            Mass = settings.Mass;
            Scale = settings.Scale;
            Position = settings.Position;
            RigidBody.velocity = settings.Velocity;
        }
        
        public Vector3 Position
        {
            get { return transform.position; }
            set { transform.position = value; }
        }

        public float Mass
        {
            get { return RigidBody.mass; }
            set { RigidBody.mass = value; }
        }

        public float Scale
        {
            get
            {
                var scale = transform.localScale;
                // We assume scale is uniform
                Assert.That(scale[0] == scale[1] && scale[1] == scale[2]);

                return scale[0];
            }
            set
            {
                transform.localScale = new Vector3(value, value, value);
                RigidBody.mass = value;
            }
        }

        public Vector3 Velocity
        {
            get { return RigidBody.velocity; }
            set { RigidBody.velocity = value; }
        }

        public override void FixedTick(float deltaTime)
        {
            base.FixedTick(deltaTime);
            // Limit speed to a maximum
            var speed = RigidBody.velocity.magnitude;

            if (speed > _settings.MaxSpeed)
            {
                var dir = RigidBody.velocity / speed;
                RigidBody.velocity = dir * _settings.MaxSpeed;
            }
        }

        public override void Tick(float deltaTime)
        {
            base.Tick(deltaTime);
            Transform.RotateAround(Transform.position, Vector3.up, 30 * Time.deltaTime);
        }
        
        protected override bool IsMovingInDirection(Vector3 dir)
        {
            return Vector3.Dot(dir, RigidBody.velocity) > 0;
        }

        [Serializable]
        public class  MovingEntityModel
        {
            public float Scale;
            public float Mass;
            public Vector3 Position;
            public Vector3 Velocity;
            public float MaxSpeed;
        }
    }
}
