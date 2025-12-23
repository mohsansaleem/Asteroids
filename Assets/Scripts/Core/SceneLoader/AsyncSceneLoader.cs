using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using Zenject;

namespace PG.Core
{
    public class AsyncSceneLoader : ISceneLoader
    {
        private Dictionary<string, AsyncOperationHandle<SceneInstance>> _sceneHandles;

        public AsyncSceneLoader()
        {
            _sceneHandles = new Dictionary<string, AsyncOperationHandle<SceneInstance>>();
        }
        
        public async UniTask LoadScene(string sceneName, LoadSceneMode mode)
        {
            if (_sceneHandles.ContainsKey(sceneName))
            {
                Debug.LogError($"[AsyncSceneLoader] The scene '{sceneName}' is already loaded.");
            }
            else
            {
                AsyncOperationHandle<SceneInstance> handle = Addressables.LoadSceneAsync(sceneName, mode, false);
                
                _sceneHandles.Add(sceneName, handle);
                
                await handle.ToUniTask();
                handle.Result.ActivateAsync().ToUniTask();
            }
        }

        public async UniTask UnloadScene(string sceneName)
        {
            if (_sceneHandles.TryGetValue(sceneName, out var handle))
            {
                var unloadHandle = Addressables.UnloadSceneAsync(handle);
            
                await unloadHandle.ToUniTask();
            }
            
        }
    }
}
