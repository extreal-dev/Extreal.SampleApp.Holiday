using Extreal.SampleApp.Holiday.Models.ScriptableObject;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Extreal.SampleApp.Holiday.Models
{
    public class ModelsScope : LifetimeScope
    {
        [SerializeField] private BuiltinAvatarRepository builtinAvatarRepository;
        [SerializeField] private Player player;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(builtinAvatarRepository).AsImplementedInterfaces();
            builder.RegisterComponent(player);
        }
    }
}
