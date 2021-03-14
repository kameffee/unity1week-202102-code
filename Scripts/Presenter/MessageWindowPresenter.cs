using System;
using Cysharp.Threading.Tasks;
using Domain;
using UniRx;
using VContainer.Unity;
using View;

namespace Presenter
{
    public class MessageWindowPresenter : IStartable, IDisposable
    {
        private readonly MessageModel model;
        private readonly IMessageWindow messageWindow;

        private readonly CompositeDisposable compositeDisposable = new CompositeDisposable();

        public MessageWindowPresenter(MessageModel model, IMessageWindow messageWindow)
        {
            this.model = model;
            this.messageWindow = messageWindow;
        }

        void IStartable.Start()
        {
            this.model.IsShowing
                .Subscribe(async isShow =>
                {
                    if (isShow)
                        await this.messageWindow.Open();
                    else
                        await this.messageWindow.Close();
                })
                .AddTo(compositeDisposable);

            this.model.OnMessage
                .Subscribe(async message =>
                {
                    await this.messageWindow.StartMessage(message);
                    model.Complete(message);
                })
                .AddTo(compositeDisposable);

            this.model.OnComplete
                .Subscribe(async message => { messageWindow.SetMessage(message); })
                .AddTo(compositeDisposable);
        }

        public void Dispose()
        {
            compositeDisposable?.Dispose();
        }
    }
}