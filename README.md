# Extreal.SampleApp.Holiday

## How to play

- This application uses Vivox for voice and text chat. Create an account on [Vivox Developer Portal](https://developer.vivox.com/) and create an application to connect to from this application.
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

- Open the cloned directory in the Unity editor.
    - If "Link your Unity project" appears, close it without setting it.
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
- Create a `ChatConfig` from the Create Assets menu in the `/Assets/Holiday/App/Config` directory and set the Vivox access information in the inspector.
  - Path in the Create Assets menu: `Holiday > ChatConfig`
  - Add it to the default group of Addressables with the name `ChatConfig`.
- Create a `MultiplayConfig` from the Create Assets menu in the `/Assets/Holiday/App/Config` directory. Default values are set so there is no need to set them in the inspector.
  - Path in the Create Assets menu: `Holiday > MultiplayConfig`
  - Add it to the default group of Addressables with the name `MultiplayConfig`.
- Create a `AppUsageConfig` from the Create Assets menu in the `/Assets/Holiday/App/Config` directory. Default values are set so there is no need to set them in the inspector.
  - Path in the Create Assets menu: `Holiday > AppUsageConfig`
  - Set `AppUsageConfig` to the Scope object in the App scene from the Inspector.
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
