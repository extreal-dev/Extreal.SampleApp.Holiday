using System;
using System.Diagnostics.CodeAnalysis;
using Extreal.Core.Logging;
using Extreal.SampleApp.Holiday.App;
using TMPro;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Extreal.SampleApp.Holiday.Screens.LoadingScreen
{
    public class LoadingScreenView : MonoBehaviour
    {
        private static readonly ELogger Logger = LoggingManager.GetLogger(nameof(LoadingScreenView));

        [SerializeField] private GameObject screen;
        [SerializeField] private TMP_Text loadedPercent;

        [SuppressMessage("Style", "IDE0051")]
        private void Start() => screen.SetActive(false);

        public void SwitchVisibility(bool isVisible)
        {
            if (Logger.IsDebug())
            {
                var status = isVisible ? "ON" : "OFF";
                Logger.LogDebug($"Loading: {status}");
            }
            ClearLoadedPercent();
            screen.SetActive(isVisible);
        }

        public void SetDownloadStatus(DownloadStatus status)
        {
            if (status.IsDone)
            {
                ClearLoadedPercent();
            }
            else
            {
                var total = AppUtils.GetSizeUnit(status.TotalBytes);
                var downloaded = AppUtils.GetSizeUnit(status.DownloadedBytes);
                loadedPercent.text = $"{status.Percent * 100:F0}%" +
                                     Environment.NewLine +
                                     $"({downloaded.Item1}{downloaded.Item2}/{total.Item1}{total.Item2})";
            }
        }

        private void ClearLoadedPercent() => loadedPercent.text = string.Empty;
    }
}
