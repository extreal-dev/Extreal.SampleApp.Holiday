﻿using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Extreal.SampleApp.Holiday.Spaces.VirtualSpace
{
    public class VirtualSpaceScope : LifetimeScope
    {
        [SerializeField] private VirtualSpaceView virtualSpaceView;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(virtualSpaceView);

            builder.RegisterEntryPoint<VirtualSpacePresenter>();
        }
    }
}
