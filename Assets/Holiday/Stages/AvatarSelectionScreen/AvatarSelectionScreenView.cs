namespace Extreal.SampleApp.Holiday.Stages.AvatarSelectionScreen
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using TMPro;
    using UniRx;
    using UnityEngine;
    using UnityEngine.UI;

    public class AvatarSelectionScreenView : MonoBehaviour
    {
        [SerializeField] private TMP_InputField nameInputField;
        [SerializeField] private TMP_Dropdown avatarDropdown;
        [SerializeField] private Button goButton;

        private readonly List<string> avatars = new List<string>();

        public void Initialize(List<string> avatars)
        {
            this.avatars.Clear();
            this.avatars.AddRange(avatars);
            avatarDropdown.options =
                this.avatars.Select(avatar => new TMP_Dropdown.OptionData(avatar.ToString())).ToList();
        }

        public void SetInitialValues(string name, string avatar)
        {
            nameInputField.text = name;
            avatarDropdown.value = avatars.IndexOf(avatar);
        }

        public IObservable<string> OnNameChanged =>
            nameInputField.onEndEdit.AsObservable().TakeUntilDestroy(this);

        public IObservable<string> OnAvatarChanged =>
            avatarDropdown.onValueChanged.AsObservable()
                .TakeUntilDestroy(this).Select(index => avatars[index].ToString());

        public IObservable<Unit> OnGoButtonClicked => goButton.OnClickAsObservable().TakeUntilDestroy(this);
    }
}
