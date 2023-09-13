using Cysharp.Threading.Tasks;
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

#pragma warning disable CS1998
        public async UniTask<ILandscapePlayer> CreateAsync(StageName stageName)
            => new LandscapeNonePlayer(appState);
#pragma warning restore CS1998
    }
}
