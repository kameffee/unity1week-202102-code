using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using View;
using Object = UnityEngine.Object;

namespace Domain
{
    /// <summary>
    /// アセットのロードとキャッシュを行う.
    /// </summary>
    public class AssetLoader<T> : IDisposable where T : Object
    {
        private readonly Dictionary<string, T> prefabCache = new Dictionary<string, T>();

        public async UniTask<T> GetCacheOrLoad(string prefabPath)
        {
            if (prefabCache.TryGetValue(prefabPath, out var prefab))
            {
                return prefab;
            }

            var asset = Resources.LoadAsync<T>(prefabPath).asset as T;
            Debug.Assert(asset != null);
            prefabCache.Add(prefabPath, asset);
            return asset;
        }
        
        public void Dispose()
        {
            prefabCache.Clear();
        }
    }
}