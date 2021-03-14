using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace View
{
    public class StatusView : MonoBehaviour, IStatusView
    {
        [SerializeField]
        private CanvasGroup rootCanvasGroup;
        
        [SerializeField]
        private TextMeshProUGUI moneyText;

        private long money;
        private long Money
        {
            get => money;
            set
            {
                money = value;
                moneyText.text = money.ToString("N0");
            }
        }

        private long endMoneyValue;

        public void RenderMoney(long money)
        {
            endMoneyValue = money;
            DOTween.To(() => Money, x => Money = x, endMoneyValue, 0.3f)
                .SetEase(Ease.Linear);
        }

        public void SetVisible(bool visible)
        {
            rootCanvasGroup.alpha = visible ? 1f : 0f;
        }

        public async UniTask Open()
        {
            await rootCanvasGroup.DOFade(1, 1f);
        }

        public async UniTask Close()
        {
            await rootCanvasGroup.DOFade(0, 1f);
        }
    }
}