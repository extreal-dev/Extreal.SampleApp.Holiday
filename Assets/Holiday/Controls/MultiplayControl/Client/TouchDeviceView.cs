using System.Diagnostics.CodeAnalysis;
using Extreal.SampleApp.Holiday.App;
using UnityEngine;

namespace Extreal.SampleApp.Holiday.Controls.MultiplyControl.Client
{
    public class TouchDeviceView : MonoBehaviour
    {
        [SerializeField] private GameObject joysticksCanvas;

        [SuppressMessage("Style", "IDE0051"), SuppressMessage("Style", "IDE0022")]
        private void Awake()
        {
            joysticksCanvas.SetActive(AppUtils.IsTouchDevice());
        }
    }
}
