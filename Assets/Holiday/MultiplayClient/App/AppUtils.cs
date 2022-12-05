using System.Collections.Generic;

namespace Extreal.SampleApp.Holiday.MultiplayClient.App
{
    public static class AppUtils
    {
        private static readonly HashSet<StageName> SpaceStages = new HashSet<StageName> { StageName.VirtualStage };

        public static bool IsSpace(StageName stageName) => SpaceStages.Contains(stageName);
    }
}
