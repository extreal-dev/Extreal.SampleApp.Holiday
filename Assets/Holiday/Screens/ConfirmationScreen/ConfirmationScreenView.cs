using System;
using System.Diagnostics.CodeAnalysis;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Extreal.SampleApp.Holiday.Screens.ConfirmationScreen
{
    public class ConfirmationScreenView : MonoBehaviour
    {
        [SerializeField] private GameObject screen;
        [SerializeField] private TMP_Text confirmationText;
        [SerializeField] private Button yesButton;
        [SerializeField] private Button noButton;

        public IObservable<Unit> YesButtonClicked
            => yesButton.OnClickAsObservable().TakeUntilDestroy(this);

        public IObservable<Unit> NoButtonClicked
            => noButton.OnClickAsObservable().TakeUntilDestroy(this);

        [SuppressMessage("Style", "IDE0051")]
        private void Start()
            => screen.SetActive(false);

        public void Show(string message)
        {
            confirmationText.text = message;
            screen.SetActive(true);
        }

        public void Hide()
            => screen.SetActive(false);
    }
}
