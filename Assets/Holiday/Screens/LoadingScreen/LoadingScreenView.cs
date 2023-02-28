using System;
using System.Diagnostics.CodeAnalysis;
using Extreal.SampleApp.Holiday.App;
using TMPro;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

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

        public void SetDownloadStatus(DownloadStatus status)
        {
            var total = AppUtils.GetSizeUnit(status.TotalBytes);
            var downloaded = AppUtils.GetSizeUnit(status.DownloadedBytes);
            loadedPercent.text = $"{status.Percent * 100:F0}%" +
                                 Environment.NewLine +
                                 $"( {downloaded.Item1}{downloaded.Item2} / {total.Item1}{total.Item2} )";
        }
    }
}
