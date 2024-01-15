using System.Linq;
using Cysharp.Threading.Tasks;
using Extreal.Core.StageNavigation;
using Extreal.SampleApp.Holiday.App;
using Extreal.SampleApp.Holiday.App.AssetWorkflow;
using Extreal.SampleApp.Holiday.App.Config;
using Extreal.SampleApp.Holiday.App.Group;
using Extreal.SampleApp.Holiday.App.Stages;
using Extreal.SampleApp.Holiday.Controls.ClientControl;
using UniRx;

namespace Extreal.SampleApp.Holiday.Screens.GroupSelectionScreen
{
    public class GroupSelectionScreenPresenter : StagePresenterBase
    {
        private readonly GroupManager groupManager;
        private readonly GroupSelectionScreenView groupSelectionScreenView;
        private readonly AssetHelper assetHelper;

        public GroupSelectionScreenPresenter
        (
            StageNavigator<StageName, SceneName> stageNavigator,
            AppState appState,
            AssetHelper assetHelper,
            GroupManager groupManager,
            GroupSelectionScreenView groupSelectionScreenView
        ) : base(stageNavigator, appState)
        {
            this.groupManager = groupManager;
            this.groupSelectionScreenView = groupSelectionScreenView;
            this.assetHelper = assetHelper;
        }

        protected override void Initialize
        (
            StageNavigator<StageName, SceneName> stageNavigator,
            AppState appState,
            CompositeDisposable sceneDisposables
        )
        {
            groupSelectionScreenView.OnRoleChanged
                .Subscribe(appState.SetRole)
                .AddTo(sceneDisposables);

            groupSelectionScreenView.OnGroupNameChanged
                .Subscribe(appState.SetGroupName)
                .AddTo(sceneDisposables);

            groupSelectionScreenView.OnGroupChanged
                .Subscribe((groupName) =>
                {
                    appState.SetGroupId(groupManager.FindByName(groupName)?.Id);
                    appState.SetGroupName(groupName);
                })
                .AddTo(sceneDisposables);

            groupSelectionScreenView.OnUpdateButtonClicked
                .Subscribe(_ => groupManager.UpdateGroupsAsync().Forget())
                .AddTo(sceneDisposables);

            groupSelectionScreenView.OnGoButtonClicked
                .Subscribe(_ => assetHelper.DownloadSpaceAsset("VirtualSpace", StageName.VirtualStage))
                .AddTo(sceneDisposables);

            groupSelectionScreenView.OnBackButtonClicked
                .Subscribe(_ => stageNavigator.ReplaceAsync(StageName.AvatarSelectionStage).Forget())
                .AddTo(sceneDisposables);

            groupManager.OnGroupsUpdated
                .Subscribe(groups =>
                {
                    var groupNames = groups.Select(group => group.Name).ToArray();
                    groupSelectionScreenView.UpdateGroupNames(groupNames);
                    if (groups.Count > 0)
                    {
                        appState.SetGroupName(groups.First().Name);
                        appState.SetGroupId(groups.First().Id);
                    }
                })
                .AddTo(sceneDisposables);
        }

        protected override void OnStageEntered
        (
            StageName stageName,
            AppState appState,
            CompositeDisposable stageDisposables
        )
        {
            groupSelectionScreenView.Initialize();
            var role = appState.IsHost ? GroupRole.Host : GroupRole.Client;
            groupSelectionScreenView.SetInitialValues(role);
        }
    }
}
