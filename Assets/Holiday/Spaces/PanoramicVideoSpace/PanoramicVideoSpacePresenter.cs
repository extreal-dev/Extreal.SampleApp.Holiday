using Extreal.Core.StageNavigation;
using Extreal.SampleApp.Holiday.App;
using Extreal.SampleApp.Holiday.App.Config;
using Extreal.SampleApp.Holiday.Spaces.Common;

namespace Extreal.SampleApp.Holiday.Spaces.PanoramicVideoSpace
{
    public class PanoramicVideoSpacePresenter : SpacePresenterBase
    {
        public PanoramicVideoSpacePresenter
        (
            StageNavigator<StageName, SceneName> stageNavigator,
            AppState appState
        ) : base(stageNavigator, appState)
        {
        }
    }
}
