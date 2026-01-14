using Cysharp.Threading.Tasks;
using PG.Core.Installers;
using Zenject;

namespace PG.Core.Commands
{
    public class UnloadSceneCommand : BaseCommand<UnloadSceneParams>
    {
        [Inject] private readonly ISceneLoader _sceneLoader;

        public override async UniTask Execute(UnloadSceneParams parameters)
        {
            await _sceneLoader.UnloadScene(parameters.Scene);
        }
    }
}
