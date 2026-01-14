using Cysharp.Threading.Tasks;
using PG.Core.Installers;
using Zenject;

namespace PG.Core.Commands
{
    public class LoadSceneCommand : BaseCommand<LoadSceneParams>
    {
        [Inject] private readonly ISceneLoader _sceneLoader;

        public override async UniTask Execute(LoadSceneParams parameters)
        {
            await _sceneLoader.LoadScene(parameters.Scene, parameters.LoadSceneMode);
        }
    }
}
