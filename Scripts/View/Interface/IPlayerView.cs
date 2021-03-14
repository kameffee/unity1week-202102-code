using UnityEngine;

namespace View
{
    public interface IPlayerView
    {
        Transform CarryingHolder { get; }

        Collider2D CatchTrigger { get; }

        Collider2D FootTrigger { get; }

        /// <summary>
        /// プレイヤーの向き
        /// </summary>
        /// <param name="direction"></param>
        void SetDirection(int direction);

        void SetWalk(bool isWalk);

        void SetCarry(bool isCarry);

        void SetStan();

        Vector3 Translate(Vector3 vector3);

        public void StanImpulse();
    }
}