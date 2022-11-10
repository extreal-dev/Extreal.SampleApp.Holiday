namespace Extreal.SampleApp.Holiday.Models
{
    using System.Collections.Generic;
    using System.Linq;
    using Cinemachine;
    using Core.Logging;
    using Cysharp.Threading.Tasks;
    using UniRx;
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using VContainer;

    public class Player : MonoBehaviour
    {
        private static readonly ELogger Logger = LoggingManager.GetLogger(nameof(Player));

        public IReadOnlyReactiveProperty<string> Name => name;
        private new readonly ReactiveProperty<string> name = new();

        public IReadOnlyReactiveProperty<Avatar> Avatar => avatar;
        private readonly ReactiveProperty<Avatar> avatar = new();

        [Inject] private IAvatarRepository avatarRepository;
        public List<Avatar> Avatars { get; private set; }

        private GameObject playerAvatar;

        private void Start()
        {
            Avatars = avatarRepository.Avatars;
            name.Value = "Guest";
            avatar.Value = Avatars.First();
        }

        public void SetName(string name) => this.name.Value = name;

        public void SetAvatar(string avatarName) => avatar.Value = Avatars.Find(a => a.Name == avatarName);

        public async UniTask CreateAsync()
        {
            if (Logger.IsDebug())
            {
                Logger.LogDebug($"spawn: name: {name} avatar: {avatar.Value.Name}");
            }

            if (playerAvatar != null)
            {
                OnDestroy();
            }

            var handle = Addressables.InstantiateAsync(avatar.Value.AssetName);
            playerAvatar = await handle.Task;
            SetPlayerCamera();
        }

        private static void SetPlayerCamera()
        {
            var playerFollowCamera = FindObjectOfType<CinemachineVirtualCamera>();
            var playerCameraRoot = GameObject.FindGameObjectWithTag("CinemachineTarget");
            playerFollowCamera.Follow = playerCameraRoot.transform;
        }

        private void OnDestroy()
        {
            if (playerAvatar != null)
            {
                Addressables.ReleaseInstance(playerAvatar);
            }
        }
    }
}
