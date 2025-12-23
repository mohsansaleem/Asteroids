using PG.Core.Contexts.StateManagement;
using PG.Asteroids.Models.MediatorModels;
using PG.Asteroids.Views.Startup;
using PG.Core.Contexts;
using Zenject;

namespace PG.Asteroids.Contexts.Startup
{
    public abstract class StartupState : MediatorState
    {
        [Inject]
        protected readonly StartupModel StartupModel;
        [Inject]
        protected readonly StartupView View;
    }
}