using System;
using PG.Asteroids.Models;
using PG.Asteroids.Models.MediatorModels;
using UnityEngine;
using Zenject;
using Zenject.SpaceFighter;
using PlayerInputState = PG.Asteroids.Models.MediatorModels.PlayerInputState;

namespace PG.Asteroids.Contexts.GamePlay
{
    public class PlayerInputSystem: ISimulationSystem
    {
        [Inject] private readonly SimulationModel _simulationModel;

        public void Initialize()
        {
            
        }

        public void Tick(float deltaTime)
        {
            PlayerInputState inputState = _simulationModel.PlayerInputState;
            inputState.IsRotatingLeft = Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A);
            inputState.IsRotatingRight = Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D);
            inputState.IsMovingUp = Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W);
            inputState.IsSlowingDown = Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S);

            if (inputState.IsRotatingLeft && inputState.IsRotatingRight)
            {
                inputState.IsRotatingLeft = false;
                inputState.IsRotatingRight = false;
            }

            if (inputState.IsMovingUp && inputState.IsSlowingDown)
            {
                inputState.IsRotatingLeft = false;
                inputState.IsRotatingRight = true;
            }

            inputState.IsFiring = Input.GetKey(KeyCode.F) || Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0);
        }

        public void FixedTick(float deltaTime)
        {
            
        }
        
        public void Reset()
        {
            
        }

        public void Dispose()
        {
            
        }
    }
}