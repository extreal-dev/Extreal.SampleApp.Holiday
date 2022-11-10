namespace Extreal.SampleApp.Holiday.Stages.VirtualSpace
{
    using System;
    using UniRx;
    using UnityEngine;
    using UnityEngine.UI;

    public class VirtualSpaceView : MonoBehaviour
    {
        [SerializeField] private Button backButton;

        public IObservable<Unit> OnBackButtonClicked => backButton.OnClickAsObservable().TakeUntilDestroy(this);
    }
}
