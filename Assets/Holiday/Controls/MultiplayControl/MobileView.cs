using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace Extreal.SampleApp.Holiday.Controls.MultiplayControl
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
