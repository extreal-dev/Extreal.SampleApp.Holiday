
using UnityEngine;
#if UNITY_STANDALONE
using System.Diagnostics.CodeAnalysis;
#endif

namespace Extreal.SampleApp.Holiday.Controls.MultiplayControl.Client
{
    public class MobileView : MonoBehaviour
    {
        [SerializeField] private GameObject joysticksCanvas;

#if UNITY_STANDALONE
        [SuppressMessage("Style", "IDE0051")]
        private void Awake()
            => joysticksCanvas.SetActive(false);
#endif
    }
}
