using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using VContainer;

namespace Domain
{
    public enum MoveState
    {
        Idle,
        Walk,
        Stan,
    }

    public class PlayerModel : IDisposable
    {
        // 持てる許容
        private static readonly float CapacityWeight = 50f;

        // ベースの重さ
        private static readonly float BaseSpeed = 4f;

        // 最大減速
        private float MaxMinusSpeed { get; } = 3;

        public float Speed { get; private set; }

        // スタン時間
        private static readonly float StanTime = 1;

        public Vector3 MoveVector { get; set; }

        /// 移動可能か
        public IReadOnlyReactiveProperty<bool> Movable => movable;
        private readonly ReactiveProperty<bool> movable = new ReactiveProperty<bool>();

        public IReadOnlyReactiveProperty<Vector3> Position => position;
        private readonly ReactiveProperty<Vector3> position = new ReactiveProperty<Vector3>();

        /// 捨てている最中か
        public IReadOnlyReactiveProperty<bool> Throwing => throwing;

        private readonly ReactiveProperty<bool> throwing = new ReactiveProperty<bool>();

        /// 向いている方向
        public IReadOnlyReactiveProperty<int> MoveDirection => moveDirection;

        private readonly ReactiveProperty<int> moveDirection = new ReactiveProperty<int>(1);

        /// 選択中のゴミ
        public IReadOnlyReactiveProperty<DustModel> SelectedObject => selectedObject;

        private readonly ReactiveProperty<DustModel> selectedObject = new ReactiveProperty<DustModel>();

        /// ターゲットしているゴミ箱
        public IReadOnlyReactiveProperty<IDustBox> TargetDustBox => targetDustBox;

        private ReactiveProperty<IDustBox> targetDustBox = new ReactiveProperty<IDustBox>(null);

        /// ゴミ捨て可能か
        public IReadOnlyReactiveProperty<bool> Scrapable => scrapable;

        private readonly ReactiveProperty<bool> scrapable = new ReactiveProperty<bool>(false);

        /// 投げ込み時
        public IObservable<Unit> OnDustIn => onDustIn;

        private readonly Subject<Unit> onDustIn = new Subject<Unit>();

        /// 転ぶ
        public IObservable<Unit> OnFallDown => onFallDown;

        private readonly Subject<Unit> onFallDown = new Subject<Unit>();

        public IReadOnlyReactiveProperty<MoveState> State => state;
        private readonly ReactiveProperty<MoveState> state = new ReactiveProperty<MoveState>(MoveState.Idle);

        public CarryingModel CarryingModel => carryingModel;
        private readonly CarryingModel carryingModel;

        private readonly StatusModel statusModel;

        private readonly ReactiveCollection<DustModel> nearDustList = new ReactiveCollection<DustModel>();

        // 近くにあるゴミ
        private readonly CompositeDisposable compositeDisposable = new CompositeDisposable();

        [Inject]
        public PlayerModel(CarryingModel carryingModel, StatusModel statusModel)
        {
            this.carryingModel = carryingModel;
            this.statusModel = statusModel;

            // 選択状態の切り替え
            selectedObject
                .Pairwise()
                .Subscribe(pair =>
                {
                    pair.Previous?.SetSelected(false);
                    pair.Current?.SetSelected(true);
                })
                .AddTo(compositeDisposable);

            // ターゲットの状況によって、スクラップできるかが決まる.
            targetDustBox
                // 投げ入れるターゲットがある
                // ゴミを一つ以上持っている
                .Select(box => (box != null) && this.carryingModel.All().Any())
                .Subscribe(SetScrapable)
                .AddTo(compositeDisposable);

            // スピードの再計算
            carryingModel.Collection
                .ObserveCountChanged()
                .Subscribe(_ => CalcSpeed(BaseSpeed))
                .AddTo(compositeDisposable);

            state
                .Where(moveState => moveState == MoveState.Stan)
                .Subscribe(async _ =>
                {
                    Debug.Log("スタン!!");
                    // スタンからの復帰設定
                    await UniTask.Delay(TimeSpan.FromSeconds(StanTime));
                    Debug.Log("スタンから復帰!!");
                    state.Value = MoveState.Idle;
                })
                .AddTo(compositeDisposable);

            position
                .Subscribe(_ => UpdateTarget())
                .AddTo(compositeDisposable);

            // スピード計算
            CalcSpeed(BaseSpeed);
        }

        public void SetPosition(Vector3 position)
            => this.position.Value = position;

