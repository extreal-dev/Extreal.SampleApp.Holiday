using System;
using UniRx;

namespace Extreal.SampleApp.Holiday.Controls.LandscapeControl.LandscapePlayers
{
    public interface ILandscapePlayer : IDisposable
    {
        IObservable<Unit> OnErrorOccurred { get; }
        void Play();
    }
}
