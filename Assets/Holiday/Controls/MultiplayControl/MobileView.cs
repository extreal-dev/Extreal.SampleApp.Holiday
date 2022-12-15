using UnityEngine;

namespace Extreal.SampleApp.Holiday.Controls.MultiplayControl
{
    public class MobileView : MonoBehaviour
    {
        [SerializeField] private GameObject joysticksCanvas;

#if UNITY_EDITOR || UNITY_STANDALONE
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0051")]
        private void Awake()
            => joysticksCanvas.SetActive(false);
#endif
    }
}
