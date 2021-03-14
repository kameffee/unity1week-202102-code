using System;
using DG.Tweening;
using Domain;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UseCase;
using VContainer;

namespace View
{
    public class GarbageTruckView : MonoBehaviour, IGarbageTruck
    {
        [SerializeField]
        private Collider2D scrapEntrance;

        [SerializeField]
        private Transform entrance;

        [SerializeField]
        private Transform body;

        [SerializeField]
        public float moveSpeed;

        [SerializeField]
        private SpriteRenderer arrow;

        [SerializeField]
        private Animator animator;

        [SerializeField]
        private AudioClip dustInSe;

        [SerializeField]
        private AudioClip startEngineSe;

        public Collider2D ScrapEntrance => scrapEntrance;

        public Transform Entrance => entrance;

        public Transform Body => body;

        public GarbageTruckModel Model { get; set; }

        private PlayerModel playerModel;

        private IDisposable moveDisposable;

        private static readonly int IsShutterOpen = Animator.StringToHash("is_shutter_open");
        private static readonly int Engine = Animator.StringToHash("engine");
        private static readonly int Move = Animator.StringToHash("move");

        [Inject]
        public void Setup(GarbageTruckModel model,
            StatusModel statusModel,
            PlayerModel playerModel,
            ISeController seController)
        {
            Model = model;
            Model.Entrance = entrance;

            this.playerModel = playerModel;

            Model.OnDustIn
                .Subscribe(dustModel =>
                {
                    statusModel.AddPaying((long) dustModel.Data.price);
                    statusModel.AddDust(dustModel.Data);
                })
                .AddTo(this);

            scrapEntrance.OnTriggerEnter2DAsObservable()
                .Select(collider2D => collider2D.GetComponentInParent<IPlayerView>())
                .Where(player => player != null)
                .Subscribe(_ => this.playerModel.SetTargetDustBox(Model))
                .AddTo(this);

            scrapEntrance.OnTriggerExit2DAsObservable()
                .Select(collider2D => collider2D.GetComponentInParent<IPlayerView>())
                .Where(player => player != null)
                .Subscribe(_ => this.playerModel.ClearTargetDustBox(Model))
                .AddTo(this);

            Model.OnStartMove
                .Subscribe(MoveTo)
                .AddTo(this);

            playerModel.TargetDustBox
                .Subscribe(target => SetArrow(target == Model))
                .AddTo(this);

            // 投げ入れ時
            playerModel.OnDustIn
                .Subscribe(_ => DustInAnimation())
                .AddTo(this);

            // シャッター
            Model.Opened
                .Subscribe(isOpen => animator.SetBool(IsShutterOpen, isOpen))
                .AddTo(this);

            Model.TruckState
                .Subscribe(state =>
                {
                    animator.SetBool(Engine, state != TruckState.Stop);

                    if (state == TruckState.Idle)
                    {
                        animator.SetBool(Move, false);
                    }
                    else if (state == TruckState.Drive)
                    {
                        animator.SetBool(Move, true);
                    }
                });

            // ストップ状態からアイドリングになった時 (エンジンがかかった時)
            Model.TruckState
                .Pairwise()
                .Where(pair => pair.Previous == TruckState.Stop && pair.Current == TruckState.Idle)
                .Subscribe(_ => seController.Play(startEngineSe))
                .AddTo(this);

            Model.OnDustIn
                .Subscribe(_ => seController.Play(dustInSe))
                .AddTo(this);
        }

        public void SetArrow(bool visible)
        {
            arrow.DOFade(visible ? 1f : 0f, 0.1f);
        }

        public void DustInAnimation()
        {
            Sequence sequence = DOTween.Sequence();
            sequence.Append(arrow.transform.DOScale(1.2f, 0.1f).SetEase(Ease.InOutSine));
            sequence.Append(arrow.transform.DOScale(1f, 0.3f).SetEase(Ease.OutElastic));
            sequence.AppendInterval(.2f);
            sequence.AppendCallback(() => SetArrow(false));
            sequence.Play();
        }

        private void MoveTo(Vector3 toPosition)
        {
            moveDisposable?.Dispose();

            moveDisposable = this.UpdateAsObservable()
                .Subscribe(_ =>
                {
                    body.Translate(Vector3.right * moveSpeed * Time.deltaTime);
                    if (body.position.x >= toPosition.x)
                    {
                        moveDisposable.Dispose();
                        Model.CheckedPoint();
                    }
                });
        }
    }
}