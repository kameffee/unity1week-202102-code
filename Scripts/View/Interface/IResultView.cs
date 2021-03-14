using System;
using Cysharp.Threading.Tasks;
using Domain;
using UniRx;

namespace View
{
    public interface IResultView
    {
        void Prepare(ResultData data);

        UniTask Open();

        UniTask Close();

        IObservable<Unit> OnClickRetry { get; }

        IObservable<Unit> OnClickRanking { get; }
    }
}