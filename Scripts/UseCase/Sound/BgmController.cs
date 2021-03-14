using System;
using Cysharp.Threading.Tasks;
using Domain;
using UniRx;
using UnityEngine;
using View.Common;

namespace UseCase
{
    public class BgmController : IBgmController, IDisposable
    {
        private readonly IBgmPlayer bgmPlayer;

        private readonly SoundSettingsModel settingsModel;

        private readonly IDisposable disposable;

        public bool IsPlaying => bgmPlayer.IsPlaying;

        public BgmController(IBgmPlayer bgmPlayer, SoundSettingsModel settingsModel)
        {
            this.bgmPlayer = bgmPlayer;
            this.settingsModel = settingsModel;

            disposable = this.settingsModel.BGMVolume
                .Subscribe(bgmPlayer.SetVolume);
        }

        public void Play(AudioClip audioClip, float fadeInTime = 0)
        {
            this.bgmPlayer.Play(audioClip, fadeInTime);
        }

        public async UniTask Stop(float fadeOutTime = 0f)
        {
            await this.bgmPlayer.Stop(fadeOutTime);
        }

        public void Dispose()
        {
            disposable?.Dispose();
        }
    }
}