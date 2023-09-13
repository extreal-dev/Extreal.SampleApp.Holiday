using Extreal.SampleApp.Holiday.Controls.LandscapeControl.LandscapePlayers;
using Extreal.SampleApp.Holiday.Controls.LandscapeControl.LandscapePlayers.Video;
using Extreal.SampleApp.Holiday.Controls.LandscapeControl.LandscapePlayers.Image;
using Extreal.SampleApp.Holiday.Controls.LandscapeControl.LandscapePlayers.None;
using UnityEngine;
using UnityEngine.Video;
using VContainer;
using VContainer.Unity;

namespace Extreal.SampleApp.Holiday.Controls.LandscapeControl
{
    public class LandscapeControlScope : LifetimeScope
    {
        [SerializeField] private VideoPlayer videoPlayer;
        [SerializeField] private Renderer panoramicRenderer;
        [SerializeField] private LandscapeControlView landscapeControlView;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(videoPlayer);
            builder.RegisterComponent(panoramicRenderer);
            builder.RegisterComponent(landscapeControlView);
            builder.Register<LandscapeVideoPlayerFactory>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<LandscapeImagePlayerFactory>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<LandscapeNonePlayerFactory>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<LandscapePlayerManager>(Lifetime.Singleton);

            builder.RegisterEntryPoint<LandscapeControlPresenter>();
        }
    }
}
