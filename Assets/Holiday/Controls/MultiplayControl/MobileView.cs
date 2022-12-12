using UnityEngine;

namespace Extreal.SampleApp.Holiday.Controls.MultiplayControl
{
    public class MobileView : MonoBehaviour
    {
        [SerializeField] private GameObject joysticksCanvas;

        private void Awake()
        {
            joysticksCanvas.SetActive(false);
#if UNITY_IOS || UNITY_ANDROID
            joysticksCanvas.SetActive(true);
#endif
        }
    }
}
