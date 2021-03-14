using System;
using UnityEngine;

namespace View.Common
{
    public class SePlayer : MonoBehaviour, ISePlayer
    {
        private AudioSource AudioSource => audioSource ?? (audioSource = GetComponent<AudioSource>());
        private AudioSource audioSource;

        private void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
        }

        public bool IsPlaying => AudioSource.isPlaying;

        public void Play(AudioClip audioClip)
        {
            AudioSource.PlayOneShot(audioClip);
        }

        public void SetVolume(float volume)
        {
            AudioSource.volume = volume;
        }
    }
}