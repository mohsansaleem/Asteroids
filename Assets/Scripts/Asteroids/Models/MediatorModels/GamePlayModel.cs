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

    [Flags]
    public enum EntityMask
    {
        None = 0,
        Movable = 1 << 0,
        Explosion = 1 << 1,
        Explosive = 1 << 2,
        Dead = 1 << 3,
        PlayerShip = 1 << 4
    }

    public class SimulationModel
    {
        public ReactiveProperty<int> AsteroidsCount = new(0);
        public ShipSimulationModel ShipSimulationModel = new();
        public PlayerInputState PlayerInputState = new();

        // Entity Registry
        public const int MAX_ENTITIES = 150;
        public readonly EntityMask[] Masks = new EntityMask[MAX_ENTITIES];
        public readonly SimulationEntity[] Views = new SimulationEntity[MAX_ENTITIES];

        public int Register(SimulationEntity view, EntityMask mask)
        {
            for (int i = 0; i < MAX_ENTITIES; i++)
            {
                if (Masks[i] == EntityMask.None)
                {
                    Masks[i] = mask;
                    Views[i] = view;
                    return i;
                }
            }

            return -1;
        }

        public void Unregister(int id)
        {
            Masks[id] = EntityMask.None;
            Views[id] = null;
        }
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