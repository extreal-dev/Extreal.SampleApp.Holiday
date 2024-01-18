using System;
using UnityEngine;

namespace Extreal.SampleApp.Holiday.PerformanceTest
{
    public static class PerformanceTestArgumentHandler
    {
        public static string MemoryUtilizationDumpFile { get; private set; }
        public static int SendMessagePeriod { get; private set; } = 5;
        public static float Lifetime { get; private set; }
        public static Role Role { get; private set; }
        public static string GroupName { get; private set; } = "TestGroup";
        public static int GroupCapacity { get; private set; }
        public static string SpaceName { get; private set; }
        public static bool SuppressMultiplayer { get; private set; }
        public static bool SuppressTextChat { get; private set; }

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
                        i++;
                        if (i == argLength || args[i].StartsWith('-'))
                        {
                            DumpHelpWithErrorMessage();
                            return;
                        }
                        MemoryUtilizationDumpFile = args[i];
                        break;
                    }
                    case "--send-message-period":
                    {
                        i++;
                        if (i == argLength || !int.TryParse(args[i], out var period))
                        {
                            DumpHelpWithErrorMessage();
                            return;
                        }
                        if (period > 0f)
                        {
                            SendMessagePeriod = period;
                        }

                        break;
                    }
                    case "-l":
                    case "--lifetime":
                    {
                        i++;
                        if (i == argLength || !float.TryParse(args[i], out var lifetime))
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
                    case "-r":
                    case "--role":
                    {
                        i++;
                        if (i == argLength || !Enum.TryParse<Role>(args[i], out var role))
                        {
                            DumpHelpWithErrorMessage();
                            return;
                        }
                        Role = role;
                        break;
                    }
                    case "--group-name":
                    {
                        i++;
                        if (i == argLength || args[i].StartsWith('-'))
                        {
                            DumpHelpWithErrorMessage();
                            return;
                        }
                        GroupName = args[i];
                        break;
                    }
                    case "--group-capacity":
                    {
                        i++;
                        if (i == argLength || !int.TryParse(args[i], out var groupCapacity) || groupCapacity <= 0)
                        {
                            DumpHelpWithErrorMessage();
                            return;
                        }
                        GroupCapacity = groupCapacity;
                        break;
                    }
                    case "--space-name":
                    {
                        i++;
                        if (i == argLength || args[i].StartsWith('-'))
                        {
                            DumpHelpWithErrorMessage();
                            return;
                        }
                        SpaceName = args[i];
                        break;
                    }
                    case "--suppress-multiplayer":
                    {
                        SuppressMultiplayer = true;
                        break;
                    }
                    case "--suppress-text-chat":
                    {
                        SuppressTextChat = true;
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
            var helpMessage = string.IsNullOrEmpty(errorMessage) ? string.Empty : errorMessage + Environment.NewLine + Environment.NewLine;
            helpMessage
                += $"Usage: {ExecCommand} [OPTION]..." + Environment.NewLine
                    + Environment.NewLine
                    + "options:" + Environment.NewLine
                    + "  --memory-utilization-dump-file <file>: Gets the memory utilization and dumps to the <file>." + Environment.NewLine
                    + "                                         If not specified, the memory utilization is not measured." + Environment.NewLine
                    + "  --send-message-period <int num>      : The client sends a message once every <int num> seconds." + Environment.NewLine
                    + "                                         If not specified/input 0 or lower, the period is set to 5." + Environment.NewLine
                    + "  --lifetime <float num>               : The performance test will exit in <float num> seconds." + Environment.NewLine
                    + "    (also -l <float num>)                If not specified/input 0 or lower, it does not exit until Ctrl+C is pressed." + Environment.NewLine
                    + "  --role Host/Client                   : Group Role is set to specified role." + Environment.NewLine
                    + "    (also -r Host/Client)                If not specified, the role is set to Host." + Environment.NewLine
                    + "  --group-name <group name>            : The client will join the group named <group name>." + Environment.NewLine
                    + "                                         If not specified, the group name is set to TestGroup." + Environment.NewLine
                    + "  --group-capacity <int num>           : The group capacity is set to <int num>." + Environment.NewLine
                    + "                                         See also \"--space-name\"." + Environment.NewLine
                    + "  --space-name <space name>            : The client will transition to the space named <space name>." + Environment.NewLine
                    + "                                         Even if the role is client, you must specify the space name." + Environment.NewLine
                    + "                                         If the role is host, the client transitions to the <space name>" + Environment.NewLine
                    + "                                         when the num of the clients that joined group is equal to the group capacity." + Environment.NewLine
                    + "                                         See also \"--role\" and \"--space-name\"." + Environment.NewLine
                    + "  --suppress-multiplayer               : The client never moves." + Environment.NewLine
                    + "  --suppress-text-chat                 : The client never sends any messages." + Environment.NewLine
                    + "  --help (also -h)                     : Shows this help messages and exit.";

            Console.Error.WriteLine(helpMessage);
            Application.Quit();
        }
    }
}
