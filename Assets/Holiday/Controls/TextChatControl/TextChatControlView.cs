using System;
using System.Diagnostics.CodeAnalysis;
using Extreal.SampleApp.Holiday.App.Data;
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

        [Inject] private DataRepository dataRepository;

        public IObservable<string> OnSendButtonClicked => onSendButtonClicked.AddTo(this);
        [SuppressMessage("CodeCracker", "CC0033")]
        private readonly Subject<string> onSendButtonClicked = new Subject<string>();

#pragma warning disable IDE0051
        private void Awake()
        {
            sendButtonLabel.text = dataRepository.AppConfig.TextChatSendButtonLabel;

            sendButton.OnClickAsObservable()
                .TakeUntilDestroy(this)
                .Subscribe(_ =>
                {
                    onSendButtonClicked.OnNext(inputField.text);
                    inputField.text = string.Empty;
                });
        }

        private void OnDestroy()
            => onSendButtonClicked.Dispose();
#pragma warning restore IDE0051

        public void ShowMessage(string message)
            => Instantiate(textChatPrefab, messageRoot)
                .GetComponent<TextChatMessageView>()
                .SetText(message);
    }
}
