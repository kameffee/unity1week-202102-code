using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Master;
using UniRx;
using UnityEngine;
using VContainer;
using View;
using Random = UnityEngine.Random;

namespace Domain
{
    /// <summary>
    /// ゴミの生成を行う
    /// </summary>
    public class GenerateDustModel
    {
        public struct GenerateData
        {
            public Vector3 Position { get; set; }

            public DustEntity Data { get; set; }

            public DustView Prefab { get; set; }
        }

        /// 生成
        public IObservable<GenerateData> OnGenerateDust => onGenerateDust;
        private readonly Subject<GenerateData> onGenerateDust = new Subject<GenerateData>();

        private readonly IDustMaster database;

        private readonly AssetLoader<DustView> assetLoader;

        [Inject]
        public GenerateDustModel(IDustMaster database, AssetLoader<DustView> assetLoader)
        {
            this.database = database;
            this.assetLoader = assetLoader;
        }

        private Vector3 position;

        public async UniTask StartGenerate(int num, Vector3 from, Vector3 to, Vector3 area)
        {
            // 距離
            var offset = to.x - from.x;
            // 生成間隔距離
            var interval = offset / num;

            // スタート位置
            position = from;

            for (int i = 0; i < num; i++)
            {
                var data = RandomDust();
                Vector3 pos = position + new Vector3(Random.Range(area.x / 2 * -1, area.x / 2),
                    Random.Range(area.y / 2 * -1, area.y / 2), 0);

                await Create(data, pos);

                position += new Vector3(interval, 0f, 0f);
            }
        }

        private async UniTask Create(DustEntity data, Vector3 position)
        {
            var prefab = await assetLoader.GetCacheOrLoad(data.prefabPath);
            Debug.Assert(prefab != null, $"Load error: {data.DustName}");
            onGenerateDust.OnNext(new GenerateData()
            {
                Data = data,
                Position = position,
                Prefab = prefab,
            });
        }

        public DustEntity RandomDust()
        {
            var all = database.All();
            var data = all.ElementAt(Random.Range(0, all.Count()));
            return data;
        }
    }
}