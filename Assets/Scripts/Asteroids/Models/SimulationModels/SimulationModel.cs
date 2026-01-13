using System;

namespace PG.Asteroids.Models.SimulationModels
{
    public class SimulationModel
    {
        private int _asteroidsCount;

        public event Action<int> OnAsteroidsCountChanged;

        public int AsteroidsCount
        {
            get => _asteroidsCount;
            set
            {
                if (_asteroidsCount != value)
                {
                    _asteroidsCount = value;
                    OnAsteroidsCountChanged?.Invoke(_asteroidsCount);
                }
            }
        }

        public ShipSimulationModel ShipSimulationModel = new();
        public PlayerInputState PlayerInputState = new();

        public SimulationModel()
        {
            _asteroidsCount = 0;
        }
    }
}
