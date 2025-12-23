using System;
using ModestTree;
using UnityEngine;
using Zenject;

namespace PG.Asteroids.Contexts.GamePlay
{
    public abstract class SimulationEntity : MonoBehaviour, IInitializable
    {
        public Transform Transform;
        
        public virtual void FixedTick(float deltaTime)
        {
           
        }

        public virtual void Tick(float deltaTime)
        {
            
        }

        public abstract void Despawn();

        public virtual void Initialize()
        {
            Transform = transform;
        }
    }
}
