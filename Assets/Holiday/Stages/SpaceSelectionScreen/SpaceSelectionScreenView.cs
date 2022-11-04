namespace Extreal.SampleApp.Holiday.Stages.RoomSelectionScreen
{
    using System;
    using UniRx;
    using UnityEngine;
    using UnityEngine.UI;

    public class SpaceSelectionScreenView : MonoBehaviour
    {
        [SerializeField] private Button goButton;

        public IObservable<Unit> OnGoButtonClicked => goButton.OnClickAsObservable().TakeUntilDestroy(this);
    }
}
