
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

namespace PG.Core
{
    public interface IAssetsLoader
    {
        UniTask Initialize();
        TAsset Load<TAsset>(AssetReference assetReference) where TAsset : class;
        TAsset Load<TAsset>(string key) where TAsset : class;
        UniTask<TAsset> LoadAsync<TAsset>(AssetReference assetReference) where TAsset : class;
        UniTask<TAsset> LoadAsync<TAsset>(string key) where TAsset : class;
        UniTask<TAsset[]> LoadAll<TAsset>(List<string> keys) where TAsset : class;
        UniTask Release(string key);
        UniTask ReleaseAll(List<string> keys);
        void Cleanup();
    }
}
