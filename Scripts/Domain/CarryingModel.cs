using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Domain
{
    /// <summary>
    /// 荷物を持つ
    /// </summary>
    public class CarryingModel
    {
        public IEnumerable<DustModel> All() => grabedList.AsEnumerable();

        public IReadOnlyReactiveCollection<DustModel> Collection => grabedList;
        private readonly ReactiveCollection<DustModel> grabedList = new ReactiveCollection<DustModel>();

        // 積む場所
        public Transform Holder { get; set; }

        public void Grab(DustModel carryableObject)
        {
            // 運んでる状態フラグを立てておく.
            carryableObject.SetCarrying(true);

            carryableObject.Transform.SetParent(Holder);
            var toPosition = new Vector3(0, 1 * grabedList.Count, 0);
            grabedList.Add(carryableObject);

            // anime
            carryableObject.Transform.DOLocalJump(toPosition, 2, 1, 0.35f);
        }

        /// <summary>
        /// 持っているものをすべて手放す
        /// </summary>
        public async UniTask ReleaseAll()
        {
            Sequence sequence = DOTween.Sequence();
            foreach (var dustModel in grabedList)
            {
                dustModel.Transform.SetParent(null);
                var toPosition = new Vector3(Holder.position.x + Random.Range(-1.5f, 1.5f),
                    Holder.position.y + Random.Range(-1.2f, 1.2f), 0);

                sequence.Join(dustModel.Transform.DOJump(toPosition, 2, 1, 0.8f)
                    .OnComplete(() => dustModel.SetCarrying(false)));
            }

            grabedList.Clear();
            await sequence.Play();
        }

        public async UniTask ReleaseAll(Vector3 direction)
        {
            // 許容重量
            const float capacity = 50f;

            Sequence sequence = DOTween.Sequence();
            foreach (var dustModel in grabedList)
            {
                var power = Mathf.Clamp01((capacity - dustModel.Data.Weight) / capacity);

                dustModel.Transform.SetParent(null);
                var toPosition = new Vector3(
                    Holder.position.x + Random.Range(0.3f, 1.2f) * direction.x * power,
                    Holder.position.y + Random.Range(0.3f, 1.2f) * direction.y * power,
                    0);

                sequence.Join(dustModel.Transform.DOJump(toPosition, Random.Range(1.5f, 2f), 1, 0.8f)
                    .OnComplete(() => dustModel.SetCarrying(false)));
            }

            grabedList.Clear();
            await sequence.Play();
        }

        public void Clear()
        {
            grabedList.Clear();
        }
    }
}