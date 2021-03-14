using UniRx;
using VContainer;

namespace Domain
{
    public class StatusModel
    {
        /// 儲け
        public IReadOnlyReactiveProperty<long> Paying => paying;
        private readonly ReactiveProperty<long> paying;

        /// 拾ったゴミ
        public IReadOnlyReactiveCollection<IDustData> DustItems => dustItemList;
        private readonly ReactiveCollection<IDustData> dustItemList = new ReactiveCollection<IDustData>();

        // 投げ入れた回数
        public IReadOnlyReactiveProperty<int> DustInCount => dustInCount;
        private readonly ReactiveProperty<int> dustInCount = new ReactiveProperty<int>(0);

        public IReadOnlyReactiveProperty<bool> Visible => visible;
        private readonly ReactiveProperty<bool> visible = new ReactiveProperty<bool>();

        [Inject]
        public StatusModel(long initialPaying)
        {
            paying = new ReactiveProperty<long>(initialPaying);
        }

        public void AddPaying(long price) => paying.Value += price;

        public void AddDust(IDustData dust) => dustItemList.Add(dust);

        public void AddDustInCount(int count) => dustInCount.Value += count;

        public void SetVisible(bool visible) => this.visible.Value = visible;
    }
}