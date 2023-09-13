using System;
using System.Diagnostics.CodeAnalysis;
using Extreal.Core.Common.System;
using UniRx;

namespace Extreal.SampleApp.Holiday.Controls.LandscapeControl.LandscapePlayers
{
    public abstract class LandscapePlayerBase : DisposableBase, ILandscapePlayer
    {
        public IObservable<Unit> OnErrorOccurred => OnErrorOccurredSubject;
        [SuppressMessage("Usage", "CC0022")]
        protected Subject<Unit> OnErrorOccurredSubject { get; } = new Subject<Unit>();

        public abstract void Play();

        protected override void ReleaseManagedResources()
            => OnErrorOccurredSubject.Dispose();
    }
}
