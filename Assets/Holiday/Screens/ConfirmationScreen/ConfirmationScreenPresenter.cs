using Cysharp.Threading.Tasks;
using Extreal.Core.StageNavigation;
using Extreal.SampleApp.Holiday.App.Common;
using Extreal.SampleApp.Holiday.App.Config;
using Extreal.SampleApp.Holiday.App.Data;
using UniRx;

namespace Extreal.SampleApp.Holiday.Screens.ConfirmationScreen
{
    public class ConfirmationScreenPresenter : StagePresenterBase
    {
        private readonly ConfirmationScreenView confirmationScreenView;
        private readonly DataRepository dataRepository;

        public ConfirmationScreenPresenter
        (
            StageNavigator<StageName, SceneName> stageNavigator,
            ConfirmationScreenView confirmationScreenView,
            DataRepository dataRepository
        ) : base(stageNavigator)
        {
            this.confirmationScreenView = confirmationScreenView;
            this.dataRepository = dataRepository;
        }

        protected override void Initialize(
            StageNavigator<StageName, SceneName> stageNavigator, CompositeDisposable sceneDisposables)
        {
            dataRepository.OnConfirm
                .Subscribe(confirmationScreenView.Show)
                .AddTo(sceneDisposables);

            confirmationScreenView.YesButtonClicked
                .Subscribe(_ =>
                {
                    confirmationScreenView.Hide();
                    dataRepository.LoadAsync().Forget();
                })
                .AddTo(sceneDisposables);

            confirmationScreenView.NoButtonClicked
                .Subscribe(_ => confirmationScreenView.Hide())
                .AddTo(sceneDisposables);
        }

        protected override void OnStageEntered(StageName stageName, CompositeDisposable stageDisposables)
        {
        }

        protected override void OnStageExiting(StageName stageName)
        {
        }
    }
}
