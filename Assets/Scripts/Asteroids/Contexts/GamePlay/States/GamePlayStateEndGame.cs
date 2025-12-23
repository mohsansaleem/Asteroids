using Cysharp.Threading.Tasks;
using PG.Asteroids.Models;
using PG.Asteroids.Models.MediatorModels;
using PG.Asteroids.Views.GamePlay;
using UnityEngine;
using Zenject;

namespace PG.Asteroids.Contexts.GamePlay
{
    public class GamePlayStateEndGame : GamePlayState
    {
        public override async UniTask Enter()
        {
            await base.Enter();

            GamePlayModel.IsDead.Value = true;
            View.EndGameCanvasGroup.alpha = 1;
            View.EndGameCanvasGroup.interactable = true;
        }
    }
}