using System;
using ModestTree;
using UnityEngine;
using Zenject;
using Zenject.Asteroids;

namespace PG.Asteroids.Contexts.GamePlay
{
    public abstract class MovingEntity : SimulationEntity
    {
        [Inject] protected LevelHelper Level;

        public virtual Vector3 Position
        {
            get { return transform.position; }
            set { transform.position = value; }
        }

        public virtual float Scale
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

        public override void FixedTick(float deltaTime)
        {
            base.FixedTick(deltaTime);
            CheckForTeleport();
        }

        private void CheckForTeleport()
        {
            float radius = Scale / 2;
            if (Position.x > Level.Right + radius && IsMovingInDirection(Vector3.right))
            {
                transform.SetX(Level.Left - radius);
            }
            else if (Position.x < Level.Left - radius && IsMovingInDirection(-Vector3.right))
            {
                transform.SetX(Level.Right + radius);
            }
            else if (Position.y < Level.Bottom - radius && IsMovingInDirection(-Vector3.up))
            {
                transform.SetY(Level.Top + radius);
            }
            else if (Position.y > Level.Top + radius && IsMovingInDirection(Vector3.up))
            {
                transform.SetY(Level.Bottom - radius);
            }
        }

        protected abstract bool IsMovingInDirection(Vector3 dir);
    }
}
