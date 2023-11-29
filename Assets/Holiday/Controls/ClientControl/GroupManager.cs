using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Cysharp.Threading.Tasks;
using Extreal.Core.Common.System;
using Extreal.Integration.Multiplay.LiveKit;
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

        private readonly PubSubMultiplayClient liveKitMultiplayClient;

        public GroupManager(PubSubMultiplayClient liveKitMultiplayClient) => this.liveKitMultiplayClient = liveKitMultiplayClient;

        protected override void ReleaseManagedResources() => disposables.Dispose();

        public async UniTask UpdateGroupsAsync()
        {
            var rooms = await liveKitMultiplayClient.ListRoomsAsync();
            groups.Value = rooms.Select(room => new Group("room.Sid", "room.Name")).ToList();
        }

        public Group FindByName(string name) => groups.Value.First(groups => groups.Name == name);
    }
}
