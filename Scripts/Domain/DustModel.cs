using System;
using Master;
using UniRx;
using UnityEngine;
using VContainer;

namespace Domain
{
    public class DustModel : IDustModel
    {
        public IReadOnlyReactiveProperty<bool> Selected => selected;
        private readonly ReactiveProperty<bool> selected = new ReactiveProperty<bool>();

        public IObservable<Unit> OnTrash => onTrash;
        private readonly Subject<Unit> onTrash = new Subject<Unit>();

        public Transform Transform { get; private set; }

        public IReadOnlyReactiveProperty<bool> Carrying => carrying;
        private readonly ReactiveProperty<bool> carrying = new ReactiveProperty<bool>(false);

        public DustEntity Data => data;
        private readonly DustEntity data;

        public Vector2 Position { get; set; }

        [Inject]
        public DustModel(Transform transform, DustEntity data)
        {
            Transform = transform;
            this.data = data;
        }

        public void SetSelected(bool isSelected)
        {
            selected.Value = isSelected;
        }

        public void SetCarrying(bool isCarrying)
        {
            carrying.Value = isCarrying;
        }

        /// <summary>
        /// 破棄
        /// </summary>
        public void Trash()
        {
            onTrash.OnNext(Unit.Default);
            onTrash.OnCompleted();
        }
    }
}