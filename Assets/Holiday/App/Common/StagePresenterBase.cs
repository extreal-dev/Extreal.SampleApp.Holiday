﻿using System;
using Extreal.Core.StageNavigation;
using Extreal.SampleApp.Holiday.App.Config;
using UniRx;
using VContainer.Unity;

namespace Extreal.SampleApp.Holiday.App.Common
{
    public abstract class StagePresenterBase : IInitializable, IDisposable
    {
        private readonly StageNavigator<StageName, SceneName> stageNavigator;
        private readonly CompositeDisposable sceneDisposables = new CompositeDisposable();
        private readonly CompositeDisposable stageDisposables = new CompositeDisposable();

        protected StagePresenterBase(StageNavigator<StageName, SceneName> stageNavigator)
            => this.stageNavigator = stageNavigator;

        public void Initialize()
        {
            stageNavigator.OnStageTransitioned
                .Subscribe(stageName => OnStageEntered(stageName, stageDisposables)).AddTo(sceneDisposables);

            stageNavigator.OnStageTransitioning
                .Subscribe(stageName =>
                {
                    OnStageExiting(stageName);
                    stageDisposables.Clear();
                }).AddTo(sceneDisposables);

            Initialize(stageNavigator, sceneDisposables);
        }

        protected abstract void Initialize(
            StageNavigator<StageName, SceneName> stageNavigator, CompositeDisposable sceneDisposables);

        protected abstract void OnStageEntered(StageName stageName, CompositeDisposable stageDisposables);

        protected abstract void OnStageExiting(StageName stageName);

        public void Dispose()
        {
            stageDisposables.Dispose();
            sceneDisposables.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
