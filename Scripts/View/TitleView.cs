using System;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace View
{
    public class TitleView : MonoBehaviour, ITitleView
    {
        [SerializeField]
        private Button settingsButton;

        [SerializeField]
        private Button startButton;

        public IObservable<Unit> OnClickSettings => settingsButton.OnClickAsObservable();
        
        public IObservable<Unit> OnClickGameStart => startButton.OnClickAsObservable();

        public void AppealStartButton()
        {
            var rectTransform = startButton.transform as RectTransform;
            Sequence sequence = DOTween.Sequence();
            sequence.Append(rectTransform.DOScale(1.2f, 0.2f));
            sequence.Append(rectTransform.DOScale(1f, 0.2f).SetEase(Ease.OutBack));
            sequence.Play();
        }
    }
}