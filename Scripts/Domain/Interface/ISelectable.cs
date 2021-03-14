using UniRx;

/// <summary>
/// プレイヤーが選択可能か
/// </summary>
public interface ISelectable
{
    IReadOnlyReactiveProperty<bool> Selected { get; }

    void SetSelected(bool isSelected);
}
