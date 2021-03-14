using Domain;
using Master;
using Presenter;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using View;
using View.Common;

namespace Installer
{
    public class TitleLifetimeScope : LifetimeScope
    {
        [SerializeField]
        private AudioClip bgm;

        [SerializeField]
        private AudioClip testSe;

        [SerializeField]
        private SettingsDialogView settingsDialog;

        [SerializeField]
        private Transform canvas;

        [SerializeField]
        private TitleView titleView;

        [SerializeField]
        private DustMaster dustMaster;

        protected override void Configure(IContainerBuilder builder)
        {
            // マスター
            builder.RegisterInstance(dustMaster).AsImplementedInterfaces();

            // プレイヤー
            builder.Register<StatusModel>(Lifetime.Scoped).WithParameter(0L);
            builder.Register<CarryingModel>(Lifetime.Scoped);
            builder.Register<PlayerModel>(Lifetime.Scoped);
            builder.RegisterComponentInHierarchy<PlayerView>().AsImplementedInterfaces();
            builder.RegisterEntryPoint<MainPlayerPresenter>(Lifetime.Scoped);

            // ゴミ生成
            builder.Register<GenerateDustModel>(Lifetime.Scoped);
            builder.RegisterComponentInHierarchy<GenerateDustView>();

            // Settings
            builder.RegisterComponentInNewPrefab(settingsDialog, Lifetime.Scoped)
                .UnderTransform(canvas)
                .AsImplementedInterfaces();
            builder.Register<SettingsDialogPresenter>(Lifetime.Scoped)
                .WithParameter(testSe);

            // トラック
            builder.Register<AssetLoader<DustView>>(Lifetime.Scoped);
            builder.Register<GarbageTruckModel>(Lifetime.Scoped);
            builder.RegisterComponentInHierarchy<GarbageTruckView>().AsImplementedInterfaces();

            // Title
            builder.RegisterInstance(titleView).AsImplementedInterfaces();
            builder.RegisterEntryPoint<TitlePresenter>(Lifetime.Scoped).AsImplementedInterfaces();

            builder.RegisterEntryPoint<TitleEntryPoint>(Lifetime.Scoped)
                .WithParameter(bgm);
        }
    }
}