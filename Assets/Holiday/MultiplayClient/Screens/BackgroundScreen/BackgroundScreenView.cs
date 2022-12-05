using UnityEngine;

namespace Extreal.SampleApp.Holiday.MultiplayClient.Screens.BackgroundScreen
{
    public class BackgroundScreenView : MonoBehaviour
    {
        [SerializeField] private GameObject screen;

        public void Show() => screen.SetActive(true);

        public void Hide() => screen.SetActive(false);
    }
}
