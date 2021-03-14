using System;
using Cysharp.Threading.Tasks;
using Domain;
using UltimateClean;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer.Unity;
using View;

namespace Presenter
{
    /// <summary>
    /// タイトル画面
    /// </summary>
    public class TitlePresenter : IStartable, IDisposable
    {
        private readonly ITitleView titleView;
        private readonly SettingsDialogPresenter presenter;
        private readonly PlayerModel playerModel;
        private readonly GarbageTruckModel truckModel;
        private readonly FadeModel fadeModel;

        private readonly CompositeDisposable compositeDisposable = new CompositeDisposable();
        
        public TitlePresenter(ITitleView titleView,
            SettingsDialogPresenter presenter,
            PlayerModel playerModel,
            GarbageTruckModel truckModel,
            FadeModel fadeModel)
        {
            this.titleView = titleView;
            this.presenter = presenter;
            this.playerModel = playerModel;
            this.truckModel = truckModel;
            this.fadeModel = fadeModel;
        }

        public void Start()
        {
            this.titleView.OnClickSettings
                .Subscribe(_ => presenter.Open())
                .AddTo(compositeDisposable);

            this.titleView.OnClickGameStart
                .Subscribe(async _ =>
                {
                    await fadeModel.FadeOut(2);
                    await SceneManager.LoadSceneAsync("Main", LoadSceneMode.Single);
                })
                .AddTo(compositeDisposable);

            // 拾ったらトラックのフタを開ける
            this.playerModel.CarryingModel.Collection
                .ObserveCountChanged()
                .Where(count => count > 0)
                .First()
                .Subscribe(_ => truckModel.SetShutter(true))
                .AddTo(compositeDisposable);
            
            // 回収したらスタートボタンを出す？
            this.truckModel.OnDustIn
                .ThrottleFirst(TimeSpan.FromSeconds(3))
                .Subscribe(_ =>
                {
                    Debug.Log("Appeal");
                    this.titleView.AppealStartButton();
                })
                .AddTo(compositeDisposable);
        }

        public void Dispose()
        {
            compositeDisposable?.Dispose();
        }
    }
}