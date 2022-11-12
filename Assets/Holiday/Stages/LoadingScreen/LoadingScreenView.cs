namespace Extreal.SampleApp.Holiday.Stages.LoadingScreen
{
    using UnityEngine;

    public class LoadingScreenView : MonoBehaviour
    {
        [SerializeField] private GameObject screen;

        private void Start() => screen.SetActive(false);

        public void Show() => screen.SetActive(true);

        public void Hide() => screen.SetActive(false);
    }
}
