using System.Collections.Generic;
using Extreal.SampleApp.Holiday.App.Config;

namespace Extreal.SampleApp.Holiday.App
{
    public static class AppUtils
    {
        private static readonly HashSet<StageName> SpaceStages = new HashSet<StageName> { StageName.VirtualStage };

        public static bool IsSpace(StageName stageName) => SpaceStages.Contains(stageName);

        private static readonly string[] Unit = new string[] { "Bytes", "KB", "MB", "GB" };

        public static (long, string) GetSizeUnit(long size)
        {
            var count = 0;
            while (size > 1024)
            {
                size /= 1024;
                count++;
            }
            return (size, Unit[count]);
        }
    }
}
