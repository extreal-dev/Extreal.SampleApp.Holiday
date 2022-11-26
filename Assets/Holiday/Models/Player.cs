using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using Cysharp.Threading.Tasks;
using Extreal.Core.Logging;
using UniRx;
using UnityEngine;
using UnityEngine.AddressableAssets;
using VContainer;

namespace Extreal.SampleApp.Holiday.Models
{
    public class Player : MonoBehaviour
    {
        private static readonly ELogger Logger = LoggingManager.GetLogger(nameof(Player));

        public IReadOnlyReactiveProperty<string> Name => name;
        private new readonly ReactiveProperty<string> name = new();

        public IReadOnlyReactiveProperty<Avatar> Avatar => avatar;
        private readonly ReactiveProperty<Avatar> avatar = new();

        public IReadOnlyReactiveProperty<bool> IsPlaying => isPlaying;
        private readonly ReactiveProperty<bool> isPlaying = new();

        private IAvatarRepository avatarRepository;
        public List<Avatar> Avatars { get; private set; }

        public Transform CameraRoot => player.gameObject.transform.Find("PlayerCameraRoot");

        private GameObject player;

        [Inject]
        public void Construct(IAvatarRepository avatarRepository) => this.avatarRepository = avatarRepository;

        private void Awake()
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
                OnDestroy();
            }

            var handle = Addressables.InstantiateAsync(avatar.Value.AssetName);
            player = await handle.Task;

            var playerFollowCamera = FindObjectOfType<CinemachineVirtualCamera>();
            var playerCameraRoot = player.gameObject.transform.Find("PlayerCameraRoot");
            playerFollowCamera.Follow = playerCameraRoot.transform;

            isPlaying.Value = true;
        }

        private void OnDestroy()
        {
            isPlaying.Value = false;

            if (player != null)
            {
                Addressables.ReleaseInstance(player);
            }
        }
    }
}
