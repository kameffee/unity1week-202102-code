using System;
using Domain;
using UnityEngine;

namespace Master
{
    [Serializable]
    public class DustEntity : IDustData
    {
        public int id;
        public string dustName;
        public long price;
        public float weight;
        public float spawnWeight;
        public string prefabPath;

        public string DustName => dustName;
        public float Price => price;
        public float Weight => weight;
        public float SpawnWeight => spawnWeight;
    }
}