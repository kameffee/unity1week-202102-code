using System;
using System.Collections.Generic;
using System.Linq;
using Domain;
using UniRx;
using UnityEngine;
using VContainer;
using View.Common;

namespace UseCase
{
    public class SeController : ISeController
    {
        private readonly Func<ISePlayer> factory;
        private readonly List<ISePlayer> sePlayerList = new List<ISePlayer>();

        private readonly SoundSettingsModel settingsModel;
        private readonly CompositeDisposable compositeDisposable = new CompositeDisposable();

        [Inject]
        public SeController(Func<ISePlayer> factory, SoundSettingsModel settingsModel)
        {
            this.factory = factory;
            this.settingsModel = settingsModel;

            this.settingsModel.SeVolume
                .Subscribe(SetVolume)
                .AddTo(compositeDisposable);
        }

        public void Play(AudioClip audioClip)
        {
            GetReadyPlayer().Play(audioClip);
        }

        private void SetVolume(float volume)
        {
            foreach (var sePlayer in sePlayerList)
            {
                sePlayer.SetVolume(volume);
            }
        }

        private ISePlayer GetReadyPlayer()
        {
            var sePlayer = sePlayerList.FirstOrDefault(player => !player.IsPlaying);
            if (sePlayer == null)
            {
                sePlayer = factory.Invoke();
                sePlayerList.Add(sePlayer);
            }

            return sePlayer;
        }
    }
}