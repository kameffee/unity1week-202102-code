using Cysharp.Threading.Tasks;

namespace View.Common
{
    public interface IFadeView
    {
        void Initialize(bool isOut);

        // 画面を隠す
        UniTask FadeOut(float duration = 1f);

        // 画面を表示
        UniTask FadeIn(float duration = 1f);
    }
}