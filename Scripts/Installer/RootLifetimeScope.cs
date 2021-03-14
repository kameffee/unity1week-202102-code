using Domain;
using Presenter;
using UnityEditor;
using UnityEngine;
using UseCase;
using VContainer;
using VContainer.Unity;
using View.Common;

public class RootLifetimeScope : LifetimeScope
{
    [SerializeField]
    private BgmPlayer bgmPlayer;
    
    [SerializeField]
    private SePlayer sePlayerPrefab;

    [SerializeField]
    private FadeView fadeCanvasPrefab;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.Register<SoundSettingsModel>(Lifetime.Singleton);

        builder.Register<BgmController>(Lifetime.Singleton).AsImplementedInterfaces();
        builder.RegisterComponentInNewPrefab(bgmPlayer, Lifetime.Singleton).AsImplementedInterfaces();

        builder.Register<ISeController, SeController>(Lifetime.Scoped);
        builder.RegisterFactory<ISePlayer>(container =>
        {
            return () => Instantiate(sePlayerPrefab);
        }, Lifetime.Scoped).AsImplementedInterfaces();
        
        // フェード
        builder.RegisterComponentInNewPrefab(fadeCanvasPrefab, Lifetime.Singleton).As<IFadeView>();
        builder.Register<FadeModel>(Lifetime.Singleton);
        builder.RegisterEntryPoint<FadePresenter>(Lifetime.Singleton);

        // グローバル
        builder.Register<GeneralStatusModel>(Lifetime.Singleton);
    }
}