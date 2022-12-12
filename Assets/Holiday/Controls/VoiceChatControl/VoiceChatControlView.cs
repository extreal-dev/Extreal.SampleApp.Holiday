using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Extreal.SampleApp.Holiday.Controls.VoiceChatControl
{
    public class VoiceChatControlView : MonoBehaviour
    {
        [SerializeField] private Button muteButton;
        [SerializeField] private TMP_Text mutedString;

        public IObservable<Unit> OnMuteButtonClicked
            => muteButton.OnClickAsObservable().TakeUntilDestroy(this);

        private Color mainColor;

        private void Awake() => mainColor = mutedString.color;

        public void ToggleMute(bool isMute)
        {
            mutedString.text = isMute ? "OFF" : "ON";
            mutedString.color = isMute ? mainColor : Color.white;
            muteButton.GetComponent<Image>().color = isMute ? Color.white : mainColor;
        }
    }
}
