using Cinemachine;
using DG.Tweening;

namespace View
{
    public static class CinemachineVirtualCameraEx
    {
        public static Tweener DOOrthographicSize(this CinemachineVirtualCamera camera, float from, float to, float duration)
        {
            return DOVirtual.Float(from, to, duration, value =>
            {
                camera.m_Lens.OrthographicSize = value;
            });
        }
    }
}