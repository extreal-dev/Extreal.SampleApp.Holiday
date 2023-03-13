using System;
using System.Diagnostics.CodeAnalysis;
using Extreal.SampleApp.Holiday.App.AssetWorkflow;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Extreal.SampleApp.Holiday.Controls.VoiceChatControl
{
    public class VoiceChatControlView : MonoBehaviour
    {
        [SerializeField] private Button muteButton;
        [SerializeField] private Image muteImage;
        [SerializeField] private TMP_Text mutedString;

        [Inject] private AssetHelper assetHelper;

        public IObservable<Unit> OnMuteButtonClicked
            => muteButton.OnClickAsObservable().TakeUntilDestroy(this);

        private Color mainColor;
        private string muteOffButtonLabel;
        private string muteOnButtonLabel;

        [SuppressMessage("Style", "IDE0051"), SuppressMessage("Style", "CC0061")]
        private void Awake()
        {
            muteOffButtonLabel = assetHelper.MessageConfig.VoiceChatMuteOffButtonLabel;
            muteOnButtonLabel = assetHelper.MessageConfig.VoiceChatMuteOnButtonLabel;
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
