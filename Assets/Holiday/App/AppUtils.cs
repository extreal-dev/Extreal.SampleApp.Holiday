using System.Collections.Generic;
using Extreal.SampleApp.Holiday.App.Config;

namespace Extreal.SampleApp.Holiday.App
{
    public static class AppUtils
    {
        private static readonly HashSet<StageName> SpaceStages = new HashSet<StageName> { StageName.VirtualStage };

        public static bool IsSpace(StageName stageName) => SpaceStages.Contains(stageName);
    }
}
