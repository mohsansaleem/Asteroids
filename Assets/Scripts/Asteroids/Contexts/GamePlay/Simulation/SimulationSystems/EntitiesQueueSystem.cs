using System.Collections.Generic;
using PG.Asteroids.Models;
using PG.Asteroids.Models.MediatorModels;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;
using Zenject.Asteroids;

namespace PG.Asteroids.Contexts.GamePlay
{
    public class EntitiesQueueSystem: ISimulationSystem
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
            if (_simulationModel.SimulationEntitiesQueue.Count > 0)
            {
                _simulationModel.SimulationEntities.AddRange(_simulationModel.SimulationEntitiesQueue);
                _simulationModel.SimulationEntitiesQueue.Clear();
            }

            for (var index = 0; index < _simulationModel.SimulationEntitiesExpired.Count; index++)
            {
                var entity = _simulationModel.SimulationEntitiesExpired[index];
                _simulationModel.SimulationEntities.Remove(entity);
                entity.Despawn();
            }
            _simulationModel.SimulationEntitiesExpired.Clear();
        }

        public void FixedTick(float deltaTime)
        {
            
        }

        public void Reset()
        {
            
        }

        public void Dispose()
        {
        }
    }
}