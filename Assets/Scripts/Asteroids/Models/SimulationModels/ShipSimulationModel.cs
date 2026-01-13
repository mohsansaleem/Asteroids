using UniRx;

namespace PG.Asteroids.Models.SimulationModels
{
    public class ShipSimulationModel
    {
        public ReactiveProperty<float> Thrust;
        public ReactiveProperty<int> Rotation;

        public ShipSimulationModel()
        {
            Thrust = new ReactiveProperty<float>();
            Rotation = new ReactiveProperty<int>();
        }
    }
}