        public Vector3 GetMoveVector()
        {
            var horizontal = Input.GetAxis("Horizontal");
            var vertical = Input.GetAxis("Vertical");

            var vector = new Vector3(horizontal, vertical, 0).normalized;
            float moveX = vector.x * Speed;
            float moveY = vector.y * Speed;
            Vector3 moveVector = new Vector3(moveX, moveY, 0);

            if (moveX > 0.01)
            {
                moveDirection.Value = 1;
            }
            else if (moveX < 0)
            {
                moveDirection.Value = -1;
            }

            // 動けない: 捨てているとき, スタン中 
            if (throwing.Value || state.Value == MoveState.Stan)
            {
                moveVector = Vector3.zero;
            }

            if (state.Value != MoveState.Stan && moveVector == Vector3.zero)
            {
                state.Value = MoveState.Idle;
            }
            else if (state.Value != MoveState.Stan)
            {
                state.Value = MoveState.Walk;
            }

            return moveVector;
        }

        /// <summary>
        /// スピードの計算.
        /// 持っているゴミの重さによって変化する.
        /// </summary>
        private void CalcSpeed(float baseSpeed)
        {
            // 合計の重さ
            float sum = 0;
            foreach (var dustModel in carryingModel.All())
            {
                sum += dustModel.Data.weight;
            }

            Speed = baseSpeed - (MaxMinusSpeed - MaxMinusSpeed * (CapacityWeight - sum) / CapacityWeight);
            Debug.Log("Speed:" + Speed);
        }

        /// <summary>
        /// 持つ
        /// </summary>
        public async UniTask Catch()
        {
            if (throwing.Value) return;

            if (selectedObject.Value != null)
            {
                DustModel target = selectedObject.Value;
                carryingModel.Grab(target);
                selectedObject.Value = null;
                nearDustList.Remove(target);

                // 範囲に入っているゴミにフォーカス
                UpdateTarget();
            }
        }

        /// <summary>
        /// 持っているものを手放す
        /// </summary>
        /// <returns></returns>
        public async UniTask Release()
        {
            if (throwing.Value) return;

            await carryingModel.ReleaseAll();
        }

        public void SetScrapable(bool isScrapable)
        {
            scrapable.Value = isScrapable;
        }

        /// <summary>
        /// 投げ入れ口のターゲット設定
        /// </summary>
        /// <param name="target"></param>
        public void SetTargetDustBox(IDustBox target)
        {
            targetDustBox.Value = target;
        }

        /// <summary>
        /// 投げ入れ口のターゲット解除
        /// </summary>
        /// <param name="target"></param>
        public void ClearTargetDustBox(IDustBox target)
        {
            //  同一だったら解除
            if (targetDustBox.Value == target)
            {
                targetDustBox.Value = null;
            }
        }

        /// <summary>
        /// 段差にあたった
        /// </summary>
        public void HitStep()
        {
            Debug.Log("HitStep");
            if (carryingModel.All().Count() > 0)
            {
                state.Value = MoveState.Stan;
                onFallDown.OnNext(Unit.Default);
                Debug.Log("MoveVec:" + MoveVector);
                carryingModel.ReleaseAll(MoveVector);
            }
        }

        /// <summary>
        /// ゴミ箱へ投げ入れる.
        /// </summary>
        public async UniTask ThrowIn()
        {
            if (throwing.Value) return;
            Debug.Log("ThrowIn");

            // 投げ入れ回数をインクリメント
            this.statusModel.AddDustInCount(1);

            throwing.Value = true;

            onDustIn.OnNext(Unit.Default);

            if (targetDustBox.Value != null)
            {
                await targetDustBox.Value.DustIn(carryingModel.All());
                carryingModel.Clear();
            }

            throwing.Value = false;
        }

        public void OnEnterDust(DustModel dust)
        {
            if (dust == null) return;

            nearDustList.Add(dust);

            UpdateTarget();
        }

        public void OnExitDust(DustModel dust)
        {
            if (nearDustList.Contains(dust))
            {
                nearDustList.Remove(dust);

                UpdateTarget();
            }
        }

        private void UpdateTarget()
        {
            // プレイヤーから最も近いゴミ
            var nearest = nearDustList.OrderBy(model => { return Vector3.Distance(model.Position, position.Value); })
                .FirstOrDefault();
            selectedObject.Value = nearest;
        }

        public void SetMovable(bool movable)
        {
            this.movable.Value = movable;
        }

        public void Dispose()
        {
            compositeDisposable?.Dispose();
        }
    }
}
