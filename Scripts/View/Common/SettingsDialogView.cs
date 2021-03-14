using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

namespace View.Common
{
    public class SettingsDialogView : MonoBehaviour, ISoundSettingsView
    {
        [SerializeField]
        private CanvasGroup canvasGroup;

        [SerializeField]
        private Slider bgmSlider;

        [SerializeField]
        private Slider seSlider;

        public IObservable<float> OnChangeBgmVolume 
            => bgmSlider.OnValueChangedAsObservable().Select(value => value / bgmSlider.maxValue);

        public IObservable<float> OnChangeSeVolume 
            => seSlider.OnValueChangedAsObservable().Select(value => value / seSlider.maxValue);

        public IObservable<float> OnSePointerUp => seSlider.OnPointerUpAsObservable().Select(_ => seSlider.value / seSlider.maxValue);

        private RectTransform rectTransform;

        private void Awake()
        {
            rectTransform = transform as RectTransform;
            canvasGroup.DOFade(0, 0f);
        }

        private void Start()
        {
            
        }

        public void SetBgmVolume(float volume)
        {
            bgmSlider.value = bgmSlider.maxValue * volume;
        }

        public void SetSeVolume(float volume)
        {
            seSlider.value = seSlider.maxValue * volume;
        }

        public async UniTask Open()
        {
            rectTransform.localScale = new Vector3(0.8f, 0.8f, 1f);
            rectTransform.DOScale(1f, 0.25f);

            canvasGroup.alpha = 0;
            await canvasGroup.DOFade(1, 0.25f);
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }

        public async UniTask Close()
        {
            rectTransform.DOScale(0.8f, 0.25f);
            await canvasGroup.DOFade(0, 0.25f);
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        public void CloseDialog()
        {
            Close().Forget();
        }
    }
}