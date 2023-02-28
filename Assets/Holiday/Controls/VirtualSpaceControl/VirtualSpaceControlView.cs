using System;
using System.Diagnostics.CodeAnalysis;
using Extreal.SampleApp.Holiday.App.Common;
using Extreal.SampleApp.Holiday.App.Config;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Extreal.SampleApp.Holiday.Controls.VirtualSpaceControl
{
    public class VirtualSpaceControlView : MonoBehaviour
    {
        [SerializeField] private Button backButton;
        [SerializeField] private TMP_Text backButtonLabel;

        [Inject] private AssetProvider assetProvider;

        public IObservable<Unit> OnBackButtonClicked
            => backButton.OnClickAsObservable().TakeUntilDestroy(this);

        [SuppressMessage("Style", "IDE0051"), SuppressMessage("Style", "CC0061")]
        private async void Awake()
        {
            var appConfig = (await assetProvider.LoadAssetAsync<AppConfigRepository>(nameof(AppConfigRepository))).ToAppConfig();
            backButtonLabel.text = appConfig.VirtualSpaceBackButtonLabel;
        }
    }
}
