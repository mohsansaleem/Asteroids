using System;
using System.Collections.Generic;
using PG.Core.Contexts.StateManagement;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using Zenject;

namespace PG.Asteroids.Contexts.GamePlay
{
    public class Simulation : IInitializable, ITickable, IFixedTickable, IDisposable
    {
        private CompositeDisposable _disposables;

        [Inject] private DiContainer _container;
        [Inject] private SignalBus _signalBus;
        [Inject] private MediatorStateMachine _mediatorStateMachine;
        [Inject] private SimulationSystemFactory _simulationSystemFactory;
        [Inject] private List<ISimulationSystem> _simulationSystems;
        [Inject] private CommandBufferMediator _commandBufferMediator;
        
        public virtual void Initialize()
		{
			_disposables = new CompositeDisposable();
            
            _signalBus.Subscribe<SimulationStartedSignal>(OnSimulationStarted);
            
            foreach (var simulationSystem in _simulationSystems)
                simulationSystem.Initialize();
        }

        public virtual void Tick()
        {
            float deltaTime = Time.deltaTime;
            foreach (var simulationSystem in _simulationSystems)
                simulationSystem.Tick(deltaTime);
            
            _commandBufferMediator.Playback();
        }
        
        public void FixedTick()
        {
            foreach (var simulationSystem in _simulationSystems)
                simulationSystem.FixedTick(Time.fixedDeltaTime);
        }
        
        private void OnSimulationStarted(SimulationStartedSignal signal)
        {
            foreach (var simulationSystem in _simulationSystems)
                simulationSystem.Reset();
        }

        public virtual void Dispose()
        {
            _disposables.Dispose();
            
            _signalBus.Unsubscribe<SimulationStartedSignal>(OnSimulationStarted);
            
            foreach (var simulationSystem in _simulationSystems)
                simulationSystem.Dispose();
        }
    }
}
