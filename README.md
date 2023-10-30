# Extreal.SampleApp.Holiday

## How to play with Unity Editor

- Clone the repository.
- See [README](Servers/P2P/README.md) to start a signaling server for P2P.`
- Open multiple Unity editors using [ParrelSync](https://github.com/VeriorPies/ParrelSync). ParrelSync is already installed in this project.
- Run the application.
  - Run the following scene: `/Assets/Holiday/App/App`
- Enjoy playing!
- To enjoy more play, see [Play with different avatars](#play-with-different-avatars) and [Play in panoramic image/video space](#play-in-panoramic-image/video-space)

## How to play with WebGL

- First, you need to make it playable on the Unity Editor.
- Enter the following command in the `Assets/WebGLScripts` directory.
   ```bash
   $ yarn
   $ yarn dev
   ```
- Open `Build Settings` and change the platform to `WebGL`.
- Select `Holiday` from `Player Settings > Resolution and Presentation > WebGL Template`.
- See [README](Servers/P2P/README.md) to start a signaling server.
- See [README](WebGLBuild/README.md) to complete WebGL setting in the local environment.
- Put panoramic image/video files in the `WebGLBuild/PanoramicData/Panorama/` directory.
  - Set image file name as `PanoramicImageStage.jpg`
  - Set video file name as `PanoramicVideoStage.mp4`
- Enable the panoramic image/video distribution server and WebGL running server locally. 
  - Enter the following command in the `WebGLBuild` directory.
    ```
    deno run --allow-net --allow-read=. index.ts
    ```
- Play by accessing `http://localhost:3333/`.

## How to visualize application usage

- See [README](Servers/AppUsage/README.md) to start Grafana/Loki.
- Enable application usage visualization.
  - Turn on the Enable field in AppUsageConfig.
  - `/Assets/Holiday/App/Config/AppUsageConfig`

## Play with different avatars

- Refer to the following page to import Mixamo model files into your project.
  - [Mixamoの無料3DモデルをUnityにインポートする方法](https://zenn.dev/gaku_moriya/articles/d1b451b288786b)
    - Please implement from "3Dモデルを入手する" to "Materialの最適化".
    - No animation required.
    - Please import "Amy" and "Michelle" from Mixamo into the following path.
      - /Assets/Mixamo/Amy
      - /Assets/Mixamo/Michelle
    - Rename FBX files to their respective avatar names (e.g. Amy.fbx).
- Create avatar prefabs into the `/Assets/Holiday/App/Avatars` directory.
  - Create a new scene.
  - Drag and drop `Amy.fbx`into the scene above and unpack completely.
  - Remove the `Animator` component and rename "Amy" to "AvatarAmy".
  - Attach the `AvatarProvider` component and select `AmyAvatar` as `Avatar`.
  - Drag and drop the `AvatarAmy` GameObject into the `/Assets/Holiday/App/Avatars` directory to create prefab.
  - Remove the scene you just created.
  - Add the `AvatarAmy` asset to the default group of Addressables with the name `AvatarAmy`.
  - Create an avatar prefab about `Michelle` in the same way as above.

## Play in panoramic image/video space

- Put panoramic image/video files in the `WebGLBuild/PanoramicData/Panorama/` directory.
  - Set image file name as `PanoramicImageStage.jpg`
  - Set video file name as `PanoramicVideoStage.mp4`
- Enable the panoramic image/video distribution server.
  - Enter the following command in the `WebGLBuild` directory.
    ```
    deno run --allow-net --allow-read=. index.ts
    ``