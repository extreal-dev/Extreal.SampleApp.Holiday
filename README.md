# Extreal.SampleApp.Holiday

## How to play

- This application uses Vivox for voice and text chat. Create an account on [Vivox Developer Portal](https://developer.vivox.com/) and create an application to connect to from this application.
- Clone the repository.
  - If "Link your Unity project" appears, close it without setting it.
- Open the cloned directory in the Unity editor.
- Create a `ChatConfig` from the Create Assets menu in the `/Assets/Holiday/App/Config` directory and set the Vivox access information in the inspector.
  - Path in the Create Assets menu: `Holiday > ChatConfig`
- Create a `MultiplayConfig` from the Create Assets menu in the `/Assets/Holiday/App/Config` directory. Default values are set so there is no need to set them in the inspector.
    - Path in the Create Assets menu: `Holiday > MultiplayConfig`
- Open `/Assets/Holiday/App/App` scene and set `ChatConfig` and `MultiplayConfig` in the `Scope` object inspector.
- Open multiple Unity editors using [ParrelSync.](https://github.com/VeriorPies/ParrelSync). ParrelSync is already installed in this project.
- Run a multiplayer server.
  - Run the following scene: `/Assets/Holiday.MultiplayServer/MultilayServer`
- Run the application.
  - Runs the following scene: `/Assets/Holiday/App/App`
- Enjoy playing!
