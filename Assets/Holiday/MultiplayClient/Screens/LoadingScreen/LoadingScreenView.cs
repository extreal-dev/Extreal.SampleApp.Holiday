using UnityEngine;

namespace Extreal.SampleApp.Holiday.MultiplayClient.Screens.LoadingScreen
{
    public class LoadingScreenView : MonoBehaviour
    {
        [SerializeField] private GameObject screen;

        private void Start() => screen.SetActive(false);

        public void Show() => screen.SetActive(true);

        public void Hide() => screen.SetActive(false);
    }
}
