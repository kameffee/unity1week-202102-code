using Cysharp.Threading.Tasks;

namespace View
{
    public interface IMessageWindow
    {
        UniTask Open();

        UniTask Close();

        UniTask StartMessage(string message);

        void SetMessage(string message);
    }
}