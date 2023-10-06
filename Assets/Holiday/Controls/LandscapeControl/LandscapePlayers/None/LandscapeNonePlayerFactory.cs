using Extreal.SampleApp.Holiday.App;
using Extreal.SampleApp.Holiday.App.Config;

namespace Extreal.SampleApp.Holiday.Controls.LandscapeControl.LandscapePlayers.None
{
    public class LandscapeNonePlayerFactory : ILandscapePlayerFactory
    {
        public LandscapeType LandscapeType => LandscapeType.None;

        private readonly AppState appState;

        public LandscapeNonePlayerFactory(AppState appState)
            => this.appState = appState;

        public ILandscapePlayer Create(StageName stageName)
            => new LandscapeNonePlayer(appState);
    }
}
