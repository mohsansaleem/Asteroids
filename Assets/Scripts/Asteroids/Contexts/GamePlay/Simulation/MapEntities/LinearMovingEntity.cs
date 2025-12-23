using System;
using ModestTree;
using UnityEngine;
using Zenject;
using Zenject.Asteroids;

namespace PG.Asteroids.Contexts.GamePlay
{
    public abstract class LinearMovingEntity : MovingEntity
    {
        [Inject] private LevelHelper _level;
        
        private Vector3 _moveDirection;
        private float _speed;
        
        public void Initialize(Vector2 movementDirection, float movementSpeed)
        {
            Initialize();
            _moveDirection = movementDirection;
            _speed = movementSpeed;
        }
        
        public Vector3 Position
        {
            get { return transform.position; }
            set { transform.position = value; }
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
            }
        }

        public override void Tick(float deltaTime)
        {
        }

        public void Update()
        {
            base.Tick(Time.deltaTime);
            Move(Time.deltaTime);
        }

        protected virtual void Move(float deltaTime)
        {
            Transform.position += _moveDirection * _speed * deltaTime;
        }

        protected override bool IsMovingInDirection(Vector3 dir)
        {
            return Vector3.Dot(dir, _moveDirection) > 0;
        }
    }
}
