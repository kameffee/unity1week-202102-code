using Cysharp.Threading.Tasks;

namespace View
{
    public interface ICinemaScopeView
    {
        UniTask Show(float duration);

        UniTask Hide(float duration);
    }
}