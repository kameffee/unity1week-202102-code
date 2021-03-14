using UnityEngine;

namespace View
{
    public interface IGarbageTruck
    {
        Transform Entrance { get; }

        Transform Body { get; }

        Collider2D ScrapEntrance { get; }
    }
}