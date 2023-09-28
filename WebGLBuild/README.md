# Run WebGL application in local environment

1. Open Addressables Profiles and change to `WebGL`.
1. Open Addressables Groups and check if Builds & Load Paths for all groups is
   set to Remote. If not, change it.
1. Set the build directory to `WebGLBuild/Holiday` directory and build the
   application.
1. Enter the following command in the `WebGLBuild` directory.
1. Put panoramic image/video files in the `WebGLBuild/PanoramicData/Panorama/` directory.

```
deno run --allow-net --allow-read=. index.ts
```
