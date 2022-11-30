using UnityEngine;

namespace Extreal.SampleApp.Holiday.Screens.LoadingScreen
{
    public class LoadingScreenView : MonoBehaviour
    {
        [SerializeField] private GameObject screen;

        private void Start() => screen.SetActive(false);

        public void Show() => screen.SetActive(true);

        public void Hide() => screen.SetActive(false);
    }
}
