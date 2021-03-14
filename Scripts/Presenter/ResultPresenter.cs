using System;
using Cinemachine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Domain;
using UniRx;
using UnityEngine;
using UseCase;
using VContainer;
using VContainer.Unity;
using View;

namespace Presenter
{
    public class ResultPresenter : IDisposable, IStartable
    {
        private readonly ResultUseCase useCase;
        private readonly Func<IResultView> resultViewFactory;
        private readonly CinemachineVirtualCamera camera;
        private readonly FadeModel fadeModel;

        private IResultView view;

        private readonly CompositeDisposable compositeDisposable = new CompositeDisposable();

        [Inject]
        public ResultPresenter(
            ResultUseCase useCase,
            Func<IResultView> resultViewFactory,
            CinemachineVirtualCamera camera,
            FadeModel fadeModel)
        {
            this.useCase = useCase;
            this.resultViewFactory = resultViewFactory;
            this.camera = camera;
            this.fadeModel = fadeModel;
        }

        public void Start()
        {
            this.useCase.OnResult
                .First()
                .Subscribe(async data => await OpenResult(data))
                .AddTo(compositeDisposable);
        }

        private async UniTask OpenResult(ResultData data)
        {
            // キャラにズーム
            await camera.DOOrthographicSize(8f, 5f, 3f);

            // Wait
            await UniTask.Delay(TimeSpan.FromSeconds(2f));

            view = resultViewFactory.Invoke();
            // 準備
            view.Prepare(data);

            view.OnClickRetry
                .Subscribe(async _ => await CloseResult())
                .AddTo(compositeDisposable);
            
            view.OnClickRanking
                .Subscribe(_ => this.useCase.OpenRanking())
                .AddTo(compositeDisposable);

            // 表示開始
            await view.Open();

            useCase.OpenRanking();
        }

        private async UniTask CloseResult()
        {
            await view.Close();

            await fadeModel.FadeOut(3f);

            this.useCase.Retry();
        }

        public void Dispose()
        {
            compositeDisposable?.Dispose();
        }
    }
}