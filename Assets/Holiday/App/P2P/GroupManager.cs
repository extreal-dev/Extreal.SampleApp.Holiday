using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Extreal.Core.Common.System;
using UniRx;

namespace Extreal.SampleApp.Holiday.App.P2P
{
    public class GroupManager : DisposableBase
    {
        public IObservable<List<Group>> OnGroupsUpdated => groups.AddTo(disposables).Skip(1);
        [SuppressMessage("Usage", "CC0033")]
        private readonly ReactiveProperty<List<Group>> groups = new ReactiveProperty<List<Group>>(new List<Group>());

        public IObservable<Unit> OnGroupsUpdateFailed => onGroupsUpdateFailed.AddTo(disposables);
        [SuppressMessage("Usage", "CC0033")]
        private readonly Subject<Unit> onGroupsUpdateFailed = new Subject<Unit>();

        [SuppressMessage("Usage", "CC0033")]
        private readonly CompositeDisposable disposables = new CompositeDisposable();

        protected override void ReleaseManagedResources() => disposables.Dispose();

        public void UpdateGroups()
        {
            var result = new List<Group>
            {
                new Group("111", "Group 1"),
                new Group("222", "Group 2"),
                new Group("333", "Group 3"),
            };
            groups.Value = result;
        }

        public Group FindByName(string name) => groups.Value.First(groups => groups.Name == name);

        public class Group
        {
            public string Id { get; private set; }
            public string Name { get; private set; }
            public Group(string id, string name)
            {
                Id = id;
                Name = name;
            }
        }
    }
}
