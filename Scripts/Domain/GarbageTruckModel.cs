using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UniRx;
using UnityEngine;
using VContainer;
using View;

namespace Domain
{
    public enum TruckState
    {
        Stop,
        Idle,
        Drive,
    }

    /// <summary>
    /// ゴミ収集車
    /// </summary>
    public class GarbageTruckModel : IDustBox
    {
        // ゴミを入れる口
        public Transform Entrance { get; set; }

        public IObservable<DustModel> OnDustIn => onDustIn;
        private readonly Subject<DustModel> onDustIn = new Subject<DustModel>();

        // 移動スタート
        public IObservable<Vector3> OnStartMove => onStartMove;
        private readonly Subject<Vector3> onStartMove = new Subject<Vector3>();

        // 到着
        public IObservable<ICheckPoint> OnArrivalCheckPoint => onArrivalCheckPoint;
        private readonly Subject<ICheckPoint> onArrivalCheckPoint = new Subject<ICheckPoint>();

        /// 蓋が開いているか
        public IReadOnlyReactiveProperty<bool> Opened => opened;
        private readonly ReactiveProperty<bool> opened = new ReactiveProperty<bool>(false);

        /// 走っているか
        public IReadOnlyReactiveProperty<TruckState> TruckState => truckState;
        private readonly ReactiveProperty<TruckState> truckState = new ReactiveProperty<TruckState>(Domain.TruckState.Stop);

        public ICheckPoint CurrentCheckPoint { get; private set; }

        [Inject]
        public GarbageTruckModel()
        {
        }

        /// <summary>
        /// エンジン起動
        /// </summary>
        public void Startup()
        {
            if (truckState.Value == Domain.TruckState.Stop)
            {
                truckState.Value = Domain.TruckState.Idle;
            }
        }

        /// <summary>
        /// 投げ入れ口の開閉
        /// </summary>
        /// <param name="isOpen"></param>
        public void SetShutter(bool isOpen)
        {
            opened.Value = isOpen;
        }

        public void MoveStart(ICheckPoint checkPoint)
        {
            CurrentCheckPoint = checkPoint;
            truckState.Value = Domain.TruckState.Drive;
            onStartMove.OnNext(checkPoint.Position);
        }

        /// <summary>
        /// チェックポイントへ到着
        /// </summary>
        public void CheckedPoint()
        {
            truckState.Value = Domain.TruckState.Idle;
            onArrivalCheckPoint.OnNext(CurrentCheckPoint);
        }

        public async UniTask DustIn(IEnumerable<DustModel> dustList)
        {
            Debug.LogWarning("Todo: ゴミ処理");

            Sequence sequence = DOTween.Sequence();

            foreach (var dustModel in dustList.Reverse())
            {
                var transform = dustModel.Transform;
                transform.SetParent(Entrance);
                sequence.Append(transform.DOLocalJump(Vector3.zero, 1, 1, 0.25f)
                    .OnComplete(() =>
                    {
                        onDustIn.OnNext(dustModel);
                        dustModel.Trash();
                    }));
            }

            await sequence.Play();
        }
    }
}
