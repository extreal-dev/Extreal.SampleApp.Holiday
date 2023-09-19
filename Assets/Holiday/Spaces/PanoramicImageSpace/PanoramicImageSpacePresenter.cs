using Extreal.Core.StageNavigation;
using Extreal.SampleApp.Holiday.App;
using Extreal.SampleApp.Holiday.App.Config;
using Extreal.SampleApp.Holiday.Spaces.Common;

namespace Extreal.SampleApp.Holiday.Spaces.ForestSpace
{
    public class PanoramicImageSpacePresenter : SpacePresenterBase
    {
        public PanoramicImageSpacePresenter
        (
            StageNavigator<StageName, SceneName> stageNavigator,
            AppState appState
        ) : base(stageNavigator, appState)
        {
        }
    }
}
