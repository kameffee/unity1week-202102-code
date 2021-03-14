using System;
using UniRx;
using UnityEngine;

namespace Domain
{
    public class SoundSettingsModel : IDisposable
    {
        public IReadOnlyReactiveProperty<float> BGMVolume => bgmVolume;
        private readonly ReactiveProperty<float> bgmVolume = new ReactiveProperty<float>(0.6f);

        public ReactiveProperty<float> SeVolume => seVolume;
        private readonly ReactiveProperty<float> seVolume = new ReactiveProperty<float>(1);

        public void SetBgmVolume(float volume) => bgmVolume.Value = Mathf.Clamp01(volume);

        public void SetSeVolume(float volume) => seVolume.Value = Mathf.Clamp01(volume);

        public void Dispose()
        {
            seVolume?.Dispose();
            bgmVolume?.Dispose();
        }
    }
}