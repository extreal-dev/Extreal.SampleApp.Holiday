using Cysharp.Threading.Tasks;
using Extreal.SampleApp.Holiday.App.Config;

namespace Extreal.SampleApp.Holiday.Controls.LandscapeControl.LandscapePlayers
{
    public interface ILandscapePlayerFactory
    {
        LandscapeType LandscapeType { get; }
        UniTask<ILandscapePlayer> CreateAsync(StageName stageName);
    }
}
