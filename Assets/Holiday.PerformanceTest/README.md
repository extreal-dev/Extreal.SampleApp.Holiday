## Performance test (P2P multiplayer)
## Build for performance test
### 1. Add a performance test scene
Have all players operate automatically in the performance test.  
In `Build Settings` - `Scenes in Build`, check `Holiday.PerformanceTest/PerformanceTest` and place it at the top.
### 2. Build Settings
- All Platform
  - It is recommended to turn on Development Build to reduce build time.
- Windows Dedicated Server (load client)
  - Change Project Settings > Player > Other Settings > Optimization > Managed Stripping Level to "Minimal".
  - Add Scripting Define Symbol: 'HOLIDAY_LOAD_CLIENT'.

## Perform performance test
### Prepare
#### 1. Modify parameters in the load client start batch file
1. Modify the [Load Client Start Batch File](.Client/StartPerformanceTest.bat) according to the load conditions.

    | parameters | functions |
    |--|--|
    | exec_time | The startup time of the performance test. Upload results to S3 when this time elapses (=completes) |
    | group_count | Number of groups to use |
    | process_count | Number of processes per Windows server |
    | group_capacity | Capacity of group |

#### 2. Load Client Setup
1. Copy the [.Client](.Client/) folder to the load client PC.
1. Copy all the Windows application files that you have built for the performance test directly under the `.Client` folder.
    - Holiday.exe and StartPerformanceTest.bat exist in the same hierarchy.
  
#### 3. Build application for observation client
1. Modify parameters in PerformanceTestArgumentHandler

    | parameters | functions |
    |--|--|
    | Lifetime | The startup time of the performance test |
    | GroupName | Group name for chat. You MUST set `VirtualSpace` |
    | GroupCapacity | Capacity of group |
    | SpaceName | Space name for multiplayer. You MUST set `VirtualSpace` |

1. Change platform to build.
1. Build.

### Perform for servers
#### 1. Check Server resource usage
1. Run one of the following commands, depending on the server from which you want to retrieve data:
    - `MultiplayServerStartPerformanceTest.sh`.
    - `TextChatServerStartPerformanceTest.sh`.
    - `VoiceChatServerStartPerformanceTest.sh`.

#### 2. Run the Load Client
1. Run the command for including host except including `VirtualSpace` described in one of the following files depending on the server you wish to test on 1 Windows server:
    - `ServerTestForMultiplay.txt`.
    - `ServerTestForTextChat.txt`.
    - `ServerTestForVoiceChat.txt`.
1. Run the command for client only except including `VirtualSpace` described in the file used at 1. on several Windows servers to ensure the required number of load clients per group.
1. Wait until the spike of CPU usage on the Windows server is observed after all load clients start.
    - The logs that appear in the console show how many processes are currently running.
1. Repeat the flow from 1. to 3. for the required number of groups.
    - DO NOT USE the command that includes the same space name before.
    - You can use the command that includes `VirtualSpace` when the last group.

### Perform for servers
#### 1. Check Server resource usage
1. On the each server, run one of the following commands depending on the server from which you want to retrieve data:
    - `MultiplayServerStartPerformanceTest.sh`.
    - `TextChatServerStartPerformanceTest.sh`.
    - `VoiceChatServerStartPerformanceTest.sh`.

#### 2. Check resource usage of observation client
Please check manually.
- For WebGL:
    1. Close all applications and Chrome tabs other than the one you are using.
    1. Start Task Manager and check Google Chrome's CPU and memory from the Processes tab.
    1. Start Resource Monitor and check the sending and receiving of chrome.exe from the network tab.
    1. Run the app.
- For Android:
    1. Open Android Studio and select the process of the running application.
    1. Launch Profiler and check CPU and memory usage.
    1. Use the Network inspector to check network transmission and reception.
- For iPhone:
    1. Open Debug Navigator from Xcode where the build was performed to check CPU, memory usage, and network send/receive usage.
- For PC:
    1. Close all applications.
    1. Start Task Manager and check Application's CPU and memory from the Processes tab.
    1. Start Resource Monitor and check the sending and receiving of Application from the network tab.
    1. run the app.

#### 3. Run the Load Client
1. run `ClientTest.bat` on several Windows servers to ensure the required number of load clients.
