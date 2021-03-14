using System.Collections.Generic;
using UnityEngine;

namespace Master
{
    public interface IDustItemDatabase
    {
        IEnumerable<DustItemData> All();
    }

    public class DustItemDatabase : IDustItemDatabase
    {
        private DustItemData[] datas;

        public DustItemDatabase()
        {
            Debug.Log("database initialize");
            datas = Resources.LoadAll<DustItemData>("Items/");
        }

        public IEnumerable<DustItemData> All() => datas;
    }
}