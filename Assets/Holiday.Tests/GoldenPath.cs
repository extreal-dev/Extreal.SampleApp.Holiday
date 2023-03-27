using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using Extreal.Integration.Multiplay.NGO;
using Extreal.SampleApp.Holiday.App;
using Extreal.SampleApp.Holiday.App.Config;
using Extreal.SampleApp.Holiday.Controls.ClientControl;
using Extreal.SampleApp.Holiday.Controls.TextChatControl;
using NUnit.Framework;
using StarterAssets;
using TMPro;
using UniRx;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Extreal.SampleApp.Holiday.Tests
{
    public class GoldenPath
    {
        [UnityTest]
        public IEnumerator GoldenPathTest() => UniTask.ToCoroutine(async () =>
        {
            // Loads application
            await SceneManager.LoadSceneAsync(nameof(App), LoadSceneMode.Additive);
            await UniTask.WaitUntil(() => Object.FindObjectOfType<Button>() != null);
            var titleGoButton = FindObjectOfTypeAndAssert<Button>();

            Assert.That(titleGoButton.gameObject.scene.name, Is.EqualTo(SceneName.TitleScreen.ToString()));

            var appScope = FindObjectOfTypeAndAssert<AppScope>();
            var appState = appScope.Container.Resolve(typeof(AppState)) as AppState;
            Assert.That(appState, Is.Not.Null);

            // Starts to download data and enters AvatarSelectionScreen
            titleGoButton.onClick.Invoke();
            await UniTask.WaitUntil(() =>
                ExistButtonOfSceneNamed(SceneName.ConfirmationScreen, SceneName.AvatarSelectionScreen));
            if (ExistButtonOfSceneNamed(SceneName.ConfirmationScreen))
            {
                PushButtonNamed("CancelButton");
                titleGoButton.onClick.Invoke();
                PushButtonNamed("OkButton");
                await UniTask.WaitUntil(() => ExistButtonOfSceneNamed(SceneName.AvatarSelectionScreen));
            }

            // Inputs player name
            var playerNameInputField = FindObjectOfTypeAndAssert<TMP_InputField>();

            playerNameInputField.Select();
            await UniTask.Yield();
            playerNameInputField.text = nameof(GoldenPath);
            EventSystem.current.SetSelectedGameObject(null);
            await UniTask.Yield();

            Assert.That(appState.PlayerName.Value, Is.EqualTo(nameof(GoldenPath)));

            // Selects avatar
            var avatarDropdown = FindObjectOfTypeAndAssert<TMP_Dropdown>();
            avatarDropdown.value = avatarDropdown.options.Count - 1;

            // Starts to download data and enters VirtualSpace
            FindObjectOfTypeAndAssert<Button>().onClick.Invoke();
            await UniTask.WaitUntil(() =>
                ExistButtonOfSceneNamed(SceneName.ConfirmationScreen, SceneName.SpaceControl));
            if (ExistButtonOfSceneNamed(SceneName.ConfirmationScreen))
            {
                PushButtonNamed("OkButton");
                await UniTask.WaitUntil(() => ExistButtonOfSceneNamed(SceneName.SpaceControl));
            }

            var appControlScope = FindObjectOfTypeAndAssert<ClientControlScope>();
            var ngoClient = appControlScope.Container.Resolve(typeof(NgoClient)) as NgoClient;
            Assert.That(ngoClient, Is.Not.Null);

            await WaitForPlayingReadyAsync(appState, ngoClient);

            // Gets player
            var playerInput = default(StarterAssetsInputs);
            foreach (var networkObject in NetworkManager.Singleton.SpawnManager.SpawnedObjects.Values)
            {
                if (networkObject.IsOwner)
                {
                    playerInput = networkObject.GetComponent<StarterAssetsInputs>();
                    break;
                }
            }
            Assert.That(playerInput, Is.Not.Null);

            // Moves player
            const float moveDuration = 1f;
            var moveDirection = Vector2.up;
            var initZPosition = playerInput.transform.position.z;
            var zPositions = new float[2];
            playerInput.MoveInput(moveDirection);
            for (var i = 0; i < 2; i++)
            {
                playerInput.SprintInput(i == 1);
                await UniTask.Delay(TimeSpan.FromSeconds(moveDuration));
                zPositions[i] = playerInput.transform.position.z;
            }
            playerInput.MoveInput(Vector2.zero);
            playerInput.SprintInput(false);

            var moveDistances = new float[] { zPositions[0] - initZPosition, zPositions[1] - zPositions[0] };
            Assert.That(moveDistances[0], Is.Positive);
            Assert.That(moveDistances[1], Is.Positive);
            Assert.That(moveDistances[1] - moveDistances[0], Is.Positive);

            // Makes player jump
            var initYPosition = playerInput.transform.position.y;
            playerInput.JumpInput(true);
            await UniTask.Delay(TimeSpan.FromSeconds(0.5));

            Assert.That(playerInput.transform.position.y - initYPosition, Is.Positive);

            // Unmute
            PushButtonNamed("MuteButton");

            // Sends a message
            const string message = "Make another player join to the space";
            var messageInput = FindObjectOfTypeAndAssert<TMP_InputField>();
            messageInput.text = message;
            PushButtonNamed("SendButton");

            await UniTask.WaitUntil(() => Object.FindObjectOfType<TextChatMessageView>() != null);

            var messageText = FindObjectOfTypeAndAssert<TextChatMessageView>().GetComponent<TMP_Text>();
            Assert.That(messageText, Is.Not.Null);
            Assert.That(messageText.text, Is.EqualTo(message));

            await UniTask.WaitUntil(() =>
            {
                if (Object.FindObjectOfType<TextChatMessageView>() == null)
                {
                    messageInput.text = message;
                    PushButtonNamed("SendButton");
                }
                return NetworkManager.Singleton.SpawnManager.SpawnedObjects.Values.Count > 1;
            });

            await UniTask.WaitUntil(() => Object.FindObjectOfType<TextChatMessageView>() == null);

            // Go back to AvatarSelectionScreen
            PushButtonNamed("SpaceButton");
            await UniTask.WaitUntil(() => ExistButtonOfSceneNamed(SceneName.AvatarSelectionScreen));

            // Waits for not ready to play
            {
                var playingReady = true;

                using var playingReadyDisposable = appState.PlayingReady
                    .Where(value => !value)
                    .Subscribe(_ => playingReady = false);

                await UniTask.WaitUntil(() => !playingReady);
            }

            // Enters VirtualSpace again
            FindObjectOfTypeAndAssert<Button>().onClick.Invoke();
            await WaitForPlayingReadyAsync(appState, ngoClient);
        });

        private static T FindObjectOfTypeAndAssert<T>() where T : Object
        {
            var obj = Object.FindObjectOfType<T>();
            Assert.That(obj, Is.Not.Null);
            return obj;
        }

        private static bool ExistButtonOfSceneNamed(params SceneName[] sceneNames)
        {
            foreach (var button in Object.FindObjectsOfType<Button>())
            {
                foreach (var sceneName in sceneNames)
                {
                    if (button.gameObject.scene.name == sceneName.ToString())
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private static void PushButtonNamed(string buttonName)
        {
            foreach (var button in Object.FindObjectsOfType<Button>())
            {
                if (button.name == buttonName)
                {
                    button.onClick.Invoke();
                    return;
                }
            }
            Assert.Fail("The button must be obtained.");
        }

        private static async UniTask WaitForPlayingReadyAsync(AppState appState, NgoClient ngoClient)
        {
            var playingReady = false;
            var isConnectionApprovalRejected = false;

            using var playingReadyDisposable = appState.PlayingReady
                .Where(value => value)
                .Subscribe(_ => playingReady = true);

            using var isConnectionApprovalRejectedDisposable =
                ngoClient.OnConnectionApprovalRejected
                    .Subscribe(_ => isConnectionApprovalRejected = true);

            await UniTask.WaitUntil(() => playingReady || isConnectionApprovalRejected);

            Assert.That(playingReady, Is.True);
            Assert.That(isConnectionApprovalRejected, Is.False);
        }
    }
}
