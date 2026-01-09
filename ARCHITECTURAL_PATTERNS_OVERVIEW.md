# Architectural Patterns Overview - Unity Asteroids

## Document Purpose

This document provides a comprehensive overview of all architectural patterns used in the Unity Asteroids project, their purpose, implementation details, and how they work together to create a maintainable, performant game architecture.

---

## Table of Contents

1. [Architecture Philosophy](#architecture-philosophy)
2. [Core Patterns](#core-patterns)
3. [Game Logic Patterns](#game-logic-patterns)
4. [Data Flow Patterns](#data-flow-patterns)
5. [Pattern Interactions](#pattern-interactions)
6. [Pattern Selection Rationale](#pattern-selection-rationale)
7. [Common Scenarios](#common-scenarios)

---

## Architecture Philosophy

### Hybrid Approach

This project employs a **pragmatic hybrid architecture** that combines:

- **Traditional Unity** - GameObject, MonoBehaviour, Component model
- **Enterprise Patterns** - Mediator, Command, Observer, Factory
- **Performance Patterns** - Object Pooling, Command Buffer, Array-based storage
- **Modern C#** - Async/Await, Dependency Injection, Reactive Programming

### Design Principles Applied

1. **Separation of Concerns** - Core framework separated from game logic
2. **Open/Closed Principle** - Extensible through interfaces (ISimulationSystem, IState, ICommand)
3. **Dependency Inversion** - High-level modules don't depend on low-level (via DI)
4. **Single Responsibility** - Most classes have one clear purpose
5. **Composition over Inheritance** - Component-based entities, injectable services

---

## 1. Core Patterns

### 1.1 Mediator Pattern ⭐⭐⭐⭐⭐

**Intent:** Define an object that encapsulates how a set of objects interact. Promotes loose coupling by keeping objects from referring to each other explicitly.

#### Implementation

**Base Class:**
```
Location: Core/Contexts/FSM/Mediator.cs
```

```csharp
public abstract class Mediator : IInitializable, ITickable, IDisposable
{
    [Inject] protected MediatorStateMachine _mediatorStateMachine;
    [Inject] protected SignalBus _signalBus;

    // State management
    public void AddState<T>() where T : IState
    public void GoToState<T>() where T : IState

    // Lifecycle
    public virtual void Initialize()
    public virtual void Tick()
    public virtual void Dispose()
}
```

#### Game Implementations

**GamePlayMediator**
```
Location: Asteroids/Contexts/GamePlay/GamePlayMediator.cs
```

**Responsibilities:**
- Coordinate between GamePlayView, GamePlayModel, and Simulation
- Manage gameplay state machine (Default ↔ EndGame)
- Handle signals (PlayerDied, AllLivesLost, etc.)
- Bind UI to reactive models

**StartupMediator**
```
Location: Asteroids/Contexts/Startup/StartupMediator.cs
```

**Responsibilities:**
- Orchestrate initialization sequence
- Transition through startup states
- Load assets, data, and scenes
- Navigate to gameplay when ready

#### Pattern Diagram

```
┌─────────────────────────────────────────────┐
│           GamePlayMediator                  │
│  - Orchestrates entire gameplay context     │
└─────────────────────────────────────────────┘
         ↓              ↓              ↓
    ┌────────┐    ┌──────────┐   ┌──────────┐
    │  View  │    │  Model   │   │  State   │
    │        │    │          │   │ Machine  │
    └────────┘    └──────────┘   └──────────┘
         ↓              ↓              ↓
    UI Updates    Reactive Data   Game Flow
```

#### Benefits
✅ Centralized coordination logic
✅ Loose coupling between components
✅ Clear entry point for each context
✅ Testable independently

#### Usage Example

```csharp
// In installer
Container.Bind<GamePlayMediator>().AsSingle();

// Mediator coordinates
public class GamePlayMediator : Mediator
{
    [Inject] private GamePlayView _view;
    [Inject] private GamePlayModel _model;
    [Inject] private Simulation _simulation;

    public override void Initialize()
    {
        base.Initialize();

        // Bind view to model
        _model.Lives.Subscribe(lives => _view.UpdateLives(lives));
        _model.Scores.Subscribe(score => _view.UpdateScore(score));

        // Subscribe to signals
        _signalBus.Subscribe<PlayerDiedSignal>(OnPlayerDied);

        // Setup state machine
        AddState<DefaultGamePlayState>();
        AddState<EndGameState>();
        GoToState<DefaultGamePlayState>();
    }
}
```

---

### 1.2 State Machine Pattern ⭐⭐⭐⭐⭐

**Intent:** Allow an object to alter its behavior when its internal state changes. The object will appear to change its class.

#### Implementation

**Core Classes:**
```
Location: Core/Contexts/FSM/StateManagement/
```

```csharp
// State interface
public interface IState
{
    UniTask Enter();  // Async entry
    UniTask Exit();   // Async cleanup
}

// State machine
public class StateMachine
{
    public async UniTask Enter<TState>() where TState : IState
    public async UniTask ChangeState<TState>() where TState : IState
}

// Mediator integration
public class MediatorStateMachine
{
    private readonly Dictionary<Type, IState> _states;
    private IState _currentState;
}
```

#### State Examples

**Startup Flow:**
```
LoadStaticDataState
    ↓
LoadAssetsState
    ↓
LoadUserDataState
    ↓
LoadGamePlayState
    ↓
GamePlayState
```

**Gameplay Flow:**
```
DefaultGamePlayState ←→ EndGameState
```

#### State Implementation Pattern

```csharp
public class LoadStaticDataState : IState
{
    [Inject] private StaticDataModel _staticDataModel;
    [Inject] private IAssetsLoader _assetsLoader;
    [Inject] private MediatorStateMachine _stateMachine;

    public async UniTask Enter()
    {
        // Load static configuration
        var metaData = await _assetsLoader.LoadAsset<MetaData>("MetaData");
        _staticDataModel.MetaData = metaData;

        // Transition to next state
        await _stateMachine.ChangeState<LoadAssetsState>();
    }

    public async UniTask Exit()
    {
        // Cleanup if needed
        await UniTask.CompletedTask;
    }
}
```

#### Pattern Diagram

```
     ┌────────────────────┐
     │   Mediator         │
     │  State Machine     │
     └────────────────────┘
              │
              │ manages
              ↓
     ┌────────────────────┐
     │  Current State     │ ←── Only one active
     └────────────────────┘
              │
     ┌────────┴────────┐
     ↓                 ↓
  Enter()           Exit()
  (Async)          (Async)
```

#### Benefits
✅ Async state transitions (loading, animations)
✅ Clear state lifecycle
✅ Easy to add new states
✅ State-specific logic encapsulated
✅ No nested if/switch statements

---

### 1.3 Dependency Injection Pattern ⭐⭐⭐⭐⭐

**Intent:** Invert the control of dependencies, allowing external configuration of object dependencies.

#### Framework: Zenject (Extenject)

**Installer Hierarchy:**
```
CoreContextInstaller          (Framework bindings)
    ↓
ProjectContextInstaller       (Project-wide services)
    ↓
StartupInstaller              (Startup scene)
    ↓
GamePlayInstaller             (Gameplay scene)
    ↓
SimulationInstaller           (Simulation layer)
```

#### Binding Examples

**Singleton Binding:**
```csharp
// SimulationInstaller.cs
Container.Bind<SimulationModel>().AsSingle();
Container.Bind<CommandBuffer>().AsSingle();
Container.Bind<CommandBufferMediator>().AsSingle();
```

**Interface Binding:**
```csharp
Container.BindInterfacesAndSelfTo<Simulation>()
    .AsSingle()
    .NonLazy();  // Create immediately
```

**Factory Binding:**
```csharp
// Asteroid factory with pool
Container.BindFactory<int, RigidMovingEntity.MovingEntityModel, Asteroid, Asteroid.Factory>()
    .FromMonoPoolableMemoryPool<Asteroid.AsteroidPool>(pool => pool
        .WithInitialSize(15)
        .FromComponentInNewPrefab(_asteroidPrefab)
        .UnderTransformGroup("Asteroids"));
```

**Command Factory Binding:**
```csharp
Container.BindFactory<int, RigidMovingEntity.MovingEntityModel, SpawnAsteroidsCommand, SpawnAsteroidsCommand.CommandFactory>()
    .FromPoolableMemoryPool<SpawnAsteroidsCommand.CommandPool>(pool => pool
        .WithInitialSize(10)
        .ExpandByDoubling());

Container.Bind<ICommandFactory<SpawnAsteroidsCommand>>()
    .To<SpawnAsteroidsCommand.CommandFactory>()
    .AsSingle();
```

**System List Binding:**
```csharp
Container.BindInterfacesAndSelfTo<PlayerInputSystem>().AsSingle();
Container.BindInterfacesAndSelfTo<ShipControlSystem>().AsSingle();
Container.BindInterfacesAndSelfTo<MovementSystem>().AsSingle();
Container.BindInterfacesAndSelfTo<ExplosionSystem>().AsSingle();
Container.BindInterfacesAndSelfTo<AsteroidsSystem>().AsSingle();

// Bind list for injection
Container.Bind<List<ISimulationSystem>>()
    .FromMethod(ctx => new List<ISimulationSystem>
    {
        ctx.Container.Resolve<PlayerInputSystem>(),
        ctx.Container.Resolve<ShipControlSystem>(),
        ctx.Container.Resolve<MovementSystem>(),
        ctx.Container.Resolve<ExplosionSystem>(),
        ctx.Container.Resolve<AsteroidsSystem>()
    })
    .AsSingle();
```

#### Injection Patterns

**Constructor Injection:**
```csharp
public class MyClass
{
    private readonly IDependency _dependency;

    [Inject]
    public MyClass(IDependency dependency)
    {
        _dependency = dependency;
    }
}
```

**Field Injection:**
```csharp
public class MyMonoBehaviour : MonoBehaviour
{
    [Inject] private SimulationModel _simulationModel;
    [Inject] private CommandBufferMediator _commandMediator;
}
```

#### Benefits
✅ Testability (mock dependencies)
✅ Loose coupling
✅ Configuration flexibility
✅ Clear dependencies visible at class level
✅ Centralized object creation

---

### 1.4 Observer Pattern (Reactive) ⭐⭐⭐⭐

**Intent:** Define a one-to-many dependency between objects so that when one object changes state, all its dependents are notified automatically.

#### Framework: UniRx (Reactive Extensions)

**Reactive Properties:**
```csharp
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
}
```

#### Subscription Pattern

```csharp
public class GamePlayMediator : Mediator
{
    [Inject] private GamePlayModel _model;
    [Inject] private GamePlayView _view;
    private CompositeDisposable _disposables;

    public override void Initialize()
    {
        _disposables = new CompositeDisposable();

        // Subscribe to model changes
        _model.Lives
            .Subscribe(lives => _view.UpdateLives(lives))
            .AddTo(_disposables);

        _model.Scores
            .Subscribe(score => _view.UpdateScore(score))
            .AddTo(_disposables);

        _model.IsDead
            .Where(isDead => isDead)
            .Subscribe(_ => OnPlayerDied())
            .AddTo(_disposables);
    }

    public override void Dispose()
    {
        _disposables.Dispose();  // Auto-unsubscribe
    }
}
```

#### Pattern Diagram

```
  ┌──────────────┐
  │    Model     │
  │  (Subject)   │
  └──────────────┘
         │
         │ notifies
         ↓
  ┌──────────────┐     ┌──────────────┐     ┌──────────────┐
  │  Observer 1  │     │  Observer 2  │     │  Observer 3  │
  │   (View)     │     │  (Mediator)  │     │   (System)   │
  └──────────────┘     └──────────────┘     └──────────────┘
```

#### Benefits
✅ Automatic UI updates
✅ Decoupled data and presentation
✅ Easy to add new observers
✅ Functional reactive programming
✅ Composable streams (Where, Select, etc.)

---

## 2. Game Logic Patterns

### 2.1 Command Pattern (Application Level) ⭐⭐⭐⭐

**Intent:** Encapsulate a request as an object, allowing for parameterization and queuing of requests.

#### Implementation

**Base Command:**
```
Location: Core/Commands/BaseCommand.cs
```

```csharp
public abstract class BaseCommand : IDisposable
{
    [Inject] protected SignalBus SignalBus;

    public abstract UniTask Execute();
    public virtual void Dispose() { }
}
```

#### Command Examples

**LoadStaticDataCommand:**
```csharp
public class LoadStaticDataCommand : BaseCommand
{
    [Inject] private StaticDataModel _staticDataModel;
    [Inject] private IAssetsLoader _assetsLoader;

    public override async UniTask Execute()
    {
        var metaData = await _assetsLoader.LoadAsset<MetaData>("MetaData");
        _staticDataModel.MetaData = metaData;

        SignalBus.Fire(new StaticDataLoadedSignal());
    }
}
```

**LoadUserDataCommand:**
```csharp
public class LoadUserDataCommand : BaseCommand
{
    [Inject] private IDataService _dataService;
    [Inject] private UserDataModel _userDataModel;

    public override async UniTask Execute()
    {
        var userData = await _dataService.LoadUserData();

        if (userData != null)
        {
            _userDataModel.UserData = userData;
            SignalBus.Fire(new UserDataLoadedSignal(userData));
        }
        else
        {
            SignalBus.Fire(new UserDataNotFoundSignal());
        }
    }
}
```

#### Signal-Based Execution

Commands fire signals to notify completion:

```csharp
// State waits for command via signal
public class LoadStaticDataState : IState
{
    [Inject] private LoadStaticDataCommand _loadCommand;
    [Inject] private SignalBus _signalBus;

    public async UniTask Enter()
    {
        var tcs = new UniTaskCompletionSource();

        _signalBus.Subscribe<StaticDataLoadedSignal>(_ => tcs.TrySetResult());

        await _loadCommand.Execute();
        await tcs.Task;

        // Continue to next state...
    }
}
```

#### Benefits
✅ Async operations
✅ Reusable operations
✅ Signal-based coordination
✅ Testable in isolation

---

### 2.2 Command Buffer Pattern ⭐⭐⭐⭐⭐

**Intent:** Queue commands during frame processing, execute them in batch to prevent mid-iteration modifications.

#### Implementation

**Command Buffer:**
```
Location: Asteroids/Contexts/GamePlay/Simulation/CommandsBuffer/CommandBuffer.cs
```

```csharp
public class CommandBuffer
{
    [Inject] private readonly DiContainer _container;
    private readonly Queue<IEntityCommand> _queue = new Queue<IEntityCommand>();

    public void Enqueue<TCommand>(params object[] args)
        where TCommand : IEntityCommand
    {
        var factory = _container.Resolve<ICommandFactory<TCommand>>();
        var cmd = factory.Create(args);
        _queue.Enqueue(cmd);
    }

    public void Playback()
    {
        while (_queue.Count > 0)
        {
            _queue.Dequeue().Execute();
        }
    }
}
```

**Mediator Facade:**
```
Location: Asteroids/Contexts/GamePlay/Simulation/CommandsBuffer/CommandBufferMediator.cs
```

```csharp
public class CommandBufferMediator
{
    [Inject] private CommandBuffer _buffer;

    public void RequestSpawnAsteroid(int levelIndex, RigidMovingEntity.MovingEntityModel entityModel)
    {
        _buffer.Enqueue<SpawnAsteroidsCommand>(levelIndex, entityModel);
    }

    public void RequestAsteroidHit(int asteroidId)
    {
        _buffer.Enqueue<AsteroidHitCommand>(asteroidId);
    }

    public void RequestSpawnExplosion(float explosionTime, Vector3 position)
    {
        _buffer.Enqueue<SpawnExplosionCommand>(explosionTime, position);
    }

    public void Playback()
    {
        _buffer.Playback();
    }
}
```

#### Entity Command Interface

```csharp
public interface IEntityCommand
{
    void Execute();
}

public interface ICommandFactory<TCommand> where TCommand : IEntityCommand
{
    TCommand Create(params object[] args);
}
```

#### Command Example with Pooling

```csharp
public class SpawnAsteroidsCommand : IEntityCommand, IPoolable<int, RigidMovingEntity.MovingEntityModel, IMemoryPool>
{
    [Inject] private readonly SimulationModel _simulationModel;
    [Inject] private readonly Asteroid.Factory _asteroidFactory;

    private int _levelIndex;
    private RigidMovingEntity.MovingEntityModel _entityModel;
    private IMemoryPool _commandPool;

    public void OnSpawned(int levelIndex, RigidMovingEntity.MovingEntityModel entityModel, IMemoryPool commandPool)
    {
        _levelIndex = levelIndex;
        _entityModel = entityModel;
        _commandPool = commandPool;
    }

    public void Execute()
    {
        var asteroid = _asteroidFactory.Create(_levelIndex, _entityModel);
        _simulationModel.AsteroidsCount.Value++;

        int entityId = _simulationModel.Register(asteroid, EntityMask.Movable | EntityMask.Explosive);
        if (entityId != -1)
            asteroid.EntityId = entityId;

        _commandPool.Despawn(this);  // Return to pool
    }

    public void OnDespawned()
    {
        _levelIndex = -1;
        _entityModel = null;
        _commandPool = null;
    }

    // Factory and pool definitions
    public class CommandFactory : PlaceholderFactory<int, RigidMovingEntity.MovingEntityModel, SpawnAsteroidsCommand>,
        ICommandFactory<SpawnAsteroidsCommand>
    {
        public SpawnAsteroidsCommand Create(params object[] args)
        {
            return base.Create((int)args[0], (RigidMovingEntity.MovingEntityModel)args[1]);
        }
    }

    public class CommandPool : MemoryPool<int, RigidMovingEntity.MovingEntityModel, IMemoryPool, SpawnAsteroidsCommand>
    {
    }
}
```

#### Execution Flow

```
Frame N:
  ┌─────────────────────┐
  │ Systems Process     │
  │ Entities            │
  │   ↓                 │
  │ Queue Commands      │ ←── Commands enqueued during iteration
  │   - SpawnAsteroid   │
  │   - DestroyEntity   │
  │   - AsteroidHit     │
  └─────────────────────┘
           ↓
  ┌─────────────────────┐
  │ CommandBuffer       │
  │ Playback()          │ ←── Execute after all systems done
  │   ↓                 │
  │ Execute Commands    │
  │   - Create entities │
  │   - Destroy entities│
  │   - Modify state    │
  └─────────────────────┘
           ↓
Frame N+1:
  Changes visible to systems
```

#### Benefits
✅ Prevents collection modification during iteration
✅ Deterministic execution order
✅ Deferred destruction (safe cleanup)
✅ Commands are pooled (no GC)
✅ Clear separation: Systems produce, Commands consume

---

### 2.3 System Pattern (ECS-Inspired) ⭐⭐⭐

**Intent:** Separate game logic into independent systems that process entities with specific components.

#### System Interface

```
Location: Asteroids/Contexts/GamePlay/Simulation/SimulationSystems/
```

```csharp
public interface ISimulationSystem
{
    void Initialize();                  // Setup
    void Tick(float deltaTime);         // Per-frame logic
    void FixedTick(float fixedDeltaTime); // Physics update
    void Reset();                       // Restart game
    void Dispose();                     // Cleanup
}
```

#### System Implementations

**MovementSystem:**
```csharp
public class MovementSystem : ISimulationSystem
{
    [Inject] private SimulationModel _simulationModel;

    public void Tick(float deltaTime)
    {
        for (int i = 0; i < SimulationModel.MAX_ENTITIES; i++)
        {
            if ((_simulationModel.Masks[i] & EntityMask.Movable) != 0)
            {
                var entity = _simulationModel.Views[i] as MovingEntity;
                entity?.Tick(deltaTime);
            }
        }
    }

    public void FixedTick(float fixedDeltaTime)
    {
        for (int i = 0; i < SimulationModel.MAX_ENTITIES; i++)
        {
            if ((_simulationModel.Masks[i] & EntityMask.Movable) != 0)
            {
                var entity = _simulationModel.Views[i] as MovingEntity;
                entity?.FixedTick(fixedDeltaTime);
            }
        }
    }
}
```

**AsteroidsSystem:**
```csharp
public class AsteroidsSystem : ISimulationSystem
{
    [Inject] private readonly CommandBufferMediator _commandBufferMediator;
    [Inject] private readonly StaticDataModel _staticDataModel;
    [Inject] private readonly SimulationModel _simulationModel;

    public void Initialize()
    {
        // Spawn starting asteroids
        for (int i = _simulationModel.AsteroidsCount.Value;
             i < _staticDataModel.MetaData.AsteroidsData.StartingSpawns;
             i++)
        {
            SpawnNext();
        }
    }

    public void Tick(float deltaTime)
    {
        int currentCount = _simulationModel.AsteroidsCount.Value;
        int maxSpawns = _staticDataModel.MetaData.AsteroidsData.MaxSpawns;

        // Maintain asteroid population
        for (int i = currentCount; i < maxSpawns; i++)
        {
            SpawnNext();
        }
    }

    private void SpawnNext()
    {
        AsteroidsData settings = _staticDataModel.MetaData.AsteroidsData;
        int levelIndex = Random.Range(0, settings.AsteroidLevels.Length);
        RequestSpawnAsteroid(levelIndex);
    }
}
```

**PlayerInputSystem:**
```csharp
public class PlayerInputSystem : ISimulationSystem
{
    [Inject] private SimulationModel _simulationModel;

    public void Tick(float deltaTime)
    {
        var input = _simulationModel.PlayerInputState;

        input.IsRotatingLeft = Input.GetKey(KeyCode.LeftArrow);
        input.IsRotatingRight = Input.GetKey(KeyCode.RightArrow);
        input.IsMovingUp = Input.GetKey(KeyCode.UpArrow);
        input.IsSlowingDown = Input.GetKey(KeyCode.DownArrow);
        input.IsFiring = Input.GetKey(KeyCode.Space);
    }
}
```

#### System Orchestration

```
Location: Asteroids/Contexts/GamePlay/Simulation/Simulation.cs
```

```csharp
public class Simulation : IInitializable, ITickable, IFixedTickable, IDisposable
{
    [Inject] private List<ISimulationSystem> _simulationSystems;
    [Inject] private CommandBufferMediator _commandBufferMediator;

    public virtual void Initialize()
    {
        foreach (var system in _simulationSystems)
            system.Initialize();
    }

    public virtual void Tick()
    {
        float deltaTime = Time.deltaTime;

        // 1. Systems process entities and queue commands
        foreach (var system in _simulationSystems)
            system.Tick(deltaTime);

        // 2. Execute queued commands
        _commandBufferMediator.Playback();
    }

    public void FixedTick()
    {
        foreach (var system in _simulationSystems)
            system.FixedTick(Time.fixedDeltaTime);
    }
}
```

#### System Execution Order

```
Frame Tick:
  1. PlayerInputSystem.Tick()     ─→ Read input
  2. ShipControlSystem.Tick()     ─→ Apply input to ship
  3. MovementSystem.Tick()        ─→ Move entities
  4. ExplosionSystem.Tick()       ─→ Update explosions
  5. AsteroidsSystem.Tick()       ─→ Maintain population
  6. CommandBuffer.Playback()     ─→ Apply changes

FixedUpdate:
  1. MovementSystem.FixedTick()   ─→ Physics movement
  2. (Other physics systems)
```

#### Benefits
✅ Clear separation of responsibilities
✅ Easy to add/remove systems
✅ Deterministic execution order
✅ Systems are independently testable
✅ Can disable systems for debugging

---

### 2.4 Entity Registry Pattern ⭐⭐

**Intent:** Manage entity lifecycle and enable efficient querying by component type using bitmasks.

#### Implementation

**Entity Registry:**
```
Location: Models/MediatorModels/GamePlayModel.cs (SimulationModel)
```

```csharp
public class SimulationModel
{
    // Entity storage
    public const int MAX_ENTITIES = 150;
    public readonly EntityMask[] Masks = new EntityMask[MAX_ENTITIES];
    public readonly SimulationEntity[] Views = new SimulationEntity[MAX_ENTITIES];
    private int _activeEntityCount = 0;

    // Registration
    public int Register(SimulationEntity view, EntityMask mask)
    {
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
        throw new InvalidOperationException("Entity registry full");
    }

    // Unregistration
    public void Unregister(int id)
    {
        if (id >= 0 && id < MAX_ENTITIES && Masks[id] != EntityMask.None)
        {
            Masks[id] = EntityMask.None;
            Views[id] = null;
            _activeEntityCount--;
        }
    }

    // Validation
    public bool IsValidEntity(int id)
    {
        return id >= 0 && id < MAX_ENTITIES && Masks[id] != EntityMask.None;
    }
}
```

**Entity Masks (Component Flags):**
```csharp
[Flags]
public enum EntityMask
{
    None = 0,
    Movable = 1 << 0,      // Can move
    Explosion = 1 << 1,    // Is explosion effect
    Explosive = 1 << 2,    // Can explode
    Dead = 1 << 3,         // Marked for deletion
    PlayerShip = 1 << 4    // Is player ship
}
```

#### Entity Lifecycle

```
1. Entity Created
   └─→ Factory.Create()
       └─→ Pool.Spawn()

2. Entity Registered
   └─→ SimulationModel.Register(entity, mask)
       └─→ Assigns ID
       └─→ Stores in Masks[] and Views[]

3. Entity Used
   └─→ Systems query by mask
       └─→ Process matching entities

4. Entity Destroyed
   └─→ RequestDestroy(id, pool)
       └─→ SimulationModel.Unregister(id)
       └─→ Pool.Despawn(entity)
```

#### Query Pattern

```csharp
// Query all movable entities
for (int i = 0; i < SimulationModel.MAX_ENTITIES; i++)
{
    if ((_simulationModel.Masks[i] & EntityMask.Movable) != 0)
    {
        var entity = _simulationModel.Views[i];
        // Process entity
    }
}

// Query specific combination
for (int i = 0; i < SimulationModel.MAX_ENTITIES; i++)
{
    var mask = _simulationModel.Masks[i];
    if ((mask & EntityMask.Movable) != 0 &&
        (mask & EntityMask.Explosive) != 0)
    {
        // Process movable explosive entities (asteroids)
    }
}
```

#### Benefits
✅ Fast iteration (array-based, cache-friendly)
✅ Bitmask queries are efficient
✅ Fixed memory allocation (no GC)
✅ Simple to understand

#### Limitations
⚠️ Not true ECS (stores MonoBehaviour references)
⚠️ Fixed max entities (150)
⚠️ Linear search for free slots (O(n))
⚠️ Tightly coupled to Unity GameObjects

---

## 3. Data Flow Patterns

### 3.1 Factory Pattern with Object Pooling ⭐⭐⭐⭐⭐

**Intent:** Control object creation and reuse objects to minimize garbage collection.

#### Implementation (Zenject Integration)

**Entity with Pool:**
```csharp
public class Asteroid : RigidMovingEntity,
    IPoolable<int, RigidMovingEntity.MovingEntityModel, IMemoryPool>
{
    // Poolable interface
    public void OnSpawned(int levelIndex, MovingEntityModel model, IMemoryPool pool)
    {
        Pool = pool;
        LevelIndex = levelIndex;
        Initialize(model);
    }

    public void OnDespawned()
    {
        Pool = null;
    }

    // Factory definition
    public class Factory : PlaceholderFactory<int, MovingEntityModel, Asteroid>
    {
    }

    // Pool definition
    public class AsteroidPool : MonoPoolableMemoryPool<int, MovingEntityModel, IMemoryPool, Asteroid>
    {
    }
}
```

**Installer Binding:**
```csharp
Container.BindFactory<int, RigidMovingEntity.MovingEntityModel, Asteroid, Asteroid.Factory>()
    .FromMonoPoolableMemoryPool<Asteroid.AsteroidPool>(pool => pool
        .WithInitialSize(15)                          // Pre-create 15 instances
        .ExpandByDoubling()                           // Double pool when full
        .FromComponentInNewPrefab(_asteroidPrefab)    // Use prefab
        .UnderTransformGroup("Asteroids"));           // Organize under parent
```

#### Usage Pattern

**Creation:**
```csharp
public class SpawnAsteroidsCommand : IEntityCommand
{
    [Inject] private readonly Asteroid.Factory _asteroidFactory;

    public void Execute()
    {
        // Factory gets object from pool or creates new
        var asteroid = _asteroidFactory.Create(_levelIndex, _entityModel);
        // asteroid.OnSpawned() called automatically
    }
}
```

**Destruction:**
```csharp
public class DestroyEntityCommand : IEntityCommand
{
    public void Execute()
    {
        if (_pool != null && entity is IPoolable)
        {
            _pool.Despawn(entity);  // Return to pool
            // entity.OnDespawned() called automatically
        }
        else
        {
            Object.Destroy(entity.gameObject);  // Permanent destruction
        }
    }
}
```

#### Pooled Entities

| Entity Type | Initial Pool Size | Expand Strategy |
|-------------|-------------------|-----------------|
| Asteroid    | 15               | Double          |
| Rocket      | 6                | Double          |
| Explosion   | 3                | Double          |

#### Pooled Commands

| Command Type           | Initial Pool Size |
|------------------------|-------------------|
| SpawnAsteroidsCommand  | 10               |
| AsteroidHitCommand     | 5                |
| SpawnExplosionCommand  | 3                |
| DestroyEntityCommand   | 10               |
| SpawnRocketCommand     | 5                |
| ShipCrashedCommand     | 1                |

#### Pool Lifecycle

```
┌─────────────────────────────────┐
│         Pool Initialize         │
│  - Pre-create N instances       │
│  - All instances in pool        │
└─────────────────────────────────┘
              ↓
┌─────────────────────────────────┐
│      Factory.Create()           │
│  - Get from pool if available   │
│  - Create new if pool empty     │
│  - Call OnSpawned()             │
└─────────────────────────────────┘
              ↓
┌─────────────────────────────────┐
│      Object In Use              │
│  - Active in scene              │
│  - Participating in gameplay    │
└─────────────────────────────────┘
              ↓
┌─────────────────────────────────┐
│      Pool.Despawn()             │
│  - Call OnDespawned()           │
│  - Return to pool               │
│  - Deactivate GameObject        │
└─────────────────────────────────┘
              ↓
         (Back to pool)
```

#### Benefits
✅ Eliminates GC from frequent create/destroy
✅ Predictable performance
✅ Automatic lifecycle management
✅ Pool grows as needed
✅ Objects organized in hierarchy

---

### 3.2 Service Locator Pattern ⭐⭐⭐

**Intent:** Provide a global point of access to services without coupling to concrete implementations.

#### Implementation (via Dependency Injection)

**Service Interfaces:**
```csharp
public interface IAssetsLoader
{
    UniTask<T> LoadAsset<T>(string key) where T : Object;
    UniTask<T[]> LoadAssets<T>(string key) where T : Object;
}

public interface IDataService
{
    UniTask<UserData> LoadUserData();
    UniTask SaveUserData(UserData userData);
}

public interface ISceneLoader
{
    UniTask LoadScene(string sceneName, LoadSceneMode mode);
    UniTask UnloadScene(string sceneName);
}
```

**Service Implementations:**
```csharp
public class AddressablesLoader : IAssetsLoader
{
    public async UniTask<T> LoadAsset<T>(string key) where T : Object
    {
        var handle = Addressables.LoadAssetAsync<T>(key);
        return await handle.Task.AsUniTask();
    }
}

public class FileStorageService : IDataService
{
    private const string FILE_NAME = "user_data.json";

    public async UniTask<UserData> LoadUserData()
    {
        string path = Path.Combine(Application.persistentDataPath, FILE_NAME);
        if (!File.Exists(path))
            return null;

        string json = await File.ReadAllTextAsync(path);
        return JsonConvert.DeserializeObject<UserData>(json);
    }
}
```

**Service Registration:**
```csharp
// ProjectContextInstaller.cs
Container.Bind<IAssetsLoader>().To<AddressablesLoader>().AsSingle();
Container.Bind<IDataService>().To<FileStorageService>().AsSingle();
Container.Bind<ISceneLoader>().To<SceneLoaderService>().AsSingle();
```

**Service Usage:**
```csharp
public class LoadStaticDataCommand : BaseCommand
{
    [Inject] private IAssetsLoader _assetsLoader;  // Service injected
    [Inject] private StaticDataModel _staticDataModel;

    public override async UniTask Execute()
    {
        var metaData = await _assetsLoader.LoadAsset<MetaData>("MetaData");
        _staticDataModel.MetaData = metaData;
    }
}
```

#### Benefits
✅ Decoupled from implementations
✅ Easy to swap implementations (testing, platforms)
✅ Services resolved at runtime
✅ Can use different implementations per platform

---

## 4. Pattern Interactions

### 4.1 Complete Frame Flow

```
┌───────────────────────────────────────────────────────────────┐
│                        FRAME N                                │
└───────────────────────────────────────────────────────────────┘

1. Unity Update()
   └─→ Zenject calls ITickable.Tick()
       ├─→ Mediator.Tick()
       │   └─→ StateMachine.Tick()
       │       └─→ CurrentState.Update()
       │
       └─→ Simulation.Tick()
           │
           ├─→ PlayerInputSystem.Tick()
           │   └─→ Read input → PlayerInputState
           │
           ├─→ ShipControlSystem.Tick()
           │   └─→ Apply input → Ship movement
           │       └─→ Queue SpawnRocketCommand
           │
           ├─→ MovementSystem.Tick()
           │   └─→ Move all Movable entities
           │
           ├─→ ExplosionSystem.Tick()
           │   └─→ Update explosion timers
           │
           ├─→ AsteroidsSystem.Tick()
           │   └─→ Maintain asteroid count
           │       └─→ Queue SpawnAsteroidsCommand
           │
           └─→ CommandBuffer.Playback()
               ├─→ Execute SpawnRocketCommand
               │   ├─→ Factory.Create(Rocket)
               │   │   └─→ Pool.Spawn() → OnSpawned()
               │   └─→ Register in SimulationModel
               │
               └─→ Execute SpawnAsteroidsCommand
                   ├─→ Factory.Create(Asteroid)
                   └─→ Register in SimulationModel

2. Unity FixedUpdate()
   └─→ Zenject calls IFixedTickable.FixedTick()
       └─→ Simulation.FixedTick()
           └─→ MovementSystem.FixedTick()
               └─→ Physics movement for all Movable entities

3. Unity LateUpdate()
   └─→ Collision detection
       └─→ Rocket.OnTriggerEnter(Asteroid)
           ├─→ Queue SpawnExplosionCommand
           ├─→ Queue DestroyEntityCommand(Rocket)
           └─→ Queue AsteroidHitCommand(Asteroid)

┌───────────────────────────────────────────────────────────────┐
│                        FRAME N+1                              │
└───────────────────────────────────────────────────────────────┘

Commands queued in Frame N are executed in Frame N+1
```

### 4.2 Entity Lifecycle Flow

```
┌──────────────────────────────┐
│  User presses SPACE          │
└──────────────────────────────┘
              ↓
┌──────────────────────────────┐
│  PlayerInputSystem.Tick()    │
│  - Sets IsFiring = true      │
└──────────────────────────────┘
              ↓
┌──────────────────────────────┐
│  ShipControlSystem.Tick()    │
│  - Detects IsFiring          │
│  - Requests spawn rocket     │
└──────────────────────────────┘
              ↓
┌──────────────────────────────┐
│  CommandBufferMediator       │
│  Enqueue<SpawnRocketCommand> │
└──────────────────────────────┘
              ↓
┌──────────────────────────────┐
│  CommandBuffer.Playback()    │
│  Execute SpawnRocketCommand  │
└──────────────────────────────┘
              ↓
┌──────────────────────────────┐
│  Rocket.Factory.Create()     │
│  - Get from pool             │
│  - Call OnSpawned()          │
└──────────────────────────────┘
              ↓
┌──────────────────────────────┐
│  SimulationModel.Register()  │
│  - Assign entity ID          │
│  - Store in registry         │
└──────────────────────────────┘
              ↓
┌──────────────────────────────┐
│  MovementSystem processes    │
│  - Rocket moves each frame   │
└──────────────────────────────┘
              ↓
┌──────────────────────────────┐
│  Collision: Rocket hits      │
│  Asteroid                    │
└──────────────────────────────┘
              ↓
┌──────────────────────────────┐
│  Rocket.OnTriggerEnter()     │
│  - Queue explosion command   │
│  - Queue destroy command     │
│  - Queue asteroid hit command│
└──────────────────────────────┘
              ↓
┌──────────────────────────────┐
│  Next Frame: Commands Execute│
│  1. Spawn explosion          │
│  2. Destroy rocket (pool)    │
│  3. Damage asteroid          │
└──────────────────────────────┘
```

### 4.3 State Transition Flow

```
┌────────────────────────────────────┐
│  Application Start                 │
└────────────────────────────────────┘
              ↓
┌────────────────────────────────────┐
│  StartupMediator.Initialize()      │
│  - Add states to state machine     │
│  - GoToState<LoadStaticDataState>()│
└────────────────────────────────────┘
              ↓
┌────────────────────────────────────┐
│  LoadStaticDataState.Enter()       │
│  - Execute LoadStaticDataCommand   │
│  - Wait for completion             │
│  - Transition to LoadAssetsState   │
└────────────────────────────────────┘
              ↓
┌────────────────────────────────────┐
│  LoadAssetsState.Enter()           │
│  - Load game assets                │
│  - Transition to LoadUserDataState │
└────────────────────────────────────┘
              ↓
┌────────────────────────────────────┐
│  LoadUserDataState.Enter()         │
│  - Execute LoadUserDataCommand     │
│  - If found: use data              │
│  - If not: create default          │
│  - Transition to LoadGamePlayState │
└────────────────────────────────────┘
              ↓
┌────────────────────────────────────┐
│  LoadGamePlayState.Enter()         │
│  - Load gameplay scene             │
│  - Transition to GamePlayState     │
└────────────────────────────────────┘
              ↓
┌────────────────────────────────────┐
│  GamePlayState.Enter()             │
│  - Initialize gameplay             │
│  - Player plays game               │
└────────────────────────────────────┘
              ↓
┌────────────────────────────────────┐
│  Player dies (all lives lost)      │
│  - Fire AllLivesLostSignal         │
└────────────────────────────────────┘
              ↓
┌────────────────────────────────────┐
│  GamePlayMediator                  │
│  - Receives signal                 │
│  - GoToState<EndGameState>()       │
└────────────────────────────────────┘
              ↓
┌────────────────────────────────────┐
│  EndGameState.Enter()              │
│  - Show game over UI               │
│  - Save score                      │
│  - Wait for restart                │
└────────────────────────────────────┘
```

---

## 5. Pattern Selection Rationale

### Why These Patterns?

#### Mediator Pattern
**Problem:** Unity scenes have many interconnected objects (UI, game logic, state)
**Solution:** Mediator centralizes coordination, reducing coupling
**Alternative Considered:** Event bus only (too distributed, hard to follow flow)

#### State Machine Pattern
**Problem:** Complex initialization sequence, multiple game states
**Solution:** Explicit states with async transitions
**Alternative Considered:** Boolean flags (unmanageable, error-prone)

#### Command Buffer Pattern
**Problem:** Collection modification during iteration (foreach + destroy = crash)
**Solution:** Queue changes, apply after iteration
**Alternative Considered:** Immediate execution (causes bugs), coroutines (harder to control)

#### Object Pooling
**Problem:** Frequent create/destroy causes GC spikes (stuttering)
**Solution:** Reuse objects, eliminate allocations
**Alternative Considered:** No pooling (unacceptable performance), manual pooling (error-prone)

#### Dependency Injection
**Problem:** Hard-coded dependencies prevent testing, flexibility
**Solution:** Inject dependencies, configure in installers
**Alternative Considered:** Singletons (global state, untestable), manual wiring (tedious)

#### Observer Pattern (Reactive)
**Problem:** Manual UI updates, change propagation
**Solution:** Reactive properties auto-notify observers
**Alternative Considered:** Manual update methods (easy to forget), polling (inefficient)

---

## 6. Common Scenarios

### Scenario 1: Adding a New Entity Type

**Example: Adding a UFO enemy**

**Step 1: Create Entity Class**
```csharp
public class UFO : RigidMovingEntity,
    IPoolable<UFOConfig, IMemoryPool>
{
    public void OnSpawned(UFOConfig config, IMemoryPool pool)
    {
        Pool = pool;
        Initialize(config.MovementModel);
    }

    public void OnDespawned()
    {
        Pool = null;
    }

    public class Factory : PlaceholderFactory<UFOConfig, UFO> { }
    public class UFOPool : MonoPoolableMemoryPool<UFOConfig, IMemoryPool, UFO> { }
}
```

**Step 2: Add Entity Mask**
```csharp
[Flags]
public enum EntityMask
{
    None = 0,
    Movable = 1 << 0,
    Explosion = 1 << 1,
    Explosive = 1 << 2,
    Dead = 1 << 3,
    PlayerShip = 1 << 4,
    UFO = 1 << 5  // New mask
}
```

**Step 3: Create Spawn Command**
```csharp
public class SpawnUFOCommand : IEntityCommand,
    IPoolable<UFOConfig, IMemoryPool>
{
    [Inject] private SimulationModel _simulationModel;
    [Inject] private UFO.Factory _ufoFactory;

    private UFOConfig _config;
    private IMemoryPool _commandPool;

    public void Execute()
    {
        var ufo = _ufoFactory.Create(_config);
        int entityId = _simulationModel.Register(ufo,
            EntityMask.Movable | EntityMask.Explosive | EntityMask.UFO);
        ufo.EntityId = entityId;

        _commandPool.Despawn(this);
    }

    // IPoolable implementation...
}
```

**Step 4: Create System**
```csharp
public class UFOSystem : ISimulationSystem
{
    [Inject] private CommandBufferMediator _commandMediator;

    private float _spawnTimer;

    public void Tick(float deltaTime)
    {
        _spawnTimer += deltaTime;

        if (_spawnTimer >= 30f)  // Spawn every 30 seconds
        {
            var config = new UFOConfig { /* ... */ };
            _commandMediator.RequestSpawnUFO(config);
            _spawnTimer = 0f;
        }
    }
}
```

**Step 5: Register in Installer**
```csharp
// Bind factory and pool
Container.BindFactory<UFOConfig, UFO, UFO.Factory>()
    .FromMonoPoolableMemoryPool<UFO.UFOPool>(pool => pool
        .WithInitialSize(2)
        .FromComponentInNewPrefab(_ufoPrefab));

// Bind command
Container.BindFactory<UFOConfig, SpawnUFOCommand, SpawnUFOCommand.CommandFactory>()
    .FromPoolableMemoryPool<SpawnUFOCommand.CommandPool>(pool => pool.WithInitialSize(2));

// Bind system
Container.BindInterfacesAndSelfTo<UFOSystem>().AsSingle();

// Add to system list
Container.Bind<List<ISimulationSystem>>()
    .FromMethod(ctx => new List<ISimulationSystem>
    {
        // ... existing systems
        ctx.Container.Resolve<UFOSystem>()
    });
```

---

### Scenario 2: Adding a New Game State

**Example: Adding a Pause state**

**Step 1: Create State Class**
```csharp
public class PauseState : IState
{
    [Inject] private GamePlayView _view;
    [Inject] private Simulation _simulation;
    [Inject] private MediatorStateMachine _stateMachine;
    [Inject] private SignalBus _signalBus;

    public async UniTask Enter()
    {
        // Pause game
        Time.timeScale = 0f;

        // Show pause UI
        _view.ShowPauseMenu();

        // Wait for resume signal
        var tcs = new UniTaskCompletionSource();
        _signalBus.Subscribe<ResumeGameSignal>(_ => tcs.TrySetResult());
        await tcs.Task;

        // Transition back to default state
        await _stateMachine.ChangeState<DefaultGamePlayState>();
    }

    public async UniTask Exit()
    {
        Time.timeScale = 1f;
        _view.HidePauseMenu();
        await UniTask.CompletedTask;
    }
}
```

**Step 2: Register State**
```csharp
public class GamePlayMediator : Mediator
{
    public override void Initialize()
    {
        base.Initialize();

        AddState<DefaultGamePlayState>();
        AddState<EndGameState>();
        AddState<PauseState>();  // Add new state

        GoToState<DefaultGamePlayState>();

        // Listen for pause input
        _signalBus.Subscribe<PauseGameSignal>(_ => GoToState<PauseState>());
    }
}
```

**Step 3: Trigger State**
```csharp
public class PlayerInputSystem : ISimulationSystem
{
    [Inject] private SignalBus _signalBus;

    public void Tick(float deltaTime)
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _signalBus.Fire<PauseGameSignal>();
        }
    }
}
```

---

### Scenario 3: Implementing Power-ups

**Step 1: Create PowerUp Entity**
```csharp
public class PowerUp : MovingEntity,
    IPoolable<PowerUpType, Vector3, IMemoryPool>
{
    public PowerUpType Type { get; private set; }

    public void OnSpawned(PowerUpType type, Vector3 position, IMemoryPool pool)
    {
        Pool = pool;
        Type = type;
        transform.position = position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ship"))
        {
            _commandMediator.RequestPowerUpCollected(EntityId, Type);
        }
    }
}
```

**Step 2: Create Collection Command**
```csharp
public class PowerUpCollectedCommand : IEntityCommand
{
    [Inject] private PlayerShip _playerShip;
    [Inject] private SimulationModel _simulationModel;

    private int _powerUpId;
    private PowerUpType _type;

    public void Execute()
    {
        // Apply effect based on type
        switch (_type)
        {
            case PowerUpType.Shield:
                _playerShip.ActivateShield();
                break;
            case PowerUpType.RapidFire:
                _playerShip.EnableRapidFire();
                break;
            case PowerUpType.ExtraLife:
                _gamePlayModel.Lives.Value++;
                break;
        }

        // Destroy power-up
        if (_simulationModel.IsValidEntity(_powerUpId))
        {
            var powerUp = _simulationModel.Views[_powerUpId];
            _commandMediator.RequestDestroy(_powerUpId, powerUp.Pool);
        }
    }
}
```

**Step 3: Spawn Power-ups**
```csharp
public class PowerUpSystem : ISimulationSystem
{
    [Inject] private CommandBufferMediator _commandMediator;

    private float _spawnTimer;

    public void Tick(float deltaTime)
    {
        _spawnTimer += deltaTime;

        if (_spawnTimer >= 15f)
        {
            var type = (PowerUpType)Random.Range(0, 3);
            var position = GetRandomPosition();
            _commandMediator.RequestSpawnPowerUp(type, position);
            _spawnTimer = 0f;
        }
    }
}
```

---

## Summary

This Unity Asteroids project demonstrates a **sophisticated, production-ready architecture** that balances:

### ✅ Strengths
- **Modularity** - Clear separation of framework and game code
- **Testability** - Dependency injection throughout
- **Performance** - Object pooling, command buffer, array-based storage
- **Maintainability** - Consistent patterns, clear structure
- **Extensibility** - Easy to add entities, systems, states, commands
- **Modern C#** - Async/await, reactive programming, LINQ

### 📊 Pattern Summary

| Pattern | Purpose | Rating | Usage |
|---------|---------|--------|-------|
| Mediator | Coordinate components | ⭐⭐⭐⭐⭐ | Every context |
| State Machine | Manage flow | ⭐⭐⭐⭐⭐ | Startup, Gameplay |
| Command (App) | Async operations | ⭐⭐⭐⭐ | Loading, saving |
| Command Buffer | Deferred execution | ⭐⭐⭐⭐⭐ | Game logic |
| Factory + Pool | Object lifecycle | ⭐⭐⭐⭐⭐ | All entities |
| Observer | Data binding | ⭐⭐⭐⭐ | UI updates |
| System | Game logic | ⭐⭐⭐ | Simulation |
| Entity Registry | Entity storage | ⭐⭐ | Entity management |

### 🎯 Key Takeaways

1. **Patterns work together** - No single pattern, cohesive architecture
2. **Performance first** - Pooling, command buffer prevent GC spikes
3. **Unity-friendly** - Works with MonoBehaviour, not against it
4. **Async-ready** - UniTask integration for smooth loading
5. **DI everywhere** - Zenject enables testable, flexible code
6. **Clear flow** - Input → Systems → Commands → Changes

This architecture can scale to larger projects and serves as a solid foundation for Unity game development.
