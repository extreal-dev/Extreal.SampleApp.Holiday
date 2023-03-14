using System.Diagnostics.CodeAnalysis;
using TMPro;
using UnityEngine;

namespace Extreal.SampleApp.Holiday.Controls.RetryStatusControl
{
    public class RetryStatusControlView : MonoBehaviour
    {
        [SerializeField] private GameObject canvas;
        [SerializeField] private TMP_Text message;

        [SuppressMessage("Usage", "IDE0051")]
        private void Start() => canvas.SetActive(false);

        public void Show(string message)
        {
            this.message.text = message;
            canvas.SetActive(true);
        }

        public void Hide() => canvas.SetActive(false);
    }
}
