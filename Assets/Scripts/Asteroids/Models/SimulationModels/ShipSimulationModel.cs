using System;

namespace PG.Asteroids.Models.SimulationModels
{
    public class ShipSimulationModel
    {
        private float _thrust;
        private int _rotation;

        public event Action<float> OnThrustChanged;
        public event Action<int> OnRotationChanged;

        public float Thrust
        {
            get => _thrust;
            set
            {
                if (Math.Abs(_thrust - value) > 0.001f)
                {
                    _thrust = value;
                    OnThrustChanged?.Invoke(_thrust);
                }
            }
        }

        public int Rotation
        {
            get => _rotation;
            set
            {
                if (_rotation != value)
                {
                    _rotation = value;
                    OnRotationChanged?.Invoke(_rotation);
                }
            }
        }

        public ShipSimulationModel()
        {
            _thrust = 0f;
            _rotation = 0;
        }
    }
}
