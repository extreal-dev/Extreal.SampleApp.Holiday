namespace Extreal.SampleApp.Holiday.Models
{
    using ScriptableObject;
    using UnityEngine;
    using VContainer;
    using VContainer.Unity;

    public class ModelsScope : LifetimeScope
    {
        [SerializeField] private Player player;
        [SerializeField] private BuiltinAvatarRepository builtinAvatarRepository;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(player);

            builder.RegisterInstance(builtinAvatarRepository).AsImplementedInterfaces();
        }
    }
}
