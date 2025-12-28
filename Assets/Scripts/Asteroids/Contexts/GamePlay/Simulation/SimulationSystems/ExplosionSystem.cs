using PG.Asteroids.Models;
using PG.Asteroids.Models.MediatorModels;
using UnityEngine;
using Zenject;
using Zenject.Asteroids;

namespace PG.Asteroids.Contexts.GamePlay
{
    public class ExplosionSystem: ISimulationSystem
    {
        [Inject] private readonly SimulationModel _simulationModel;
        [Inject] private readonly Explosion.Factory _explosionFactory;
        [Inject] private readonly StaticDataModel _staticDataModel;
        [Inject] private readonly SignalBus _signalBus;
        
        public void Initialize()
        {
        }

        public void Tick(float deltaTime)
        {
            for (int i = 0; i < SimulationModel.MAX_ENTITIES; i++)
            {
                if ((_simulationModel.Masks[i] & EntityMask.Explosion) != 0)
                {
                    _simulationModel.Views[i].Tick(deltaTime);
                }
            }
        }

        public void FixedTick(float fixedDeltaTime)
        {
            for (int i = 0; i < SimulationModel.MAX_ENTITIES; i++)
            {
                if ((_simulationModel.Masks[i] & EntityMask.Explosion) != 0)
                {
                    _simulationModel.Views[i].FixedTick(fixedDeltaTime);
                }
            }
        }

        
        public void Reset()
        {
        }

        public void Dispose()
        {
        }
    }
}