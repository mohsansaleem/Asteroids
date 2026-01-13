using System;
using System.Collections.Generic;
using PG.Asteroids.Contexts.GamePlay;
using UniRx;

namespace PG.Asteroids.Models.MediatorModels
{
    public class GamePlayModel
    {
        public ReactiveProperty<int> Lives;
        public ReactiveProperty<bool> IsDead;
        public ReactiveProperty<int> Scores;

        public GamePlayModel()
        {
            Lives = new ReactiveProperty<int>(0);
            IsDead = new ReactiveProperty<bool>();
            Scores = new ReactiveProperty<int>();
        }
    }

    public class SimulationModel
    {
        public ReactiveProperty<int> AsteroidsCount = new(0);
        public ShipSimulationModel ShipSimulationModel = new();
        public PlayerInputState PlayerInputState = new();
    }

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

    public class PlayerInputState
    {
        public bool IsRotatingLeft;
        public bool IsRotatingRight;
        public bool IsMovingUp;
        public bool IsSlowingDown;
        public bool IsFiring;
    }
}