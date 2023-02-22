using Extreal.Integration.Chat.Vivox;
using Extreal.Integration.Multiplay.NGO;
using Extreal.SampleApp.Holiday.App.Avatars;
using Extreal.SampleApp.Holiday.App.Data;
using Unity.Netcode;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Extreal.SampleApp.Holiday.Controls.AppControl
{
    public class AppControlScope : LifetimeScope
    {
        [SerializeField] private NetworkManager networkManager;

        protected override void Configure(IContainerBuilder builder)
        {
            var dataRepository = Parent.Container.Resolve<DataRepository>();

            builder.Register<AvatarService>(Lifetime.Singleton);

            builder.RegisterComponent(networkManager);
            builder.Register<NgoClient>(Lifetime.Singleton);

            builder.RegisterComponent(dataRepository.VivoxAppConfig);
            builder.Register<VivoxClient>(Lifetime.Singleton);
        }
    }
}
