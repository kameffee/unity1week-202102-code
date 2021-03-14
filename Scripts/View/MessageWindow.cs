using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace View
{
    public class MessageWindow : MonoBehaviour, IMessageWindow
    {
        [SerializeField]
        private TextMeshProUGUI messageText;

        [SerializeField]
        private float textSpeed = 0.1f;

        [SerializeField]
        private Vector2 hideAnchorPosition;
        
        [SerializeField]
        private Vector2 showAnchorPosition;

        private RectTransform rectTransform;

        private void Awake()
        {
            this.rectTransform = transform as RectTransform;
            this.rectTransform.anchoredPosition = hideAnchorPosition;
            this.messageText.text = "";
        }

        public async UniTask Open()
        {
            await rectTransform.DOAnchorPos(showAnchorPosition, 0.3f)
                .SetEase(Ease.OutBack);
        }

        public async UniTask Close()
        {
            await rectTransform.DOAnchorPos(hideAnchorPosition, 0.3f)
                .SetEase(Ease.InBack);

            // Reset
            this.messageText.text = "";
        }

        public async UniTask StartMessage(string message)
        {
            messageText.text = "";

            var time = message.Length * textSpeed;
            await messageText.DOText(message, time).SetEase(Ease.Linear);
        }

        public void SetMessage(string message)
        {
            messageText.text = message;
        }
    }
}