using Zenject;

namespace PG.Asteroids.Contexts.GamePlay
{
    public class PlayerCrashedSignal
    {
        public Asteroid Asteroid;
        public PlayerShip PlayerShip;

        public PlayerCrashedSignal(Asteroid asteroid, PlayerShip playerShip)
        {
            Asteroid = asteroid;
            PlayerShip = playerShip;
        }
    }
    
    public class RocketHitSignal
    {
        public Asteroid Asteroid;
        public Rocket Rocket;

        public RocketHitSignal(Asteroid asteroid, Rocket rocket)
        {
            Asteroid = asteroid;
            Rocket = rocket;
        }
    }

    public class SimulationStartedSignal
    {
        
    }
}