using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Domain;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace View
{
    public class ResultView : MonoBehaviour, IResultView
    {
        [SerializeField]
        private CanvasGroup canvasGroup;

        [SerializeField]
        private TextMeshProUGUI sumWeightText;

        [SerializeField]
        private TextMeshProUGUI sumCarryingCountText;

        [SerializeField]
        private TextMeshProUGUI sumItemCount;

        [SerializeField]
        private ResultDustItemCellView view;

        [SerializeField]
        private RectTransform holder;

        [SerializeField]
        private Button retryButton;

        [SerializeField]
        private Button rankingButton;

        [SerializeField]
        private TextMeshProUGUI scoreText;

        public IObservable<Unit> OnClickRetry => retryButton.OnClickAsObservable();

        public IObservable<Unit> OnClickRanking => rankingButton.OnClickAsObservable();

        private readonly List<ResultDustItemCellView> cellList = new List<ResultDustItemCellView>();

        private ResultData ResultData { get; set; }

        private void Awake()
        {
            canvasGroup.alpha = 0;
        }

        public void Prepare(ResultData data)
        {
            ResultData = data;

            // 総重量
            sumWeightText.text = data.AllWeight.ToString("N");
            // 合計個数
            sumItemCount.text = data.DustItemList.Count.ToString();
            // 投げ入れた回数
            sumCarryingCountText.text = data.DustInCount.ToString();

            scoreText.text = "0";

            var groupBy = data.DustItemList.GroupBy(dustData => dustData.DustName);

            // セットアップ
            foreach (var group in groupBy)
            {
                var cell = Instantiate(view, holder);
                cell.Prepare(group.First(), group.Count());
                cellList.Add(cell);
            }
        }

        public async UniTask Open()
        {
            await canvasGroup.DOFade(1, 0.5f);

            await scoreText.DOCounter(0, (int) ResultData.Paying, 3f);
        }

        public async UniTask Close()
        {
            await canvasGroup.DOFade(0, 0.5f);
            Destroy(gameObject);
        }
    }
}