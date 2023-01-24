using System.Diagnostics.CodeAnalysis;
using Extreal.Core.Logging;
using Extreal.Core.StageNavigation;
using Extreal.SampleApp.Holiday.App.Config;
using UniRx;
using VContainer.Unity;

namespace Extreal.SampleApp.Holiday.App.Common
{
    public abstract class StagePresenterBase : DisposableBase, IInitializable
    {
        private static readonly ELogger Logger = LoggingManager.GetLogger(nameof(StagePresenterBase));

        private readonly StageNavigator<StageName, SceneName> stageNavigator;

        [SuppressMessage("Usage", "CC0033")]
        private readonly CompositeDisposable sceneDisposables = new CompositeDisposable();

        [SuppressMessage("Usage", "CC0033")]
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

        protected override void FreeManagedResources()
        {
            if (Logger.IsDebug())
            {
                Logger.LogDebug(nameof(FreeManagedResources));
            }

            stageDisposables.Dispose();
            sceneDisposables.Dispose();
        }
    }
}
