using System;
using System.Diagnostics.CodeAnalysis;
using Extreal.SampleApp.Holiday.App.Common;
using Extreal.SampleApp.Holiday.App.Config;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Extreal.SampleApp.Holiday.Controls.TextChatControl
{
    public class TextChatControlView : MonoBehaviour
    {
        [SerializeField] private TMP_InputField inputField;
        [SerializeField] private Button sendButton;
        [SerializeField] private TMP_Text sendButtonLabel;
        [SerializeField] private Transform messageRoot;
        [SerializeField] private GameObject textChatPrefab;

        [Inject] private AssetProvider assetProvider;

        public IObservable<string> OnSendButtonClicked => onSendButtonClicked.AddTo(this);
        [SuppressMessage("CodeCracker", "CC0033")]
        private readonly Subject<string> onSendButtonClicked = new Subject<string>();

        [SuppressMessage("CodeQuality", "IDE0051"), SuppressMessage("Style", "CC0061")]
        private async void Awake()
        {
            var appConfig = (await assetProvider.LoadAssetAsync<AppConfigRepository>(nameof(AppConfigRepository)))
                .ToAppConfig();

            sendButtonLabel.text = appConfig.TextChatSendButtonLabel;

            sendButton.OnClickAsObservable()
                .TakeUntilDestroy(this)
                .Subscribe(_ =>
                {
                    onSendButtonClicked.OnNext(inputField.text);
                    inputField.text = string.Empty;
                });
        }

        [SuppressMessage("CodeQuality", "IDE0051")]
        private void OnDestroy()
            => onSendButtonClicked.Dispose();

        public void ShowMessage(string message)
            => Instantiate(textChatPrefab, messageRoot)
                .GetComponent<TextChatMessageView>()
                .SetText(message);
    }
}
