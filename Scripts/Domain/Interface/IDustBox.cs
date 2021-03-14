using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Domain
{
    /// <summary>
    /// ゴミが入れられるところ
    /// </summary>
    public interface IDustBox
    {
        // todo: どうにかしたい
        Transform Entrance { get; set; }

        IObservable<DustModel> OnDustIn { get; }

        UniTask DustIn(IEnumerable<DustModel> dustList);
    }
}