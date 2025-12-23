using System;
using PG.Asteroids.Models.MediatorModels;
using PG.Asteroids.Models.RemoteDataModels;
using PG.Asteroids.Views.Startup;
using PG.Core.Contexts;
using PG.Core.Installers;
using UniRx;
using UnityEngine;
using Zenject;

namespace PG.Asteroids.Contexts.Startup
{
    public class StartupMediator : Mediator
    {
        [Inject] private readonly StartupView _view;

        [Inject] private readonly StartupModel _startupModel;
        [Inject] private readonly RemoteDataModel _remoteDataModel;

        public StartupMediator()
        {
            Disposables = new CompositeDisposable();
        }

        public override void Initialize()
        {
            base.Initialize();

            AddState<StartupStateLoadStaticData>();
            AddState<StartupStateLoadAssets>();
            AddState<StartupStateLoadUserData>();
            AddState<StartupStateLoadGamePlay>();
            AddState<StartupStateGamePlay>();

            _startupModel.LoadingProgress.Subscribe(OnLoadingProgressChanged).AddTo(Disposables);
            GoToState<StartupStateLoadStaticData>();
        }

        private void OnLoadingProgressChanged(int loadingProgress)
        {
            _view.ProgressBar.value = (float)loadingProgress / 100;
        }

        private void OnLoadingStart()
        {
            _view.Show();
        }
    }
}

