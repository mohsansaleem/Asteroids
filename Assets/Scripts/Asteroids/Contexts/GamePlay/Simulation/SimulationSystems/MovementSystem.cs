using PG.Asteroids.Models.MediatorModels;
using UnityEngine;
using Zenject;

namespace PG.Asteroids.Contexts.GamePlay
{
    public class MovementSystem: ISimulationSystem
    {
        [Inject] private SimulationModel _simulationModel;
        
        public void Initialize()
        {
        }

        public void Tick(float deltaTime)
        {
            for (int i = 0; i < SimulationModel.MAX_ENTITIES; i++)
            {
                if ((_simulationModel.Masks[i] & EntityMask.Movable) != 0)
                {
                    _simulationModel.Views[i].Tick(deltaTime);
                }
            }
        }

        public void FixedTick(float fixedDeltaTime)
        {
            for (int i = 0; i < SimulationModel.MAX_ENTITIES; i++)
            {
                if ((_simulationModel.Masks[i] & EntityMask.Movable) != 0)
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