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
            for (var index = 0; index < _simulationModel.SimulationEntities.Count; index++)
            {
                var simulationEntity = _simulationModel.SimulationEntities[index];
                if (simulationEntity is MovingEntity movingEntity)
                {
                    movingEntity.Tick(deltaTime);
                }
            }
        }

        public void FixedTick(float deltaTime)
        {
            for (var index = 0; index < _simulationModel.SimulationEntities.Count; index++)
            {
                var simulationEntity = _simulationModel.SimulationEntities[index];
                if (simulationEntity is MovingEntity movingEntity)
                {
                    movingEntity.FixedTick(deltaTime);
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