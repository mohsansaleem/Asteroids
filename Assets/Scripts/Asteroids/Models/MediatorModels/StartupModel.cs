using System;
using UniRx;

namespace PG.Asteroids.Models.MediatorModels
{
    public class StartupModel
    {
        public ReactiveProperty<int> LoadingProgress;

        public StartupModel()
        {
            LoadingProgress = new ReactiveProperty<int>();
        }
    }
}

