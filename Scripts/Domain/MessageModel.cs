using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using VContainer;


namespace Domain
{
    /// <summary>
    /// メッセージ表示ロジック
    /// </summary>
    public class MessageModel : IDisposable
    {
        /// 表示状態
        public IReadOnlyReactiveProperty<bool> IsShowing => isShowing;

        private readonly ReactiveProperty<bool> isShowing = new ReactiveProperty<bool>();

        /// メッセージ発行
        public IObservable<string> OnMessage => onMessage;

        private readonly Subject<string> onMessage = new Subject<string>();

        public IObservable<string> OnComplete => onComplete;
        private readonly Subject<string> onComplete = new Subject<string>();

        private readonly CompositeDisposable compositeDisposable = new CompositeDisposable();

        [Inject]
        public MessageModel()
        {
        }

        public async UniTask StartMessage(string message, CancellationToken cancellationToken = default)
        {
            // 表示してない場合は開いてから開始
            if (!isShowing.Value)
            {
                isShowing.Value = true;
            }

            await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: cancellationToken);

            onMessage.OnNext(message);
            await onComplete.ToUniTask(true, cancellationToken);
        }

        public void SetVisible(bool visible)
        {
            isShowing.Value = visible;
        }

        public void Complete(string message)
        {
            onComplete.OnNext(message);
        }

        public void Dispose()
        {
            isShowing?.Dispose();
            onMessage?.Dispose();
            compositeDisposable?.Dispose();
        }
    }
}