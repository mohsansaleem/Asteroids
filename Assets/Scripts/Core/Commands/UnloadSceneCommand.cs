using PG.Core.Installers;
using Zenject;

namespace PG.Core.Commands
{
    public class UnloadSceneCommand : BaseCommand
    {
        [Inject] private readonly ISceneLoader _sceneLoader;

        public async void Execute(UnloadSceneSignal loadParams)
        {
            await _sceneLoader.UnloadScene(loadParams.Scene);
            PostExecute();
        }
    }
}
