using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Extreal.SampleApp.Holiday
{
    public interface IMultipayStrategy
    {
        void Initialize(Avatar avatar, bool isOwner, bool isTouchDevice);
        void ResetPosition();
        void DoLateUpdate();
    }
}
