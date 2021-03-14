using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Master
{
    public class DustMaster : ScriptableObject, IDustMaster
    {
        [SerializeField]
        private DustEntity[] dataList;

        public IEnumerable<DustEntity> All() => dataList;

        public DustEntity FindById(int id)
            => dataList.First(data => data.id == id);

        public void UpdateData(DustEntity[] dataList)
        {
            this.dataList = dataList;
        }
    }
}