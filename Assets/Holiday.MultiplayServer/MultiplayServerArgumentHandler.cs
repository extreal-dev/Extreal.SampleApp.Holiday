using System;
using UnityEngine;

namespace Extreal.SampleApp.Holiday.MultiplayServer
{
    public static class MultiplayServerArgumentHandler
    {
        public static string MemoryUtilizationDumpFile { get; private set; }
        public static int MaxCapacity { get; private set; } = 90;
        public static float Lifetime { get; private set; }
        public static ushort Port { get; private set; } = 7777;

        private const string ExecCommand = "HolidayServer.x86_64";

        static MultiplayServerArgumentHandler()
            => ReadArgs();

        private static void ReadArgs()
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
                    case "-p":
                    case "--port":
                    {
                        if (!ushort.TryParse(args[++i], out var port))
                        {
                            DumpHelpWithErrorMessage();
                            return;
                        }
                        Port = port;
                        break;
                    }
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
                    case "--max-capacity":
                    {
                        if (!int.TryParse(args[++i], out var maxCapacity))
                        {
                            DumpHelpWithErrorMessage();
                            return;
                        }
                        if (maxCapacity > 0)
                        {
                            MaxCapacity = maxCapacity;
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
                        return;
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
                    + "  --port <ushort num>                  : Sets <ushort num> to the server port.\n"
                    + "    (also -p <ushort num>)               If not specified, the server port is set to 7777."
                    + "  --memory-utilization-dump-file <file>: Gets the memory utilization and dumps to the <file>.\n"
                    + "                                         If not specified, the memory utilization is not measured.\n"
                    + "  --max-capacity <int num>             : Max capacity of the clients that can connect to this server.\n"
                    + "                                         If not specified/input 0 or lower, it is set to 90.\n"
                    + "  --lifetime <float num>               : The server will exit in <float num> seconds.\n"
                    + "    (also -l <float num>)                If not specified/input 0 or lower, it does not exit until Ctrl+C is pressed.\n"
                    + "  --help (also -h)                     : Shows this help messages and exit.\n";

            Console.Error.WriteLine(helpMessage);
            Application.Quit();
        }
    }
}
