using System;
using Domain;
using UniRx;
using VContainer;
using VContainer.Unity;
using View;

namespace Presenter
{
    public class StatusPresenter : IStartable, IDisposable
    {
        private readonly StatusModel model;
        private readonly IStatusView view;

        private readonly CompositeDisposable compositeDisposable = new CompositeDisposable();

        [Inject]
        public StatusPresenter(StatusModel model, IStatusView statusView)
        {
            this.model = model;
            this.view = statusView;
        }

        public void Start()
        {
            model.Paying
                .Subscribe(paying => view.RenderMoney(paying))
                .AddTo(compositeDisposable);

            view.SetVisible(model.Visible.Value);

            model.Visible
                .Subscribe(visible =>
                {
                    if (visible)
                        view.Open();
                    else
                        view.Close();
                })
                .AddTo(compositeDisposable);

            view.SetVisible(true);
        }

        public void Dispose() => compositeDisposable.Dispose();
    }
}