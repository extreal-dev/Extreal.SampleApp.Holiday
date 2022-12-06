using System;
using UnityEngine;
using UniRx;
using UnityEngine.UI;
using TMPro;

namespace Extreal.SampleApp.Holiday.Screens.VoiceChatScreen
{
    public class VoiceChatScreenView : MonoBehaviour
    {
        [SerializeField] private Button muteButton;
        [SerializeField] private TMP_Text mutedString;

        public IObservable<Unit> OnMuteButtonClicked
            => muteButton.OnClickAsObservable().TakeUntilDestroy(this);

        public void SetMutedString(string value)
            => mutedString.text = value;
    }
}
