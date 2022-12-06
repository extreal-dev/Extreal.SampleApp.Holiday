using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Extreal.SampleApp.Holiday.Spaces.VirtualSpace
{
    public class VirtualSpaceView : MonoBehaviour
    {
        [SerializeField] private Button backButton;

        public IObservable<Unit> OnBackButtonClicked
            => backButton.OnClickAsObservable().TakeUntilDestroy(this);
    }
}
