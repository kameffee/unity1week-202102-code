using System;
using Cysharp.Threading.Tasks;
using UniRx;

namespace Domain
{
    public class FadeModel
    {
        public IReadOnlyReactiveProperty<bool> IsOut => isOut;
        private ReactiveProperty<bool> isOut = new ReactiveProperty<bool>(false);

        public IObservable<float> OnFadeOut => onFadeOut;
        private readonly Subject<float> onFadeOut = new Subject<float>();

        public IObservable<float> OnFadeIn => onFadeIn;
        private readonly Subject<float> onFadeIn = new Subject<float>();

        public async UniTask FadeOut(float duration = 1f)
        {
            onFadeOut.OnNext(duration);
            await UniTask.WaitUntil(() => isOut.Value);
        }

        public async UniTask FadeIn(float duration = 1f)
        {
            onFadeIn.OnNext(duration);
            await UniTask.WaitUntil(() => !isOut.Value);
        }
        
        public void SetState(bool isOut)
        {
            this.isOut.Value = isOut;
        }
    }
}