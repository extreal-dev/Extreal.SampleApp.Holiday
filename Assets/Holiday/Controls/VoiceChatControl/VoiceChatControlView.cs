using System;
using System.Diagnostics.CodeAnalysis;
using Extreal.SampleApp.Holiday.App.Common;
using Extreal.SampleApp.Holiday.App.Config;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
using VContainer;

namespace Extreal.SampleApp.Holiday.Controls.VoiceChatControl
{
    public class VoiceChatControlView : MonoBehaviour
    {
        [SerializeField] private Button muteButton;
        [SerializeField] private Image muteImage;
        [SerializeField] private TMP_Text mutedString;

        [Inject] private AssetProvider assetProvider;

        public IObservable<Unit> OnMuteButtonClicked
            => muteButton.OnClickAsObservable().TakeUntilDestroy(this);

        private Color mainColor;
        private string muteOffButtonLabel;
        private string muteOnButtonLabel;

        [SuppressMessage("Style", "IDE0051"), SuppressMessage("Style", "CC0061")]
        private void Awake()
        {
            var appConfigRepository = assetProvider.LoadAsset<AppConfigRepository>(nameof(AppConfigRepository));
            var appConfig = appConfigRepository.ToAppConfig();
            Addressables.Release(appConfigRepository);
            muteOffButtonLabel = appConfig.VoiceChatMuteOffButtonLabel;
            muteOnButtonLabel = appConfig.VoiceChatMuteOnButtonLabel;
            mainColor = mutedString.color;
        }

        public void ToggleMute(bool isMute)
        {
            mutedString.text = isMute ? muteOffButtonLabel : muteOnButtonLabel;
            mutedString.color = isMute ? mainColor : Color.white;
            muteImage.color = isMute ? Color.white : mainColor;
        }
    }
}
