using Cysharp.Threading.Tasks;
using UnityEngine;

namespace View.Common
{
    public interface IBgmPlayer
    {
        bool IsPlaying { get; }

        void Play(float fadeInTime = 0);

        void Play(AudioClip audioClip, float fadeInTime = 0);

        UniTask Stop(float fadeOutTime = 0f);

        void SetVolume(float volume);
    }
}