using System;
using ModestTree;
using UnityEngine;
using Zenject;
using Zenject.Asteroids;

namespace PG.Asteroids.Contexts.GamePlay
{
    public abstract class RigidMovingEntity : MovingEntity
    {
        protected Rigidbody RigidBody;
        private MovingEntityModel _settings;

        protected void Initialize(MovingEntityModel settings)
        {
            Initialize();
            RigidBody = GetComponent<Rigidbody>();

            if (RigidBody == null)
                throw new System.InvalidOperationException($"{GetType().Name} requires a Rigidbody component");

            _settings = settings ?? throw new System.ArgumentNullException(nameof(settings));
            Mass = settings.Mass;
            Scale = settings.Scale;
            Position = settings.Position;
            RigidBody.velocity = settings.Velocity;
        }

        public float Mass
        {
            get { return RigidBody.mass; }
            set { RigidBody.mass = value; }
        }

        public override float Scale
        {
            get => base.Scale;
            set
            {
                base.Scale = value;
                if (RigidBody != null)
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
