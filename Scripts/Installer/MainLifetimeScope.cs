using System;
using Cinemachine;
using Domain;
using Master;
using Presenter;
using UnityEngine;
using UseCase;
using VContainer;
using VContainer.Unity;
using View;

public class MainLifetimeScope : LifetimeScope
{
    [SerializeField]
    private ResultView resultPrefab;

    [SerializeField]
    private DustMaster dustMaster;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.Register<IDustItemDatabase, DustItemDatabase>(Lifetime.Singleton);
        builder.Register<IMessageDatabase, MessageDatabase>(Lifetime.Singleton);
        builder.Register<MessageEventUseCase>(Lifetime.Singleton);

        // マスター
        builder.RegisterInstance(dustMaster).AsImplementedInterfaces();

        // キャッシュ
        builder.Register<AssetLoader<DustView>>(Lifetime.Scoped);

        // カメラ
        builder.RegisterComponentInHierarchy<CinemachineVirtualCamera>();

        // 黒帯
        builder.RegisterComponentInHierarchy<CinemaScopeView>().AsImplementedInterfaces();
        builder.Register<CinemaScopeModel>(Lifetime.Scoped);
        builder.RegisterEntryPoint<CinemaScopePresenter>(Lifetime.Scoped);

        // メッセージ
        builder.Register<MessageWindowPresenter>(Lifetime.Scoped).AsImplementedInterfaces();
        builder.Register<MessageModel>(Lifetime.Scoped);
        builder.RegisterComponentInHierarchy<MessageWindow>().AsImplementedInterfaces();

        // プレイヤー
        builder.Register<CarryingModel>(Lifetime.Scoped);
        builder.Register<PlayerModel>(Lifetime.Scoped);
        builder.RegisterComponentInHierarchy<PlayerView>().As<IPlayerView>();
        builder.RegisterEntryPoint<MainPlayerPresenter>(Lifetime.Scoped);

        // ゴミ生成
        builder.Register<GenerateDustModel>(Lifetime.Scoped);
        builder.RegisterComponentInHierarchy<GenerateDustView>();

        // Todo: listクラスを作る
        foreach (var checkPoint in FindObjectsOfType<CheckPoint>())
        {
            builder.RegisterInstance(checkPoint).As<ICheckPoint>();
        }

        // トラック
        builder.Register<GarbageTruckModel>(Lifetime.Scoped);
        builder.RegisterComponentInHierarchy<GarbageTruckView>().AsImplementedInterfaces();

        // ステータス
        builder.Register<StatusModel>(Lifetime.Scoped).WithParameter("initialPaying", 0L);
        builder.RegisterComponentInHierarchy<StatusView>().AsImplementedInterfaces();
        builder.Register<StatusPresenter>(Lifetime.Scoped).AsImplementedInterfaces();

        // リザルト
        builder.RegisterFactory<IResultView>(container => { return () => Instantiate(resultPrefab); }, Lifetime.Scoped);
        builder.Register<ResultUseCase>(Lifetime.Scoped);
        builder.RegisterEntryPoint<ResultPresenter>(Lifetime.Scoped);

        builder.RegisterEntryPoint<GameLoop>(Lifetime.Scoped).AsImplementedInterfaces();
    }
}
