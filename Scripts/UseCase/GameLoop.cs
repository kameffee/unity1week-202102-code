using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Domain;
using Master;
using UniRx;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using View;
using View.Common;

namespace UseCase
{
    public class GameLoop : IAsyncStartable, IGoalEventHandler, IDisposable
    {
        private readonly IEnumerable<ICheckPoint> checkPoints;

        private readonly GarbageTruckModel truckModel;

        private readonly IBgmController bgmController;
        private readonly MessageEventUseCase messageEventUseCase;
        private readonly PlayerModel playerModel;
        private readonly StatusModel statusModel;
        private readonly CinemaScopeModel cinemaScopeModel;
        private readonly FadeModel fadeModel;
        private readonly GeneralStatusModel generalStatusModel;

        public IObservable<Unit> OnGoal => onGoal;
        private readonly Subject<Unit> onGoal = new Subject<Unit>();

        public ICheckPoint ToCheckPoint { get; private set; }


        private int Index { get; set; } = 0;

        [Inject]
        public GameLoop(IEnumerable<ICheckPoint> checkPoints,
            GarbageTruckModel truckModel,
            IBgmController bgmController,
            MessageEventUseCase messageEventUseCase,
            PlayerModel playerModel,
            StatusModel statusModel,
            CinemaScopeModel cinemaScopeModel,
            FadeModel fadeModel,
            GeneralStatusModel generalStatusModel)
        {
            this.checkPoints = checkPoints.OrderBy(point => point.Position.x);
            this.truckModel = truckModel;
            this.bgmController = bgmController;
            this.messageEventUseCase = messageEventUseCase;
            this.playerModel = playerModel;
            this.statusModel = statusModel;
            this.cinemaScopeModel = cinemaScopeModel;
            this.cinemaScopeModel = cinemaScopeModel;
            this.fadeModel = fadeModel;
            this.generalStatusModel = generalStatusModel;
        }

        public async UniTask StartAsync(CancellationToken cancellation)
        {
            playerModel.SetMovable(false);

            await fadeModel.FadeIn();

            // BGM再生
            if (!bgmController.IsPlaying)
                bgmController.Play(null, 3f);

            await UniTask.Delay(TimeSpan.FromSeconds(2), cancellationToken: cancellation);

            // ゆっくり黒帯を表示
            await cinemaScopeModel.Show();

            this.statusModel.SetVisible(true);

            if (generalStatusModel.IsIntroduction)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: cancellation);

                // ロボットの小言を話し始める.
                await messageEventUseCase.StartMessage(0, cancellationToken: cancellation);

                generalStatusModel.IsIntroduction = false;
            }

            // エンジン起動
            truckModel.Startup();
            await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: cancellation);

            // 開ける
            truckModel.SetShutter(true);
            await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: cancellation);

            // 黒幕を隠す
            await cinemaScopeModel.Hide();

            // 移動可能に変更
            playerModel.SetMovable(true);

            GameStart().Forget();

            playerModel.OnDustIn
                .Where(_ => !messageEventUseCase.IsPlaying)
                .Subscribe(async _ => await messageEventUseCase.RandomMessage());
        }

        private ICheckPoint CurrentCheckPoint()
        {
            return checkPoints.ElementAt(Index);
        }

        public async UniTask GameStart()
        {
            while (Index < checkPoints.Count())
            {
                Debug.Log("Start : " + Index);

                ToCheckPoint = CurrentCheckPoint();
                truckModel.MoveStart(ToCheckPoint);

                // 到着を待つ
                await truckModel.OnArrivalCheckPoint.ToUniTask(true);

                // トラックの走行をストップ
                await UniTask.Delay(TimeSpan.FromSeconds(ToCheckPoint.StopTime));

                // 次のチェックポイント
                Index++;
            }

            Goal();
        }

        public void Goal()
        {
            onGoal.OnNext(Unit.Default);
        }

        public void Dispose()
        {
            onGoal?.Dispose();
        }
    }
}