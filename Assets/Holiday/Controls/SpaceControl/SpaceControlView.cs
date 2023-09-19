using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Extreal.SampleApp.Holiday.App.AssetWorkflow;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Extreal.SampleApp.Holiday.Controls.SpaceControl
{
    public class SpaceControlView : MonoBehaviour
    {
        [SerializeField] private TMP_Dropdown spaceDropdown;
        [SerializeField] private Button goButton;
        [SerializeField] private Button backButton;
        [SerializeField] private TMP_Text goButtonLabel;
        [SerializeField] private TMP_Text backButtonLabel;

        [Inject] private AssetHelper assetHelper;

        public IObservable<Unit> OnGoButtonClicked
            => goButton.OnClickAsObservable().TakeUntilDestroy(this);

        public IObservable<Unit> OnBackButtonClicked
            => backButton.OnClickAsObservable().TakeUntilDestroy(this);

        public IObservable<string> OnSpaceChanged
            => spaceDropdown.onValueChanged.AsObservable()
            .Select(value => spaceNames[value]).TakeUntilDestroy(this);

        private readonly List<string> spaceNames = new List<string>();

        [SuppressMessage("Usage", "IDE0051")]
        private void Awake()
        {
            goButtonLabel.text = assetHelper.MessageConfig.SpaceGoButtonLabel;
            backButtonLabel.text = assetHelper.MessageConfig.SpaceBackButtonLabel;
        }

        public void Initialize(List<string> spaceNames)
        {
            this.spaceNames.Clear();
            this.spaceNames.AddRange(spaceNames);
            spaceDropdown.options =
                this.spaceNames.Select(spaceName => new TMP_Dropdown.OptionData(spaceName)).ToList();
        }

        public void SetSpaceDropdownValue(string spaceName)
            => spaceDropdown.value = spaceNames.IndexOf(spaceName);
    }
}
