using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace View.Common
{
    public class FadeView : MonoBehaviour, IFadeView
    {
        [SerializeField]
        private CanvasGroup rootCanvasGroup;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        public void Initialize(bool isOut)
        {
            if (isOut)
            {
                rootCanvasGroup.interactable = true;
                rootCanvasGroup.blocksRaycasts = true;
                rootCanvasGroup.alpha = 1;
            }
            else
            {
                rootCanvasGroup.interactable = false;
                rootCanvasGroup.blocksRaycasts = false;
                rootCanvasGroup.alpha = 0;
            }
        }

        public async UniTask FadeOut(float duration = 1)
        {
            rootCanvasGroup.interactable = true;
            rootCanvasGroup.blocksRaycasts = true;
            await rootCanvasGroup.DOFade(1, duration);
        }

        public async UniTask FadeIn(float duration = 1)
        {
            rootCanvasGroup.interactable = false;
            rootCanvasGroup.blocksRaycasts = false;
            await rootCanvasGroup.DOFade(0, duration);
        }
    }
}