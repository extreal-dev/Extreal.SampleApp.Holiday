using System;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using Cysharp.Threading.Tasks;
using Extreal.Core.Logging;
using UniRx;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Object = UnityEngine.Object;

namespace Extreal.SampleApp.Holiday.Models
{
    public class Player : IDisposable
    {
        private static readonly ELogger Logger = LoggingManager.GetLogger(nameof(Player));

        public IReadOnlyReactiveProperty<string> Name => name;
        private readonly ReactiveProperty<string> name = new ReactiveProperty<string>();

        public IReadOnlyReactiveProperty<Avatar> Avatar => avatar;
        private readonly ReactiveProperty<Avatar> avatar = new ReactiveProperty<Avatar>();

        public IReadOnlyReactiveProperty<bool> IsPlaying => isPlaying;
        private readonly ReactiveProperty<bool> isPlaying = new ReactiveProperty<bool>();

        public List<Avatar> Avatars { get; private set; }

        public Transform CameraRoot => player.gameObject.transform.Find("PlayerCameraRoot");

        private GameObject player;

        public Player(IAvatarRepository avatarRepository)
        {
            Avatars = avatarRepository.Avatars;
            name.Value = "Guest";
            avatar.Value = Avatars.First();
            isPlaying.Value = false;
        }

        public void SetName(string name) => this.name.Value = name;

        public void SetAvatar(string avatarName) => avatar.Value = Avatars.Find(a => a.Name == avatarName);

        public async UniTask SpawnAsync()
        {
            if (Logger.IsDebug())
            {
                Logger.LogDebug($"spawn: name: {name} avatar: {avatar.Value.Name}");
            }

            if (player != null)
            {
                DisposePlayer();
            }

            var handle = Addressables.InstantiateAsync(avatar.Value.AssetName);
            player = await handle.Task;

            var playerFollowCamera = Object.FindObjectOfType<CinemachineVirtualCamera>();
            var playerCameraRoot = player.gameObject.transform.Find("PlayerCameraRoot");
            playerFollowCamera.Follow = playerCameraRoot.transform;

            isPlaying.Value = true;
        }

        private void DisposePlayer()
        {
            isPlaying.Value = false;
            if (player != null)
            {
                Addressables.ReleaseInstance(player);
            }
        }

        public void Dispose()
        {
            name.Dispose();
            avatar.Dispose();
            isPlaying.Dispose();
            DisposePlayer();
            GC.SuppressFinalize(this);
        }
    }
}
