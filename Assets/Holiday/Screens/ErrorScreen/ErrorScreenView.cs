using System;
using System.Diagnostics.CodeAnalysis;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Extreal.SampleApp.Holiday.Screens.ErrorScreen
{
    public class ErrorScreenView : MonoBehaviour
    {
        [SerializeField] private GameObject canvas;
        [SerializeField] private TMP_Text errorMessage;
        [SerializeField] private Button okButton;

        public IObservable<Unit> OnOkButtonClicked => okButton.OnClickAsObservable().TakeUntilDestroy(this);

        [SuppressMessage("Style", "IDE0051")]
        private void Start()
            => canvas.SetActive(false);

        public void SetAndShowErrorMessage(string message)
        {
            errorMessage.text = message;
            canvas.SetActive(true);
        }

        public void HideErrorMessage()
            => canvas.SetActive(false);
    }
}
