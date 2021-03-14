using Cysharp.Threading.Tasks;

namespace View
{
    public interface IStatusView
    {
        void RenderMoney(long money);

        void SetVisible(bool visible);

        UniTask Open();

        UniTask Close();
    }
}