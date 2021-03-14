using UnityEngine;

namespace View.Common
{
    public interface ISePlayer
    {
        bool IsPlaying { get; }

        void Play(AudioClip audioClip);

        void SetVolume(float volume);
    }
}