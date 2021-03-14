using UniRx;
using UnityEngine;

namespace Domain
{
    /// <summary>
    /// 運べるもの
    /// </summary>
    public interface ICarryable
    {
        Transform Transform { get; }

        IReadOnlyReactiveProperty<bool> Carrying { get; }

        void SetCarrying(bool isCarrying);
    }
}
