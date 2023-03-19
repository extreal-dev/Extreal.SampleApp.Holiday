using System;
using UnityEngine;

namespace Extreal.SampleApp.Holiday.PerformanceTest
{
    public static class PerformanceTestArgumentHandler
    {
        public static string MemoryUtilizationDumpFile { get; private set; }
        public static int SendMessagePeriod { get; private set; }
        public static float Lifetime { get; private set; }

        private const string ExecCommand = nameof(Holiday);

        static PerformanceTestArgumentHandler()
        {
            var args = Environment.GetCommandLineArgs();
            var argLength = args.Length;
            if (argLength == 1)
            {
                return;
            }

            for (var i = 1; i < argLength; i++)
            {
                switch (args[i])
                {
                    case "--memory-utilization-dump-file":
                    {
                        MemoryUtilizationDumpFile = args[++i];
                        if (MemoryUtilizationDumpFile.StartsWith('-'))
                        {
                            DumpHelpWithErrorMessage();
                            return;
                        }
                        break;
                    }
                    case "--send-message-period":
                    {
                        if (!float.TryParse(args[++i], out var period))
                        {
                            DumpHelpWithErrorMessage();
                            return;
                        }
                        if (period > 0f)
                        {
                            SendMessagePeriod = (int)(period * 5400);
                        }

                        break;
                    }
                    case "-l":
                    case "--lifetime":
                    {
                        if (!float.TryParse(args[++i], out var lifetime))
                        {
                            DumpHelpWithErrorMessage();
                            return;
                        }
                        if (lifetime > 0)
                        {
                            Lifetime = lifetime;
                        }
                        break;
                    }
                    case "-h":
                    case "--help":
                    {
                        DumpHelp();
                        break;
                    }
                    default:
                    {
                        DumpHelpWithErrorMessage();
                        return;
                    }
                }
            }
        }

        private static void DumpHelp()
            => DumpHelpWithErrorMessage(null);

        private static void DumpHelpWithErrorMessage(string errorMessage = "Unexpected argument was input.")
        {
            var helpMessage = string.IsNullOrEmpty(errorMessage) ? string.Empty : errorMessage + "\n\n";
            helpMessage
                += $"Usage: {ExecCommand} [OPTION]...\n"
                    + "\n"
                    + "options:\n"
                    + "  --memory-utilization-dump-file <file>: Gets the memory utilization and dumps to the <file>.\n"
                    + "                                         If not specified, the memory utilization is not measured.\n"
                    + "  --send-message-period <float num>    : A total of 90 clients send a message approximately once every <float num> seconds.\n"
                    + "                                         If not specified/input 0 or lower, no message is sent.\n"
                    + "  --lifetime <float num>               : The performance test will exit in <float num> seconds.\n"
                    + "    (also -l <float num>)                If not specified/input 0 or lower, it does not exit until Ctrl+C is pressed.\n"
                    + "  --help (also -h)                     : Shows this help messages and exit.";

            Console.Error.WriteLine(helpMessage);
            Application.Quit();
        }
    }
}
