using Extreal.Core.Logging;
using Extreal.Core.StageNavigation;
using Extreal.SampleApp.Holiday.App;
using Extreal.SampleApp.Holiday.App.AssetWorkflow;
using Extreal.SampleApp.Holiday.App.Config;
using Extreal.SampleApp.Holiday.Spaces.Common;
using UnityEngine;

namespace Extreal.SampleApp.Holiday.Spaces.VirtualSpace
{
    public class VirtualSpacePresenter : SpacePresenterBase
    {
        private readonly AssetHelper assetHelper;

        private GameObject sceneAsset;

        private static readonly ELogger Logger = LoggingManager.GetLogger(nameof(VirtualSpacePresenter));

        public VirtualSpacePresenter
        (
            StageNavigator<StageName, SceneName> stageNavigator,
            AppState appState
        ) : base(stageNavigator, appState)
        {
        }

    }
}