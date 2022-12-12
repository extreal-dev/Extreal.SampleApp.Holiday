using System;
using System.Diagnostics.CodeAnalysis;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Extreal.SampleApp.Holiday.Controls.TextChatControl
{
    public class TextChatControlView : MonoBehaviour
    {
        [SerializeField] private TMP_InputField inputField;
        [SerializeField] private Button sendButton;
        [SerializeField] private Transform messageRoot;
        [SerializeField] private GameObject textChatPrefab;

        public IObservable<string> OnSendButtonClicked => onSendButtonClicked.AddTo(this);
        [SuppressMessage("CodeCracker", "CC0033")]
        private readonly Subject<string> onSendButtonClicked = new Subject<string>();

        private void Awake()
            => sendButton.OnClickAsObservable()
                .TakeUntilDestroy(this)
                .Subscribe(_ =>
                {
                    onSendButtonClicked.OnNext(inputField.text);
                    inputField.text = string.Empty;
                });

        public void ShowMessage(string message)
            => Instantiate(textChatPrefab, messageRoot)
                .GetComponent<TextChatMessageView>()
                .SetText(message);
    }
}
