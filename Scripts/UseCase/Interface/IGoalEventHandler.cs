using System;
using UniRx;

namespace UseCase
{
    public interface IGoalEventHandler
    {
        IObservable<Unit> OnGoal { get; }
    }
}