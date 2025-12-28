using System;
using ModestTree;
using UnityEngine;
using Zenject;

namespace PG.Asteroids.Contexts.GamePlay
{
    public abstract class SimulationEntity : MonoBehaviour, IInitializable
    {
        public int EntityId { get; set; }
        public IMemoryPool Pool { get; protected set; }

        public virtual void FixedTick(float deltaTime)
        {

        }

        public virtual void Tick(float deltaTime)
        {

        }

        public abstract void Despawn();

        public virtual void Initialize()
        {
        }
    }
}
