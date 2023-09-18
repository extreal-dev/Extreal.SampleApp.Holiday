using System.Linq;
using Cysharp.Threading.Tasks;
using Extreal.Core.StageNavigation;
using Extreal.Integration.P2P.WebRTC;
using Extreal.SampleApp.Holiday.App;
using Extreal.SampleApp.Holiday.App.AssetWorkflow;
using Extreal.SampleApp.Holiday.App.Config;
using Extreal.SampleApp.Holiday.App.Stages;
using Extreal.SampleApp.Holiday.Controls.ClientControl;
using UniRx;
using UnityEngine;

namespace Extreal.SampleApp.Holiday.Screens.GroupSelectionScreen
{
    public class GroupSelectionScreenPresenter : StagePresenterBase
    {
        private readonly PeerClient peerClient;
        private readonly GroupManager groupManager;
        private readonly GroupSelectionScreenView groupSelectionScreenView;
        private readonly AssetHelper assetHelper;

        public GroupSelectionScreenPresenter
        (
            StageNavigator<StageName, SceneName> stageNavigator,
            AppState appState,
            AssetHelper assetHelper,
            PeerClient peerClient,
            GroupManager groupManager,
            GroupSelectionScreenView groupSelectionScreenView
        ) : base(stageNavigator, appState)
        {
            this.peerClient = peerClient;
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
                .Subscribe((groupName) => appState.SetGroupId(groupManager.FindByName(groupName)?.Id))
                .AddTo(sceneDisposables);

            groupSelectionScreenView.OnUpdateButtonClicked
                .Subscribe(_ => groupManager.UpdateGroupsAsync().Forget())
                .AddTo(sceneDisposables);

            groupSelectionScreenView.OnGoButtonClicked
                .Subscribe(_ =>
                {
                    appState.SetSpaceName("PanoramicImageSpace");
                    appState.SetSpace(assetHelper.SpaceConfig.Spaces.Find(space => space.SpaceName == "PanoramicImageSpace"));
                    stageNavigator.ReplaceAsync(StageName.PanoramicImageStage).Forget();
                }
                )
                .AddTo(sceneDisposables);

            groupSelectionScreenView.OnBackButtonClicked
                .Subscribe(_ => stageNavigator.ReplaceAsync(StageName.AvatarSelectionStage).Forget())
                .AddTo(sceneDisposables);

            groupManager.OnGroupsUpdated
                .Subscribe(groups =>
                {
                    var groupNames = groups.Select(group => group.Name).ToArray();
                    groupSelectionScreenView.UpdateGroupNames(groupNames);
                    appState.SetGroupId(
                        groups.Count > 0 ? groups.First().Id : null);
                })
                .AddTo(sceneDisposables);

            peerClient.OnConnectFailed
                .Subscribe(_ => appState.Notify(assetHelper.MessageConfig.GroupMatchingUpdateFailureMessage))
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
            groupSelectionScreenView.SetInitialValues(appState.IsHost ? PeerRole.Host : PeerRole.Client);
        }
    }
}
