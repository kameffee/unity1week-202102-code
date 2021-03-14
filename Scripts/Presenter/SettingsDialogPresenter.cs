using System;
using Domain;
using UniRx;
using UnityEngine;
using UseCase;
using VContainer;
using VContainer.Unity;
using View.Common;

namespace Presenter
{
    /// <summary>
    /// 設定ダイアログ
    /// </summary>
    public class SettingsDialogPresenter : IDisposable
    {
        private readonly SoundSettingsModel soundSettingsModel;
        private readonly ISoundSettingsView view;
        private readonly ISeController seController;

        private readonly CompositeDisposable compositeDisposable = new CompositeDisposable();

        [Inject]
        public SettingsDialogPresenter(
            SoundSettingsModel soundSettingsModel,
            ISoundSettingsView view,
            ISeController seController,
            AudioClip testSeClip)
        {
            this.soundSettingsModel = soundSettingsModel;
            this.view = view;
            this.seController = seController;

            this.soundSettingsModel.BGMVolume
                .Subscribe(view.SetBgmVolume)
                .AddTo(compositeDisposable);

            this.view.OnChangeBgmVolume
                .Subscribe(value => this.soundSettingsModel.SetBgmVolume(value));

            this.soundSettingsModel.SeVolume
                .Subscribe(view.SetSeVolume)
                .AddTo(compositeDisposable);

            this.view.OnChangeSeVolume
                .Subscribe(value => this.soundSettingsModel.SetSeVolume(value))
                .AddTo(compositeDisposable);

            this.view.OnSePointerUp
                .Subscribe(_ => seController.Play(testSeClip))
                .AddTo(compositeDisposable);
        }

        public void Open()
        {
            view.Open();
        }

        public void Dispose()
        {
            compositeDisposable?.Dispose();
        }
    }
}