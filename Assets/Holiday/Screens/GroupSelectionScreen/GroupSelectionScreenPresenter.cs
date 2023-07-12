using System.Linq;
using Cysharp.Threading.Tasks;
using Extreal.Core.Logging;
using Extreal.Core.StageNavigation;
using Extreal.P2P.Dev;
using Extreal.SampleApp.Holiday.App;
using Extreal.SampleApp.Holiday.App.AssetWorkflow;
using Extreal.SampleApp.Holiday.App.Config;
using Extreal.SampleApp.Holiday.App.Stages;
using Extreal.SampleApp.Holiday.Controls.ClientControl;
using SocketIOClient;
using UniRx;

namespace Extreal.SampleApp.Holiday.Screens.GroupSelectionScreen
{
    public class GroupSelectionScreenPresenter : StagePresenterBase
    {
        private static readonly ELogger Logger = LoggingManager.GetLogger(nameof(GroupSelectionScreenPresenter));

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
                .Subscribe((groupName) => appState.SetGroupId(groupManager.FindByName(groupName)?.Id))
                .AddTo(sceneDisposables);

            groupSelectionScreenView.OnUpdateButtonClicked
                .Subscribe(async _ =>
                {
                    try
                    {
                        await groupManager.UpdateGroupsAsync();
                    }
                    catch (ConnectionException e)
                    {
                        if (Logger.IsDebug())
                        {
                            Logger.LogDebug(e.Message);
                        }
                        appState.Notify(assetHelper.MessageConfig.P2PStartFailureMessage);
                    }
                })
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
                    appState.SetGroupId(
                        groups.Count > 0 ? groupManager.FindByName(groupNames.FirstOrDefault()).Id : null);
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
            groupSelectionScreenView.SetInitialValues(appState.IsHost ? PeerRole.Host : PeerRole.Client);
        }
    }
}
