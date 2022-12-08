using UnityEngine;

namespace Extreal.SampleApp.Holiday.Screens.MultiplayCommon
{
    public class SafeAreaView : MonoBehaviour
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0051")]
        private void Start()
        {
            var safeArea = Screen.safeArea;

            var anchorMin = safeArea.position;
            var anchorMax = safeArea.position + safeArea.size;

            anchorMin.x /= Screen.width;
            anchorMax.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.y /= Screen.height;

            var target = GetComponent<RectTransform>();
            target.anchorMin = anchorMin;
            target.anchorMax = anchorMax;
        }
    }
}
