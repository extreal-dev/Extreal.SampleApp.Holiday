using Extreal.SampleApp.Holiday.App.Config;

namespace Extreal.SampleApp.Holiday.Controls.LandscapeControl.LandscapePlayers
{
    public interface ILandscapePlayerFactory
    {
        LandscapeType LandscapeType { get; }
        ILandscapePlayer Create(StageName stageName);
    }
}
