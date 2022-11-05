namespace Extreal.SampleApp.Holiday.Stages.LoadingScreen
{
    using UnityEngine;

    public class LoadingScreenView : MonoBehaviour
    {
        [SerializeField] private GameObject screen;

        public void Show() => screen.SetActive(true);

        public void Hide() => screen.SetActive(false);
    }
}
