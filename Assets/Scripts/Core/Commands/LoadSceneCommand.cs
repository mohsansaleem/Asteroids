using PG.Core.Installers;
using Zenject;

namespace PG.Core.Commands
{
    public class LoadSceneCommand : BaseCommand
    {
        [Inject] private readonly ISceneLoader _sceneLoader;

        public async void Execute(LoadSceneSignal loadParams)
        {
            await _sceneLoader.LoadScene(loadParams.Scene, loadParams.LoadSceneMode);
            PostExecute();
        }
    }
}
