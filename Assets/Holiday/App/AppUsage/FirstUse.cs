using System.Diagnostics.CodeAnalysis;

namespace Extreal.SampleApp.Holiday.App.AppUsage
{
    [SuppressMessage("Usage", "IDE1006")]
    public class FirstUse : AppUsageBase
    {
        public string OS;
        public string DeviceModel;
        public string DeviceType;
        public string DeviceId;
        public string ProcessorType;
    }
}
