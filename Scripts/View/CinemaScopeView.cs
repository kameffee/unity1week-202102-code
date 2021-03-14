using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace View
{
    /// <summary>
    /// 映画みたいな上下の黒帯
    /// </summary>
    public class CinemaScopeView : MonoBehaviour, ICinemaScopeView
    {
        [SerializeField]
        private RectTransform top;

        [SerializeField]
        private RectTransform bottom;

        private async void Awake()
        {
            await Hide(0);
        }


        public async UniTask Show(float duration = 2f)
        {
            Sequence sequence = DOTween.Sequence();
            sequence.Append(top.DOAnchorPosY(0, duration));
            sequence.Join(bottom.DOAnchorPosY(0, duration));

            await sequence.Play();
        }

        public async UniTask Hide(float duration = 2f)
        {
            Sequence sequence = DOTween.Sequence();
            sequence.Append(top.DOAnchorPosY(top.sizeDelta.y, duration));
            sequence.Join(bottom.DOAnchorPosY(-bottom.sizeDelta.y, duration));

            await sequence.Play();
        }
    }
}
