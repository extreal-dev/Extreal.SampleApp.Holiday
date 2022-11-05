namespace Extreal.SampleApp.Holiday.Stages.LoadingScreen
{
    using System.Collections.Generic;
    using App;
    using UnityEngine;

    public class LoadingScreenView : MonoBehaviour
    {
        [SerializeField] private GameObject screen;

        private readonly HashSet<StageName> stageNamesForLoading = new() { StageName.VirtualSpace };

        private void Start() => screen.SetActive(false);

        public void Show(StageName stageName)
        {
            if (stageNamesForLoading.Contains(stageName))
            {
                screen.SetActive(true);
            }
        }

        public void Hide(StageName stageName)
        {
            if (stageNamesForLoading.Contains(stageName))
            {
                screen.SetActive(false);
            }
        }
    }
}
