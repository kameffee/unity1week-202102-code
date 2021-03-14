using System;
using UniRx;

namespace Domain
{
    public interface IDustModel : ICarryable, ISelectable
    {
        IObservable<Unit> OnTrash { get; }
    }
}
