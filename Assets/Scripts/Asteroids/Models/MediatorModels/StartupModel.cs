using System;

namespace PG.Asteroids.Models.MediatorModels
{
    public class StartupModel
    {
        private int _loadingProgress;

        public event Action<int> OnLoadingProgressChanged;

        public int LoadingProgress
        {
            get => _loadingProgress;
            set
            {
                if (_loadingProgress != value)
                {
                    _loadingProgress = value;
                    OnLoadingProgressChanged?.Invoke(_loadingProgress);
                }
            }
        }

        public StartupModel()
        {
            _loadingProgress = 0;
        }
    }
}

