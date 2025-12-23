using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace PG.Core
{
    public interface ISceneLoader
    {
        UniTask LoadScene(string sceneName, LoadSceneMode mode);
        UniTask UnloadScene(string sceneName);
    }
}
