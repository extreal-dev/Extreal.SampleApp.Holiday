## Performance test (P2P multiplayer)
## Build for performance test
### 1. Add a performance test scene
Have all players operate automatically in the performance test.  
In `Build Settings` - `Scenes in Build`, check `Holiday.PerformanceTest/PerformanceTest` and place it at the top.
### 2. Host/Client Selection
In P2P multiplayer, the automatic operation differs between the host and the client.  
To select which one to operate, use `Role` in `Assets/Holiday.PerformanceTest/PerformanceTestConfig`.
### 3. Exclude VoiceChat.(Dedicated Server Build only)
- Remove "Voice Chat Control" from "StageConfig.asset".
- Comment out the following line from "ClientControlScope.cs".
  ````
  var voiceChatClient = VoiceChatClientProvider.Provide(peerClient, assetHelper.VoiceChatConfig);
  builder.RegisterComponent(voiceChatClient);
  ````
### 6. Build Settings
- All Platform
  - It is recommended to turn on Development Build to reduce build time.
- Windows Dedicated Server (load client)
  - Change Project Settings > Player > Other Settings > Optimization > Managed Stripping Level to "Minimal".

## Perform performance test
#### Modify parameters in the load client start batch file
Modify the [Load Client Start Batch File](Assets/Holiday.PerformanceTest/.Client/StartPerformanceTest.bat) according to the load conditions.
|parameters|functions|
|--|--|
|exec_time| the startup time of the performance test. Upload results to S3 when this time elapses (=completes)||
||client_num|Number of load clients to start|
### Load Client Setup
1. copy the [.Client](Assets/Holiday.PerformanceTest/.Client/) folder to the load client PC.
1. Copy all the Windows application file counts that you have built for the performance test directly under the `.Client` folder.
    - Holiday.exe and StartPerformanceTest.bat exist in the same hierarchy.

### Checking Signaling Server resource usage
1. run `SignalingServerStartPerformanceTest.sh`.

### Checking AppUsage Server resource usage
1. run `AppUsageServerStartPerformanceTest.sh`.

### PChecking WebGL app resource usage
Please Check manually.
1. Close all applications and Chrome tabs other than the one you are using.
1. Start Task Manager and check Google Chrome's CPU and memory from the Processes tab.
1. Start Resource Monitor and check the sending and receiving of chrome.exe from the network tab.
1. run the app.

### Running the Load Client
1. run `StartPerformanceTest.bat`.
