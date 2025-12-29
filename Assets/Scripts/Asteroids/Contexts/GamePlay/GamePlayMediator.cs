using System;
using Cysharp.Threading.Tasks;
using PG.Asteroids.Models;
using PG.Asteroids.Models.MediatorModels;
using PG.Asteroids.Models.RemoteDataModels;
using PG.Asteroids.Views.GamePlay;
using PG.Core.Contexts;
using PG.Asteroids.Misc;
using UniRx;
using UnityEngine;
using Zenject;

namespace PG.Asteroids.Contexts.GamePlay
{
    public partial class GamePlayMediator : Mediator
    {
        [Inject] private readonly GamePlayView _view;

        [Inject] private readonly GamePlayModel _gamePlayModel;
        [Inject] private readonly RemoteDataModel _remoteDataModel;
        [Inject] private readonly StaticDataModel _staticDataModel;
        
        [Inject] DiContainer _instantiator;

        public override void Initialize()
        {
            base.Initialize();
            
            AddState<GamePlayStateDefault>();
            AddState<GamePlayStateEndGame>();
            
            _gamePlayModel.Scores.Subscribe(OnScoreChanged).AddTo(Disposables);
            _gamePlayModel.Lives.Subscribe(OnLivesChanged).AddTo(Disposables);
            
            _view.ButtonRetry.onClick.AddListener(OnRetryClicked);
            
            GoToState<GamePlayStateDefault>();
        }

        private void OnRetryClicked()
        {
            GoToState<GamePlayStateDefault>();
        }

        private void OnScoreChanged(int score)
        {
            _view.ScoreText.text = $"Scores: {score}";
        }

        private void OnLivesChanged(int lives)
        {
            _view.LivesText.text = $"Lives: {lives}";
            if (lives == 0)
            {
                GoToState<GamePlayStateEndGame>().Forget();
            }
        }
        
        public override void Dispose()
        {
            base.Dispose();
            
            _view.ButtonRetry.onClick.RemoveListener(OnRetryClicked);
        }
    }
}

