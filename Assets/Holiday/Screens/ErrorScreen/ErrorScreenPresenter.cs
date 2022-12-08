using System;
using Extreal.SampleApp.Holiday.App;
using UniRx;
using VContainer.Unity;

namespace Extreal.SampleApp.Holiday.Screens.ErrorScreen
{
    public class ErrorScreenPresenter : IInitializable, IDisposable
    {
        private readonly ErrorScreenView errorScreenView;
        private readonly AppState appState;

        private readonly CompositeDisposable disposables = new CompositeDisposable();

        public ErrorScreenPresenter(ErrorScreenView errorScreenView, AppState appState)
        {
            this.errorScreenView = errorScreenView;
            this.appState = appState;
        }

        public void Initialize()
        {
            appState.OnErrorOccurred
                .Where(_ => !appState.IsErrorShowed)
                .Subscribe(message =>
                {
                    appState.SetIsErrorShowed(true);
                    errorScreenView.SetAndShowErrorMessage(message);
                })
                .AddTo(disposables);

            errorScreenView.OnOkButtonClicked
                .Subscribe(_ =>
                {
                    appState.SetIsErrorShowed(false);
                    errorScreenView.HideErrorMessage();
                });
        }

        public void Dispose()
        {
            disposables.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
