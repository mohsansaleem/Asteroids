using System;
using ModestTree;
using UnityEngine;
using Zenject;
using Zenject.Asteroids;

namespace PG.Asteroids.Contexts.GamePlay
{
    public abstract class LinearMovingEntity : MovingEntity
    {
        private Vector3 _moveDirection;
        private float _speed;

        public void Initialize(Vector2 movementDirection, float movementSpeed)
        {
            Initialize();
            _moveDirection = movementDirection;
            _speed = movementSpeed;
        }

        public override void FixedTick(float deltaTime)
        {
            base.FixedTick(deltaTime);
            Move(deltaTime);
        }

        protected virtual void Move(float deltaTime)
        {
            transform.position += _moveDirection * _speed * deltaTime;
        }

        protected override bool IsMovingInDirection(Vector3 dir)
        {
            return Vector3.Dot(dir, _moveDirection) > 0;
        }
    }
}
