# Extreal.SampleApp.Holiday

## How to play

- Clone the repository.
- Before opening in the Unity Editor, Create the following class in the `Assets/Holiday/App/AssetWorkflow/Custom` directory.

  ```csharp
  namespace Extreal.SampleApp.Holiday.App.AssetWorkflow.Custom
  {
      public static class SecretVariables
      {
          public static string ServerAddress => "test";
          public static string CryptAssetIv => "initialization-vector";
          public static string CryptAssetPassword => "password";
      }
  }
  ```

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
- Open multiple Unity editors using [ParrelSync.](https://github.com/VeriorPies/ParrelSync). ParrelSync is already installed in this project.
- Run a multiplayer server.
  - Run the following scene: `/Assets/Holiday.MultiplayServer/MultilayServer`
- Run the application.
  - Run the following scene: `/Assets/Holiday/App/App`
- Enjoy playing!

## How to visualize application usage

- See [README](Servers/VisualizationOfAppUsage/README.md) to start Grafana/Loki.
- Enable application usage visualization.
  - Turn on the Enable field in AppUsageConfig.
  - `/Assets/Holiday/App/Config/AppUsageConfig`
