using System;
using System.Collections.Generic;
using System.Linq;
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

        public IObservable<string> OnConnectFailed => onConnectFailed.AddTo(Disposables);
        private readonly Subject<string> onConnectFailed = new Subject<string>();

        public IObservable<string> OnDisconnected => onDisconnected.AddTo(Disposables);
        private readonly Subject<string> onDisconnected = new Subject<string>();

        public bool IsRunning { get; private set; }

        protected CompositeDisposable Disposables { get; private set; } = new CompositeDisposable();

        protected void FireOnStarted()
        {
            if (IsRunning)
            {
                return;
            }
            if (Logger.IsDebug())
            {
                Logger.LogDebug("P2P started");
            }
            IsRunning = true;
            onStarted.OnNext(Unit.Default);
        }

        protected void FireOnConnectFailed(string reason)
        {
            if (Logger.IsDebug())
            {
                Logger.LogDebug($"{nameof(FireOnConnectFailed)}: reason={reason}");
            }
            onConnectFailed.OnNext(reason);
        }

        protected void FireOnDisconnected(string reason)
        {
            if (Logger.IsDebug())
            {
                Logger.LogDebug($"{nameof(FireOnDisconnected)}: reason={reason}");
            }
            if (reason == "io client disconnect")
            {
                return;
            }
            onDisconnected.OnNext(reason);
        }

        protected sealed override void ReleaseManagedResources()
        {
            DoReleaseManagedResources();
            Disposables.Dispose();
        }

        protected abstract void DoReleaseManagedResources();

        public async UniTask StartHostAsync(string name)
        {
            if (Logger.IsDebug())
            {
                Logger.LogDebug($"Start host: name={name}");
            }
            var startHostResponse = await DoStartHostAsync(name);
            if (startHostResponse.Status == 409)
            {
                if (Logger.IsDebug())
                {
                    Logger.LogDebug(startHostResponse.Message);
                }
                throw new HostNameAlreadyExistsException(startHostResponse.Message);
            }
            else
            {
                FireOnStarted();
            }
        }

        protected abstract UniTask<StartHostResponse> DoStartHostAsync(string name);

        public async UniTask<List<Host>> ListHostsAsync()
        {
            var listHostsResponse = await DoListHostsAsync();
            return listHostsResponse.Hosts.Select(host => new Host(host.Id, host.Name)).ToList();
        }

        protected abstract UniTask<ListHostsResponse> DoListHostsAsync();

        public UniTask StartClientAsync(string hostId)
        {
            if (Logger.IsDebug())
            {
                Logger.LogDebug($"Start client: hostId={hostId}");
            }
            return DoStartClientAsync(hostId);
        }

        protected abstract UniTask DoStartClientAsync(string hostId);

        public void Stop()
        {
            if (Logger.IsDebug())
            {
                Logger.LogDebug("Stop");
            }
            IsRunning = false;
            DoStopAsync().Forget();
        }

        protected abstract UniTask DoStopAsync();
    }
}
