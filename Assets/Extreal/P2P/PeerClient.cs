using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Extreal.Core.Common.System;
using Extreal.Core.Logging;
using UniRx;

namespace Extreal.P2P.Dev
{
    public abstract class PeerClient : DisposableBase
    {
        private static readonly ELogger Logger = LoggingManager.GetLogger(nameof(PeerClient));

        public IObservable<Unit> OnStarted => onStarted.AddTo(Disposables);
        private readonly Subject<Unit> onStarted = new Subject<Unit>();

        public bool IsRunning { get; private set; }
        public PeerRole Role { get; private set; } = PeerRole.None;
        public string HostId { get; private set; }

        protected PeerConfig PeerConfig { get; private set; }
        protected CompositeDisposable Disposables { get; private set; } = new CompositeDisposable();

        protected PeerClient(PeerConfig peerConfig)
        {
            PeerConfig = peerConfig;
            OnStarted.Subscribe(_ =>
            {
                if (Logger.IsDebug())
                {
                    Logger.LogDebug("P2P started");
                }
                IsRunning = true;
            }).AddTo(Disposables);
        }

        protected void FireOnStarted() => onStarted.OnNext(Unit.Default);

        protected sealed override void ReleaseManagedResources()
        {
            DoReleaseManagedResources();
            Disposables.Dispose();
        }

        protected abstract void DoReleaseManagedResources();

        public UniTask StartHostAsync(string name)
        {
            if (Logger.IsDebug())
            {
                Logger.LogDebug($"Start host: name={name}");
            }
            Role = PeerRole.Host;
            return DoStartHostAsync(name);
        }

        protected abstract UniTask DoStartHostAsync(string name);

        public abstract UniTask<List<Host>> ListHostsAsync();

        public UniTask StartClientAsync(string hostId)
        {
            if (Logger.IsDebug())
            {
                Logger.LogDebug($"Start client: hostId={HostId}");
            }
            Role = PeerRole.Client;
            HostId = hostId;
            return DoStartClientAsync();
        }

        protected abstract UniTask DoStartClientAsync();

        public void Stop()
        {
            if (Logger.IsDebug())
            {
                Logger.LogDebug("Stop");
            }
            IsRunning = false;
            Role = PeerRole.None;
            HostId = null;
            DoStopAsync().Forget();
        }

        protected abstract UniTask DoStopAsync();
    }
}
