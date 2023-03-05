using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Extreal.SampleApp.Holiday.App.Common;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Extreal.SampleApp.Holiday.Screens.AvatarSelectionScreen
{
    public class AvatarSelectionScreenView : MonoBehaviour
    {
        [SerializeField] private TMP_InputField nameInputField;
        [SerializeField] private TMP_Dropdown avatarDropdown;
        [SerializeField] private Button goButton;
        [SerializeField] private TMP_Text title;
        [SerializeField] private TMP_Text goButtonLabel;

        [Inject] private AssetHelper assetHelper;

        private readonly List<string> avatarNames = new List<string>();

        [SuppressMessage("Style", "IDE0051"), SuppressMessage("Style", "CC0061")]
        private void Awake()
        {
            title.text = assetHelper.MessageConfig.AvatarSelectionTitle;
            goButtonLabel.text = assetHelper.MessageConfig.AvatarSelectionGoButtonLabel;
        }

        public void Initialize(List<string> avatarNames)
        {
            this.avatarNames.Clear();
            this.avatarNames.AddRange(avatarNames);
            avatarDropdown.options =
                this.avatarNames.Select(avatarName => new TMP_Dropdown.OptionData(avatarName)).ToList();
        }

        public void SetInitialValues(string name, string avatarName)
        {
            nameInputField.text = name;
            avatarDropdown.value = avatarNames.IndexOf(avatarName);
        }

        public IObservable<string> OnNameChanged =>
            nameInputField.onEndEdit.AsObservable().TakeUntilDestroy(this);

        public IObservable<string> OnAvatarChanged =>
            avatarDropdown.onValueChanged.AsObservable()
                .TakeUntilDestroy(this).Select(index => avatarNames[index]);

        public IObservable<Unit> OnGoButtonClicked => goButton.OnClickAsObservable().TakeUntilDestroy(this);
    }
}
