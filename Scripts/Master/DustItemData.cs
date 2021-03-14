using Domain;
using UnityEngine;
using View;

namespace Master
{
    [CreateAssetMenu(fileName = "DustItemData", menuName = "Item/Dust", order = 0)]
    public class DustItemData : ScriptableObject, IDustData
    {
        public string dustName;

        public Sprite sprite;

        public DustView prefab;

        public float weight;

        public float price;

        public float spawnWeight;

        public string DustName => dustName;

        public float Price => price;

        public float Weight => weight;

        public float SpawnWeight => spawnWeight;

        public Sprite Sprite => sprite;

        public DustView Prefab => prefab;
    }
}