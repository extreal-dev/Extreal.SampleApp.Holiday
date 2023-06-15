using System;

namespace Extreal.SampleApp.Holiday.App.AppUsage
{
    public interface IAppUsageCollector
    {
        public IDisposable Collect(Action<AppUsageBase> collect);
    }
}
