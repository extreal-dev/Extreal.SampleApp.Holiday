using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Cysharp.Threading.Tasks;
using Extreal.Core.Common.System;
using Extreal.Integration.Multiplay.Common;
using Extreal.Integration.P2P.WebRTC;
using Extreal.SampleApp.Holiday.App;
using UniRx;

namespace Extreal.SampleApp.Holiday.Controls.ClientControl
{
    public class GroupManager : DisposableBase
    {
        public IObservable<List<Group>> OnGroupsUpdated => groups.AddTo(disposables).Skip(1);
        [SuppressMessage("Usage", "CC0033")]
        private readonly ReactiveProperty<List<Group>> groups = new ReactiveProperty<List<Group>>(new List<Group>());

        [SuppressMessage("Usage", "CC0033")]
        private readonly CompositeDisposable disposables = new CompositeDisposable();

        private readonly PeerClient peerClient;
        private readonly ExtrealMultiplayClient pubSubMultiplayClient;
        private readonly AppState appState;

        public GroupManager(PeerClient peerClient, ExtrealMultiplayClient pubSubMultiplayClient, AppState appState)
        {
            this.peerClient = peerClient;
            this.pubSubMultiplayClient = pubSubMultiplayClient;
            this.appState = appState;
        }

        protected override void ReleaseManagedResources() => disposables.Dispose();

        public async UniTask UpdateGroupsAsync()
        {
            if (appState.IsLightForCommunication)
            {
                var hosts = await peerClient.ListHostsAsync();
                groups.Value = hosts.Select(host => new Group(host.Id, host.Name)).ToList();
            }
            else
            {
                var rooms = await pubSubMultiplayClient.ListRoomsAsync();
                groups.Value = rooms.Select(room => new Group(room.Id, room.Name)).ToList();
            }
        }

        public Group FindByName(string name) => groups.Value.First(groups => groups.Name == name);
    }
}
