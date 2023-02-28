using Extreal.Integration.Chat.Vivox;
using Extreal.Integration.Multiplay.NGO;
using Extreal.SampleApp.Holiday.App.Avatars;
using Extreal.SampleApp.Holiday.App.Common;
using Extreal.SampleApp.Holiday.App.Config;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AddressableAssets;
using VContainer;
using VContainer.Unity;

namespace Extreal.SampleApp.Holiday.Controls.AppControl
{
    public class AppControlScope : LifetimeScope
    {
        [SerializeField]
        private NetworkManager networkManager;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<AvatarService>(Lifetime.Singleton);

            builder.RegisterComponent(networkManager);
            builder.Register<NgoClient>(Lifetime.Singleton);

            var assetProvider = Parent.Container.Resolve<AssetProvider>();
            var chatConfig = assetProvider.LoadAsset<ChatConfig>(nameof(ChatConfig));
            builder.RegisterComponent(chatConfig.ToVivoxAppConfig());
            Addressables.Release(chatConfig);
            builder.Register<VivoxClient>(Lifetime.Singleton);

            builder.RegisterEntryPoint<AppControlPresenter>();
        }
    }
}
