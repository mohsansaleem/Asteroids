using System;
using ModestTree;
using UnityEngine;
using Zenject;

namespace PG.Asteroids.Contexts.GamePlay
{
    public abstract class SimulationEntity : MonoBehaviour, IInitializable
    {
        public IMemoryPool Pool { get; protected set; }

        // Unity lifecycle methods - these call our custom Tick/FixedTick
        private void Update()
        {
            Tick(Time.deltaTime);
        }

        private void FixedUpdate()
        {
            FixedTick(Time.fixedDeltaTime);
        }

        protected virtual void FixedTick(float deltaTime)
        {

        }

        protected virtual void Tick(float deltaTime)
        {

        }

        public abstract void Despawn();

        public virtual void Initialize()
        {
        }
    }
}
