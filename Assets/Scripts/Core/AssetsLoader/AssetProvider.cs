using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace  PG.Core
{
    public class AssetsLoader : IAssetsLoader
    {
        private readonly Dictionary<string, AsyncOperationHandle> _assetRequests = new ();

        public async UniTask Initialize()
        {
            await Addressables.InitializeAsync();
        }

        public TAsset Load<TAsset>(AssetReference assetReference) where TAsset : class
        {
            return Load<TAsset>(assetReference.AssetGUID);
        }

        public TAsset Load<TAsset>(string key) where TAsset : class
        {
            if (!_assetRequests.TryGetValue(key, out var handle))
            {
                handle = Addressables.LoadAssetAsync<TAsset>(key);
                _assetRequests.Add(key, handle);
            }
            
            return handle.WaitForCompletion() as TAsset;
        }

        public async UniTask<TAsset> LoadAsync<TAsset>(string key) where TAsset : class
        {
            AsyncOperationHandle handle;

            if (!_assetRequests.TryGetValue(key, out handle))
            {
                handle = Addressables.LoadAssetAsync<TAsset>(key);
                _assetRequests.Add(key, handle);
            }

            await handle.ToUniTask();
            
            return handle.Result as TAsset;
        }

        public async UniTask<TAsset[]> LoadAll<TAsset>(List<string> keys) where TAsset : class
        {
            List<UniTask<TAsset>> tasks = new List<UniTask<TAsset>>(keys.Count);

            foreach (string key in keys)
            {
                tasks.Add(LoadAsync<TAsset>(key));
            }

            return await UniTask.WhenAll(tasks);
        }

        public async UniTask Release(string key)
        {
            if (_assetRequests.TryGetValue(key, out var handle))
            {
                Addressables.Release(handle);
            }
        }
        
        public async UniTask ReleaseAll(List<string> keys)
        {
            List<UniTask> tasks = new List<UniTask>();
            foreach (string key in keys)
            {
                tasks.Add(Release(key));
            }
            await UniTask.WhenAll(tasks);
        }

        public async UniTask<TAsset> LoadAsync<TAsset>(AssetReference assetReference) where TAsset : class
        {
            return await LoadAsync<TAsset>(assetReference.AssetGUID);
        }

        public void Cleanup()
        {
            foreach (var assetRequest in _assetRequests)
            {
                Addressables.Release(assetRequest.Value);
            }
            
            _assetRequests.Clear();
        }
    }
}