using Cysharp.Threading.Tasks;
using Extreal.SampleApp.Holiday.App.Config;
using System.Collections.Generic;

namespace Extreal.SampleApp.Holiday.Controls.LandscapeControl.LandscapePlayers
{
    public class LandscapePlayerManager
    {
        private readonly Dictionary<LandscapeType, ILandscapePlayerFactory> landscapePlayerFactories
            = new Dictionary<LandscapeType, ILandscapePlayerFactory>();

        public LandscapePlayerManager(IEnumerable<ILandscapePlayerFactory> landscapePlayerFactories)
        {
            foreach (var landscapePlayerFactory in landscapePlayerFactories)
            {
                this.landscapePlayerFactories[landscapePlayerFactory.LandscapeType] = landscapePlayerFactory;
            }
        }

        public async UniTask<ILandscapePlayer> CreateAsync(StageName stageName, LandscapeType landscapeType)
            => await landscapePlayerFactories[landscapeType].CreateAsync(stageName);
    }
}
