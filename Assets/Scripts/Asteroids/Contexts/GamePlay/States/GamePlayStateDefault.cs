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

            GamePlayModel.Lives.Value = StaticDataModel.MetaData.Lives;
            GamePlayModel.Scores.Value = 0;
            GamePlayModel.IsDead.Value = false;
            
            View.EndGameCanvasGroup.alpha = 0;
            View.EndGameCanvasGroup.interactable = false;
            
            SignalBus.Fire<SimulationStartedSignal>();
        }
    }
}