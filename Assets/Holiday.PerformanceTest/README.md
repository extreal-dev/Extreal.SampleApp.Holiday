## Performance test (P2P multiplayer)
## Build for performance test
### 1. add a performance test scene
Have all players operate automatically in the performance test.  
In `Build Settings` - `Scenes in Build`, check `Holiday.PerformanceTest/PerformanceTest` and place it at the top.
### 2. Host/Client Selection
In P2P multiplayer, the automatic operation differs between the host and the client.  
To select which one to operate, use `Role` in `Assets/Holiday.PerformanceTest/PerformanceTestConfig`.
### 3. setting the signaling server address
Set the address of the signaling server for P2P connections in P2PConfig > Signaling Url.
### Set the address of the usage visualization server.
Set the address of the usage visualization server in AppUsageConfig > Push Url.
### 5. Build Settings
- WebGL
    - Development Build` is off.
    - `Code Optimization` is set to `Shorter Build Time
- Windows Dedicated Server (load client)
    - Change Project Settings > Player > Other Settings > Optimization > Managed Stripping Level to "Minimal".
    - It is recommended to turn on Development Build to reduce build time.

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
### Running the Load Client
1. run `StartPerformanceTest.bat`.
### Organize performance test results
#### Load client memory usage result measurement tool
- DataAnalysis.py](/Assets/Holiday.PerformanceTest/.Server/DataAnalysis.py)
    - Memory usage is output to a file for each client launched. A tool that aggregates memory max and min across multiple clients.
    - Usage
        - Example: In the following folder state.
      ````
      ├─ DataAnalysis.py
      └──1
         ├─ dev-stress-server1_CpuUtilization.csv
         ├─ dev-stress-server1_MemoryUtilization.csv
         ├─ dev-stress-server1_MemoryUtilization0.txt
         ├─ dev-stress-server1_MemoryUtilization1.txt
         └─ dev-stress-server1_MemoryUtilization2.txt
      ````
        - The execution command is as follows
          ```
          $ python DataAnalysis.py 1/dev-stress-server1_MemoryUtilization\*
          ```
