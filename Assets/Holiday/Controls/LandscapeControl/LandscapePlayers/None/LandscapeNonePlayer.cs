using Extreal.SampleApp.Holiday.App;

namespace Extreal.SampleApp.Holiday.Controls.LandscapeControl.LandscapePlayers.None
{
    public class LandscapeNonePlayer : LandscapePlayerBase
    {
        private readonly AppState appState;

        public LandscapeNonePlayer(AppState appState)
            => this.appState = appState;

        public override void Play()
            => appState.SetLandscapeInitialized(true);
    }
}
