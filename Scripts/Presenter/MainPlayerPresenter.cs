using System;
using Cysharp.Threading.Tasks;
using Domain;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using VContainer.Unity;
using View;

namespace Presenter
{
    /// <summary>
    /// ゲームシーンで使用するプレイヤー
    /// </summary>
    public class MainPlayerPresenter : IStartable, IDisposable
    {
        private readonly PlayerModel playerModel;
        private readonly IPlayerView playerView;

        private readonly CompositeDisposable compositeDisposable = new CompositeDisposable();

        public MainPlayerPresenter(PlayerModel playerModel, IPlayerView playerView)
        {
            this.playerModel = playerModel;
            this.playerView = playerView;
        }

        public void Start()
        {
            playerModel.CarryingModel.Holder = playerView.CarryingHolder;

            playerView.CatchTrigger.OnTriggerEnter2DAsObservable()
                .Select(other => other.GetComponent<DustView>()?.Model)
                .Subscribe(dust => playerModel.OnEnterDust(dust))
                .AddTo(compositeDisposable);

            playerView.CatchTrigger.OnTriggerExit2DAsObservable()
                .Select(other => other.GetComponent<DustView>()?.Model)
                .Subscribe(dust => playerModel.OnExitDust(dust))
                .AddTo(compositeDisposable);

            playerView.FootTrigger.OnTriggerEnter2DAsObservable()
                .Select(other => other.GetComponent<Step>())
                .Where(step => step != null)
                .Subscribe(step => playerModel.HitStep())
                .AddTo(compositeDisposable);

            playerModel.MoveDirection
                .Subscribe(direction => playerView.SetDirection(direction))
                .AddTo(compositeDisposable);

            playerModel.State
                .DelayFrame(1)
                .Subscribe(state => playerView.SetWalk(state == MoveState.Walk))
                .AddTo(compositeDisposable);

            playerModel.State
                .Where(state => state == MoveState.Stan)
                .Subscribe(async state =>
                {
                    playerView.SetStan();
                    await UniTask.Delay(TimeSpan.FromSeconds(.3f));
                    // Animatorから叩きたかったが直接張り付いているため,無理やりウェイトで対処.
                    playerView.StanImpulse();
                })
                .AddTo(compositeDisposable);

            playerModel.CarryingModel.Collection
                .ObserveCountChanged()
                .Subscribe(count => playerView.SetCarry(count > 0))
                .AddTo(compositeDisposable);

            Observable.EveryUpdate()
                .Where(_ => Input.GetMouseButtonDown(0))
                .Subscribe(async _ =>
                {
                    if (playerModel.Scrapable.Value)
                    {
                        await playerModel.ThrowIn();
                    }
                    else
                    {
                        await playerModel.Catch();
                    }
                })
                .AddTo(compositeDisposable);

            Observable.EveryUpdate()
                .Where(_ => Input.GetMouseButtonDown(1))
                .Subscribe(async _ => { await this.playerModel.Release(); })
                .AddTo(compositeDisposable);

            Observable.EveryFixedUpdate()
                .Where(_ => playerModel.Movable.Value)
                .Where(_ => playerModel.State.Value != MoveState.Stan)
                .Subscribe(_ =>
                {
                    var moveVector = playerModel.GetMoveVector();
                    var afterPosition = playerView.Translate(moveVector * Time.fixedDeltaTime);
                    playerModel.SetPosition(afterPosition);
                    playerModel.MoveVector = moveVector;
                })
                .AddTo(compositeDisposable);

            playerModel.Scrapable
                .Subscribe(scrappable => Debug.Log("スクラップ" + (scrappable ? "可能" : "不可能")))
                .AddTo(compositeDisposable);
        }

        public void Dispose()
        {
            compositeDisposable?.Dispose();
        }
    }
}
