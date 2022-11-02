namespace Extreal.SampleApp.Holiday.Stages.AvatarSelectionScreen
{
    using System;
    using UniRx;
    using UnityEngine;
    using UnityEngine.UI;

    public class AvatarSelectionScreenView : MonoBehaviour
    {
        [SerializeField] private Button goButton;

        public IObservable<Unit> OnGoButtonClicked => goButton.OnClickAsObservable().TakeUntilDestroy(this);
    }
}
