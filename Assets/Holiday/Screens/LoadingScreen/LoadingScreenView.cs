using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace Extreal.SampleApp.Holiday.Screens.LoadingScreen
{
    public class LoadingScreenView : MonoBehaviour
    {
        [SerializeField] private GameObject screen;

        [SuppressMessage("Style", "IDE0051")]
        private void Start() => screen.SetActive(false);

        public void Show() => screen.SetActive(true);

        public void Hide() => screen.SetActive(false);
    }
}
