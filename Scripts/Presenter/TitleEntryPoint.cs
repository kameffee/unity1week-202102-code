using Domain;
using UnityEngine;
using UseCase;
using VContainer.Unity;

namespace Installer
{
    public class TitleEntryPoint : IStartable
    {
        private readonly IBgmController bgmController;
        private readonly AudioClip bgmClip;
        private readonly PlayerModel playerModel;
        private readonly FadeModel fadeModel;

        public TitleEntryPoint(IBgmController bgmController, AudioClip bgm, PlayerModel playerModel, FadeModel fadeModel)
        {
            this.bgmController = bgmController;
            this.bgmClip = bgm;
            this.playerModel = playerModel;
            this.fadeModel = fadeModel;
        }

        public void Start()
        {
            Debug.Log("Start title");
            this.bgmController.Play(bgmClip, 1f);

            playerModel.SetMovable(true);
        }
    }
}
