using Cysharp.Threading.Tasks;
using Extreal.Core.StageNavigation;
using Extreal.SampleApp.Holiday.MultiplayClient.App;
using Extreal.SampleApp.Holiday.MultiplayClient.Models;
using UniRx;
using VContainer.Unity;
using System;

namespace Extreal.SampleApp.Holiday.MultiplayClient.Spaces.VirtualSpace
{
    public class VirtualSpacePresenter : IInitializable, IDisposable
    {
        private readonly StageNavigator<StageName, SceneName> stageNavigator;
        private readonly Space space;

        private readonly CompositeDisposable disposables = new CompositeDisposable();

        public VirtualSpacePresenter(StageNavigator<StageName, SceneName> stageNavigator, Space space)
        {
            this.stageNavigator = stageNavigator;
            this.space = space;
        }

        public void Initialize()
            => space.OnDisconnected
                .Subscribe(_ => stageNavigator.ReplaceAsync(StageName.AvatarSelectionStage).Forget())
                .AddTo(disposables);

        public void Dispose()
        {
            disposables.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
