using System;
using Cysharp.Threading.Tasks;

namespace View.Common
{
    public interface ISoundSettingsView
    {
        IObservable<float> OnChangeBgmVolume { get; }

        IObservable<float> OnChangeSeVolume { get; }

        IObservable<float> OnSePointerUp { get; }

        void SetBgmVolume(float volume);

        void SetSeVolume(float volume);

        UniTask Open();

        UniTask Close();
    }
}