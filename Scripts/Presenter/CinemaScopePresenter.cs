using Domain;
using UniRx;
using VContainer.Unity;
using View;

namespace Presenter
{
    public class CinemaScopePresenter : IStartable
    
    {
        private readonly CinemaScopeModel model;
        private readonly ICinemaScopeView view;

        private readonly CompositeDisposable compositeDisposable = new CompositeDisposable();

        public CinemaScopePresenter(CinemaScopeModel model, ICinemaScopeView view)
        {
            this.model = model;
            this.view = view;
        }

        public void Start()
        {
            this.model.OnShow
                .Subscribe(async duration =>
                {
                    await this.view.Show(duration);
                    model.SetShownState(true);
                })
                .AddTo(compositeDisposable);

            this.model.OnHide
                .Subscribe(async duration =>
                {
                    await this.view.Hide(duration);
                    model.SetShownState(false);
                })
                .AddTo(compositeDisposable);
        }
    }
}
