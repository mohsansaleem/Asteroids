using Cysharp.Threading.Tasks;
using PG.Asteroids.Models;
using PG.Asteroids.Models.MediatorModels;
using PG.Asteroids.Views.GamePlay;
using PG.Core.Contexts;
using PG.Core.Contexts.StateManagement;
using Zenject;

namespace PG.Asteroids.Contexts.GamePlay
{
    public abstract class GamePlayState : MediatorState
    {
        [Inject]
        protected readonly StaticDataModel StaticDataModel;
        [Inject]
        protected readonly GamePlayModel GamePlayModel;
        [Inject]
        protected readonly GamePlayView View;
    }
}