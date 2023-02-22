using System.Diagnostics.CodeAnalysis;
using TMPro;
using UnityEngine;

namespace Extreal.SampleApp.Holiday.Screens.LoadingScreen
{
    public class LoadingScreenView : MonoBehaviour
    {
        [SerializeField] private GameObject screen;
        [SerializeField] private TMP_Text loadedPercent;

        [SuppressMessage("Style", "IDE0051")]
        private void Start()
            => screen.SetActive(false);

        public void Show()
        {
            loadedPercent.text = string.Empty;
            screen.SetActive(true);
        }

        public void Hide()
            => screen.SetActive(false);

        public void SetLoadedPercent(string percent)
            => loadedPercent.text = percent;
    }
}
