using System;
using Extreal.Core.Common.System;
using UniRx;

namespace Extreal.Chat.Dev
{
    public abstract class VoiceChatClient : DisposableBase
    {
        protected CompositeDisposable Disposables { get; private set; } = new CompositeDisposable();

        public IObservable<bool> OnMuted => onMuted.AddTo(Disposables);
        private readonly ReactiveProperty<bool> onMuted = new ReactiveProperty<bool>();

        protected override void ReleaseManagedResources() => Disposables.Dispose();

        protected void FireOnMuted(bool muted) => onMuted.Value = muted;

        public abstract void ToggleMute();

        public abstract void Clear();
    }
}
