using UnityEngine;

namespace Extreal.SampleApp.Holiday.Controls.InputControl
{
    public class InputControlView : MonoBehaviour
    {
        [SerializeField] private GameObject canvas4Mobile;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0051")]
        [SerializeField] private GameObject eventSystem4StandAlone;
        [SerializeField] private GameObject eventSystem4Mobile;


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0051")]
        private void Awake()
        {
            canvas4Mobile.SetActive(false);
#if UNITY_IOS || UNITY_ANDROID
            eventSystem4Mobile.SetActive(true);
#else
            eventSystem4StandAlone.SetActive(true);
#endif
        }

        public void ShowCanvas()
            => canvas4Mobile.SetActive(true);

        public void HideCanvas()
            => canvas4Mobile.SetActive(false);
    }
}
