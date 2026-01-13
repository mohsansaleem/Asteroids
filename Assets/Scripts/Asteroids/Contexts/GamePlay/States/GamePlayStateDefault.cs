using Cysharp.Threading.Tasks;
using PG.Asteroids.Models;
using PG.Asteroids.Models.MediatorModels;
using PG.Asteroids.Views.GamePlay;
using UnityEngine;
using Zenject;

namespace PG.Asteroids.Contexts.GamePlay
{
    public class GamePlayStateDefault : GamePlayState
    {
        public override async UniTask Enter()
        {
            await base.Enter();

            GamePlayModel.Lives = StaticDataModel.MetaData.Lives;
            GamePlayModel.Scores = 0;
            GamePlayModel.IsDead = false;

            View.ShowEndGame(false);

            SignalBus.Fire<SimulationStartedSignal>();
        }
    }
}