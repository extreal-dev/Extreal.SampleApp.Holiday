using Cysharp.Threading.Tasks;
using Extreal.Core.StageNavigation;
using Extreal.SampleApp.Holiday.App;
using UniRx;
using VContainer.Unity;
using System;

namespace Extreal.SampleApp.Holiday.Spaces.VirtualSpace
{
    public class VirtualSpacePresenter : IInitializable, IDisposable
    {
        private readonly StageNavigator<StageName, SceneName> stageNavigator;
        private readonly VirtualSpaceView virtualSpaceView;

        private readonly CompositeDisposable disposables = new CompositeDisposable();

        public VirtualSpacePresenter
        (
            StageNavigator<StageName, SceneName> stageNavigator,
            VirtualSpaceView virtualSpaceView
        )
        {
            this.stageNavigator = stageNavigator;
            this.virtualSpaceView = virtualSpaceView;
        }

        public void Initialize()
            => virtualSpaceView.OnBackButtonClicked
                .Subscribe(_ => stageNavigator.ReplaceAsync(StageName.AvatarSelectionStage).Forget())
                .AddTo(disposables);

        public void Dispose()
        {
            disposables.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
