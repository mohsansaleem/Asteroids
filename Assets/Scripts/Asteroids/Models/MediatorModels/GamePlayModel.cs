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

        private int _activeEntityCount = 0;

        /// <summary>
        /// Gets the current number of active entities in the registry
        /// </summary>
        public int ActiveEntityCount => _activeEntityCount;

        /// <summary>
        /// Registers an entity in the simulation model with safety checks
        /// </summary>
        /// <param name="view">The entity to register (must not be null)</param>
        /// <param name="mask">The entity mask flags (must not be None)</param>
        /// <returns>The assigned entity ID, or throws exception if registry is full</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when view is null</exception>
        /// <exception cref="System.ArgumentException">Thrown when mask is None</exception>
        /// <exception cref="System.InvalidOperationException">Thrown when registry is full</exception>
        public int Register(SimulationEntity view, EntityMask mask)
        {
            if (view == null)
                throw new System.ArgumentNullException(nameof(view), "Cannot register null entity");

            if (mask == EntityMask.None)
                throw new System.ArgumentException("Cannot register entity with EntityMask.None", nameof(mask));

            for (int i = 0; i < MAX_ENTITIES; i++)
            {
                if (Masks[i] == EntityMask.None)
                {
                    Masks[i] = mask;
                    Views[i] = view;
                    _activeEntityCount++;
                    return i;
                }
            }

            // Registry is full - this is a critical error
            UnityEngine.Debug.LogError($"Entity registry full! Cannot register {view.name}. Active entities: {_activeEntityCount}/{MAX_ENTITIES}");
            throw new System.InvalidOperationException($"Entity registry exceeded MAX_ENTITIES ({MAX_ENTITIES}). Consider increasing MAX_ENTITIES or investigating entity leaks.");
        }

        /// <summary>
        /// Unregisters an entity from the simulation model with safety checks
        /// </summary>
        /// <param name="id">The entity ID to unregister</param>
        public void Unregister(int id)
        {
            // Bounds check
            if (id < 0 || id >= MAX_ENTITIES)
            {
                UnityEngine.Debug.LogError($"Invalid entity ID: {id}. Valid range: 0-{MAX_ENTITIES - 1}");
                return;
            }

            // Check if already unregistered
            if (Masks[id] == EntityMask.None)
            {
                UnityEngine.Debug.LogWarning($"Attempting to unregister already inactive entity at ID: {id}");
                return;
            }

            Masks[id] = EntityMask.None;
            Views[id] = null;
            _activeEntityCount--;
        }

        /// <summary>
        /// Checks if an entity ID is valid and active
        /// </summary>
        /// <param name="id">The entity ID to validate</param>
        /// <returns>True if the entity is valid and active, false otherwise</returns>
        public bool IsValidEntity(int id)
        {
            return id >= 0 && id < MAX_ENTITIES && Masks[id] != EntityMask.None;
        }

        /// <summary>
        /// Gets an entity view by ID with bounds checking
        /// </summary>
        /// <param name="id">The entity ID</param>
        /// <returns>The entity view, or null if invalid</returns>
        public SimulationEntity GetEntity(int id)
        {
            if (!IsValidEntity(id))
            {
                UnityEngine.Debug.LogWarning($"Attempted to get invalid entity at ID: {id}");
                return null;
            }
            return Views[id];
        }

        /// <summary>
        /// Resets the entity registry (useful for cleanup/restart)
        /// </summary>
        public void ClearRegistry()
        {
            for (int i = 0; i < MAX_ENTITIES; i++)
            {
                Masks[i] = EntityMask.None;
                Views[i] = null;
            }
            _activeEntityCount = 0;
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