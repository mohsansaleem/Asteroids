using System;
using ModestTree;
using UnityEngine;
using Zenject;
using Zenject.Asteroids;

namespace PG.Asteroids.Contexts.GamePlay
{
    public abstract class MovingEntity : SimulationEntity
    {
        [Inject] private LevelHelper _level;
        
        public Vector3 Position
        {
            get { return Transform.position; }
            set { Transform.position = value; }
        }

        public virtual float Scale
        {
            get
            {
                var scale = Transform.localScale;
                // We assume scale is uniform
                Assert.That(scale[0] == scale[1] && scale[1] == scale[2]);

                return scale[0];
            }
            set
            {
                Transform.localScale = new Vector3(value, value, value);
            }
        }

        public override void Tick(float deltaTime)
        {
            base.Tick(deltaTime);
            CheckForTeleport();
        }

        private void CheckForTeleport()
        {
            float radius = Scale / 2;
            if (Position.x > _level.Right + radius && IsMovingInDirection(Vector3.right))
            {
                Transform.SetX(_level.Left - radius);
            }
            else if (Position.x < _level.Left - radius && IsMovingInDirection(-Vector3.right))
            {
                Transform.SetX(_level.Right + radius);
            }
            else if (Position.y < _level.Bottom - radius && IsMovingInDirection(-Vector3.up))
            {
                Transform.SetY(_level.Top + radius);
            }
            else if (Position.y > _level.Top + radius && IsMovingInDirection(Vector3.up))
            {
                Transform.SetY(_level.Bottom - radius);
            }
        }

        protected abstract bool IsMovingInDirection(Vector3 dir);
    }
}
