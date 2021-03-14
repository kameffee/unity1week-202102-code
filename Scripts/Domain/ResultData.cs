using System.Collections.Generic;
using System.Linq;
using VContainer;

namespace Domain
{
    /// <summary>
    /// 結果表示に使うデータ郡
    /// </summary>
    public class ResultData
    {
        // 儲け
        public long Paying { get; private set; }

        // 投げ入れた回数
        public int DustInCount { get; }

        // 投げ入れたゴミ
        public IReadOnlyList<IDustData> DustItemList => dustItemList;
        private readonly List<IDustData> dustItemList;

        // 総重量
        public float AllWeight
        {
            get
            {
                float sum = 0;
                foreach (var dustData in dustItemList)
                {
                    sum += dustData.Weight;
                }

                return sum;
            }
        }

        [Inject]
        public ResultData( long paying, List<IDustData> dustItemList, int dustInCount)
        {
            this.dustItemList = dustItemList;
            this.Paying = paying;
            this.dustItemList = dustItemList;
            DustInCount = dustInCount; // TODO:
        }

        // Todo: 壊したものとか
    }
}