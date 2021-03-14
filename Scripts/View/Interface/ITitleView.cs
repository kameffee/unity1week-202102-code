using System;
using UniRx;

namespace View
{
    public interface ITitleView
    {
        IObservable<Unit> OnClickSettings { get; }

        IObservable<Unit> OnClickGameStart { get; }

        void AppealStartButton();
    }
}