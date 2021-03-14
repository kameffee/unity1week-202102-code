using UnityEngine;

namespace View
{
    public class CheckPoint : MonoBehaviour, ICheckPoint
    {
        public Vector3 Position => transform.position;

        // 停止時間
        [SerializeField]
        private float stopTime;

        public float StopTime => stopTime;
    }
}