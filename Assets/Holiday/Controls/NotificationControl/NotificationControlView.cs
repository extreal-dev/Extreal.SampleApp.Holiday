using System;
using System.Diagnostics.CodeAnalysis;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Extreal.SampleApp.Holiday.Controls.NotificationControl
{
    public class NotificationControlView : MonoBehaviour
    {
        [SerializeField] private GameObject canvas;
        [SerializeField] private TMP_Text message;
        [SerializeField] private Button okButton;
        [SerializeField] private TMP_Text okButtonLabel;

        public IObservable<Unit> OnOkButtonClicked => okButton.OnClickAsObservable().TakeUntilDestroy(this);

        [SuppressMessage("Style", "IDE0051")]
        private void Start() => canvas.SetActive(false);

        public void Initialize(string okButtonLabel)
            => this.okButtonLabel.text = okButtonLabel;

        public void Show(string message)
        {
            this.message.text = message;
            canvas.SetActive(true);
        }

        public void Hide() => canvas.SetActive(false);
    }
}
