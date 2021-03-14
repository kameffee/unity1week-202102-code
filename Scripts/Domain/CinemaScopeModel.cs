using System;
using Cysharp.Threading.Tasks;
using UniRx;

namespace Domain
{
    public class CinemaScopeModel
    {
        public IReadOnlyReactiveProperty<bool> Shown => shown;
        private readonly ReactiveProperty<bool> shown = new ReactiveProperty<bool>(false);

        public IObservable<float> OnShow => onShow;
        private readonly Subject<float> onShow = new Subject<float>();

        public IObservable<float> OnHide => onHide;
        private readonly Subject<float> onHide = new Subject<float>();

        public async UniTask Show(float duration = 2f)
        {
            onShow.OnNext(duration);

            await UniTask.WaitUntil(() => shown.Value);
        }

        public async UniTask Hide(float duration = 2f)
        {
            onHide.OnNext(duration);
            await UniTask.WaitUntil(() => !shown.Value);
        }

        public void SetShownState(bool isShow)
        {
            shown.Value = isShow;
        }
    }
}
