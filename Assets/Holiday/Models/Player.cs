namespace Extreal.SampleApp.Holiday.Models
{
    using Core.Logging;
    using Cysharp.Threading.Tasks;
    using UniRx;
    using UnityEngine;
    using UnityEngine.AddressableAssets;

    public class Player : MonoBehaviour
    {
        private static readonly ELogger Logger = LoggingManager.GetLogger(nameof(Player));

        private new readonly ReactiveProperty<string> name = new("Guest");
        private readonly ReactiveProperty<AvatarName> avatar = new(AvatarName.Armature);

        public IReadOnlyReactiveProperty<string> Name => name;
        public IReadOnlyReactiveProperty<AvatarName> Avatar => avatar;

        public void SetName(string name) => this.name.Value = name;

        public void SetAvatar(AvatarName avatar) => this.avatar.Value = avatar;

        private GameObject player;
        private GameObject character;

        public async UniTask CreateAsync()
        {
            if (Logger.IsDebug())
            {
                Logger.LogDebug($"spawn: name: {name} avatar: {avatar}");
            }

            if (player != null)
            {
                OnDestroy();
            }

            player = await LoadPlayerAsync();
            character = await LoadCharacterAsync();
            if (character != null)
            {
                player.transform.Find("Geometry").gameObject.SetActive(false);
                character.transform.parent = player.transform;
            }
        }

        private static async UniTask<GameObject> LoadPlayerAsync()
        {
            var handle = Addressables.InstantiateAsync("PlayerPrefab");
            return await handle.Task.ConfigureAwait(false);
        }

        private async UniTask<GameObject> LoadCharacterAsync()
        {
            if (AvatarName.Armature == avatar.Value)
            {
                return null;
            }
            var handle = Addressables.LoadAssetAsync<GameObject>($"Player{avatar.Value}");
            return await handle.Task.ConfigureAwait(false);
        }

        private void OnDestroy()
        {
            if (character != null)
            {
                Addressables.ReleaseInstance(character);
            }
            if (player != null)
            {
                Addressables.ReleaseInstance(player);
            }
        }
    }
}
