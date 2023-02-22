using System;
using System.Diagnostics.CodeAnalysis;
using Extreal.SampleApp.Holiday.App.Data;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Extreal.SampleApp.Holiday.Controls.VirtualSpaceControl
{
    public class VirtualSpaceControlView : MonoBehaviour
    {
        [SerializeField] private Button backButton;
        [SerializeField] private TMP_Text backButtonLabel;

        [Inject] private DataRepository dataRepository;

        public IObservable<Unit> OnBackButtonClicked
            => backButton.OnClickAsObservable().TakeUntilDestroy(this);

        [SuppressMessage("Style", "IDE0051")]
        private void Awake()
            => backButtonLabel.text = dataRepository.AppConfig.VirtualSpaceBackButtonLabel;
    }
}
