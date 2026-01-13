using UniRx;

namespace PG.Asteroids.Models.SimulationModels
{
    public class SimulationModel
    {
        public ReactiveProperty<int> AsteroidsCount = new(0);
        public ShipSimulationModel ShipSimulationModel = new();
        public PlayerInputState PlayerInputState = new();
    }
}
