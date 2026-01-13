using System;

namespace PG.Asteroids.Models.MediatorModels
{
    public class GamePlayModel
    {
        private int _lives;
        private bool _isDead;
        private int _scores;

        public event Action<int> OnLivesChanged;
        public event Action<bool> OnIsDeadChanged;
        public event Action<int> OnScoresChanged;

        public int Lives
        {
            get => _lives;
            set
            {
                if (_lives != value)
                {
                    _lives = value;
                    OnLivesChanged?.Invoke(_lives);
                }
            }
        }

        public bool IsDead
        {
            get => _isDead;
            set
            {
                if (_isDead != value)
                {
                    _isDead = value;
                    OnIsDeadChanged?.Invoke(_isDead);
                }
            }
        }

        public int Scores
        {
            get => _scores;
            set
            {
                if (_scores != value)
                {
                    _scores = value;
                    OnScoresChanged?.Invoke(_scores);
                }
            }
        }

        public GamePlayModel()
        {
            _lives = 0;
            _isDead = false;
            _scores = 0;
        }
    }
}