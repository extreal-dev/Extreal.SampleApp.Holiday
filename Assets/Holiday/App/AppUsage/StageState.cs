using System.Diagnostics;
using Extreal.SampleApp.Holiday.App.Config;

namespace Extreal.SampleApp.Holiday.App.AppUsage
{
    public class StageState
    {
        public StageName StageName => stageName;

        private readonly StageName stageName;
        private readonly Stopwatch stopwatch;
        private int numberOfTextChatsSent;

        public StageState(StageName stageName)
        {
            this.stageName = stageName;
            stopwatch = new Stopwatch();
            stopwatch.Start();
        }

        public void CountUpTextChats() => numberOfTextChatsSent++;

        public StageUsage ToStageUsage()
        {
            stopwatch.Stop();
            return new StageUsage
            {
                UsageId = nameof(StageUsage),
                StayTimeSeconds = stopwatch.ElapsedMilliseconds / 1000,
                NumberOfTextChatsSent = numberOfTextChatsSent
            };
        }
    }
}
