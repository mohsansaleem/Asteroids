using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using PG.Core.Contexts.StateManagement;
using PG.Asteroids.Models.MediatorModels;
using PG.Asteroids.Views.GamePlay;
using PG.Asteroids.Commands;
using PG.Core;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;
using Zenject.SpaceFighter;

namespace PG.Asteroids.Contexts.GamePlay
{
    public class GamePlayInstaller : MonoInstaller
    {
        [SerializeField]
        public GamePlayView GamePlayView;

        public override void InstallBindings()
        {
            Container.Bind<GamePlayModel>().AsSingle();
            
            MediatorStateMachineInstaller.Install(Container);
            
            // Simulation
            Container.DeclareSignal<SimulationStartedSignal>();
            SimulationInstaller.Install(Container);
            
            Container.BindInstance(GamePlayView).AsSingle();
            Container.BindInterfacesTo<GamePlayMediator>().AsSingle();
        }
    }
}
