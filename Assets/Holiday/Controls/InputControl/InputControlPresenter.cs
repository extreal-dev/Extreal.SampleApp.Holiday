using System;
using Extreal.Core.StageNavigation;
using Extreal.SampleApp.Holiday.App;
using UniRx;
using VContainer.Unity;

namespace Extreal.SampleApp.Holiday.Controls.InputControl
{
    public class InputControlPresenter : IInitializable, IDisposable
    {
        private readonly StageNavigator<StageName, SceneName> stageNavigator;
        private readonly InputControlView inputControlView;

        private readonly CompositeDisposable disposables = new CompositeDisposable();

        public InputControlPresenter
        (
            StageNavigator<StageName, SceneName> stageNavigator,
            InputControlView inputControlView
        )
        {
            this.stageNavigator = stageNavigator;
            this.inputControlView = inputControlView;
        }

        public void Initialize()
#if UNITY_IOS || UNITY_ANDROID
            => stageNavigator.OnStageTransitioned
                        .Subscribe(OnStageTransitioned)
                        .AddTo(disposables);
#else
        { }
#endif


        public void Dispose()
        {
            disposables.Dispose();
            GC.SuppressFinalize(this);
        }

        private void OnStageTransitioned(StageName stageName)
        {
            if (AppUtils.IsSpace(stageName))
            {
                inputControlView.ShowCanvas();
            }
            else
            {
                inputControlView.HideCanvas();
            }
        }
    }
}
