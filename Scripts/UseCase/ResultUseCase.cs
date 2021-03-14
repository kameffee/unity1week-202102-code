using System;
using System.Linq;
using Domain;
using naichilab;
using UniRx;
using UnityEngine.SceneManagement;
using VContainer;


namespace UseCase
{
    /// <summary>
    /// 結果表示ロジック
    /// </summary>
    public class ResultUseCase : IDisposable
    {
        private readonly StatusModel statusModel;
        private IGoalEventHandler goalEventHandler;

        // TODO: リザルトデータを流す.
        public IObservable<ResultData> OnResult => onResult;
        private Subject<ResultData> onResult = new Subject<ResultData>();

        public ResultData ResultData { get; private set; }

        private readonly CompositeDisposable compositeDisposable = new CompositeDisposable();

        [Inject]
        public ResultUseCase(IGoalEventHandler goalEventHandler, StatusModel statusModel)
        {
            this.statusModel = statusModel;
            goalEventHandler.OnGoal
                .Subscribe(_ => OpenResult())
                .AddTo(compositeDisposable);
        }

        public void OpenResult()
        {
            // 非表示にする
            statusModel.SetVisible(false);

            // 結果データ作成
            ResultData = new ResultData(
                statusModel.Paying.Value,
                statusModel.DustItems.ToList(),
                statusModel.DustInCount.Value);

            onResult.OnNext(ResultData);
        }

        public void OpenRanking()
        {
            RankingLoader.Instance.SendScoreAndShowRanking((double) ResultData.Paying);
        }

        /// <summary>
        /// リトライ
        /// </summary>
        public void Retry()
        {
            SceneManager.LoadSceneAsync("Main");
        }

        public void Dispose()
        {
            compositeDisposable?.Dispose();
        }
    }
}