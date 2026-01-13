using System;
using Cysharp.Threading.Tasks;
using PG.Asteroids.Models;
using PG.Asteroids.Models.MediatorModels;
using PG.Asteroids.Models.RemoteDataModels;
using PG.Asteroids.Views.GamePlay;
using PG.Core.Contexts;
using UnityEngine;
using Zenject;

namespace PG.Asteroids.Contexts.GamePlay
{
    public class GamePlayMediator : Mediator
    {
        [Inject] private readonly GamePlayView _view;

        [Inject] private readonly GamePlayModel _gamePlayModel;
        [Inject] private readonly RemoteDataModel _remoteDataModel;
        [Inject] private readonly StaticDataModel _staticDataModel;

        [Inject] IInstantiator _instantiator;

        public override void Initialize()
        {
            base.Initialize();

            AddState<GamePlayStateDefault>();
            AddState<GamePlayStateEndGame>();

            // Subscribe to events
            _gamePlayModel.OnScoresChanged += OnScoreChanged;
            _gamePlayModel.OnLivesChanged += OnLivesChanged;

            _view.ButtonRetry.onClick.AddListener(OnRetryClicked);

            // Initialize display with current values
            OnScoreChanged(_gamePlayModel.Scores);
            OnLivesChanged(_gamePlayModel.Lives);

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
            _gamePlayModel.OnScoresChanged -= OnScoreChanged;
            _gamePlayModel.OnLivesChanged -= OnLivesChanged;
            _view.ButtonRetry.onClick.RemoveListener(OnRetryClicked);

            base.Dispose();
        }
    }
}

