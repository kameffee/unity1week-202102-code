using UnityEngine;

namespace View
{
    public interface ICheckPoint
    {
        Vector3 Position { get; }

        float StopTime { get; }
    }
}