using System;
using Extreal.SampleApp.Holiday.App.Data;
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

        [Inject] private DataRepository dataRepository;

        public IObservable<Unit> OnMuteButtonClicked
            => muteButton.OnClickAsObservable().TakeUntilDestroy(this);

        private Color mainColor;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0051")]
        private void Awake()
            => mainColor = mutedString.color;

        public void ToggleMute(bool isMute)
        {
            mutedString.text = isMute
                ? dataRepository.AppConfig.VoiceChatMuteOffButtonLabel
                : dataRepository.AppConfig.VoiceChatMuteOnButtonLabel;
            mutedString.color = isMute ? mainColor : Color.white;
            muteImage.color = isMute ? Color.white : mainColor;
        }
    }
}
