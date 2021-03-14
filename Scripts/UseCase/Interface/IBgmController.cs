using UnityEngine;

namespace UseCase
{
    public interface IBgmController
    {
        bool IsPlaying { get; }

        void Play(AudioClip audioClip, float fadeInTime = 0f);
    }
}