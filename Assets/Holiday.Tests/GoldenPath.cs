using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using Extreal.Core.Logging;
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
        private static readonly ELogger Logger = LoggingManager.GetLogger(nameof(GoldenPath));

        [UnityTest]
        public IEnumerator GoldenPathTest() => UniTask.ToCoroutine(async () =>
        {
            // Loads application
            Logger.LogInfo("Loads application");
            await SceneManager.LoadSceneAsync(nameof(App), LoadSceneMode.Additive);
            await UniTask.WaitUntil(() => Object.FindObjectOfType<Button>() != null);
            var titleGoButton = FindObjectOfTypeAndAssert<Button>();
            var title = FindObjectOfTypeWithName<TMP_Text>("Title");

            Assert.That(titleGoButton.gameObject.scene.name, Is.EqualTo(SceneName.TitleScreen.ToString()));
            Assert.That(title.text, Is.EqualTo(nameof(Holiday)));

            // Gets AppState
            Logger.LogInfo("Gets AppState");
            var appScope = FindObjectOfTypeAndAssert<AppScope>();
            var appState = appScope.Container.Resolve(typeof(AppState)) as AppState;
            Assert.That(appState, Is.Not.Null);

            // Starts to download data and enters AvatarSelectionScreen
            Logger.LogInfo("Starts to download data and enters AvatarSelectionScreen");
            titleGoButton.onClick.Invoke();
            await UniTask.WaitUntil(() =>
                ExistButtonOfSceneNamed(SceneName.ConfirmationScreen, SceneName.AvatarSelectionScreen));
            if (ExistButtonOfSceneNamed(SceneName.ConfirmationScreen))
            {
                // Cancels to download AppConfig
                Logger.LogInfo("Cancels to download AppConfig");
                PushButtonNamed("CancelButton");
                await UniTask.Yield();
                Assert.That(ExistButtonOfSceneNamed(SceneName.TitleScreen), Is.True);
                Assert.That(ExistButtonOfSceneNamed(SceneName.ConfirmationScreen), Is.False);

                // Downloads AppConfig
                Logger.LogInfo("Downloads AppConfig");
                titleGoButton.onClick.Invoke();
                await UniTask.WaitUntil(() => ExistButtonOfSceneNamed(SceneName.ConfirmationScreen));
                PushButtonNamed("OkButton");
                await UniTask.Yield();

                var loadedPercent = FindObjectOfTypeWithName<TMP_Text>("LoadedPercent");
                Assert.That(loadedPercent.text, Does.Contain("%"));

                await UniTask.WaitUntil(() => ExistButtonOfSceneNamed(SceneName.AvatarSelectionScreen));
            }

            // Inputs player name
            Logger.LogInfo("Inputs player name");
            var playerNameInputField = FindObjectOfTypeAndAssert<TMP_InputField>();

            playerNameInputField.Select();
            await UniTask.Yield();
            playerNameInputField.text = nameof(GoldenPath);
            EventSystem.current.SetSelectedGameObject(null);
            await UniTask.Yield();

            Assert.That(appState.PlayerName.Value, Is.EqualTo(nameof(GoldenPath)));

            // Selects avatar
            Logger.LogInfo("Selects avatar");
            var avatarDropdown = FindObjectOfTypeAndAssert<TMP_Dropdown>();
            avatarDropdown.value = avatarDropdown.options.Count - 1;
            await UniTask.Yield();

            // Starts to download data and enters VirtualSpace
            Logger.LogInfo("Starts to download data and enters VirtualSpace");
            var avatarSelectionGoButton = FindObjectOfTypeAndAssert<Button>();
            avatarSelectionGoButton.onClick.Invoke();
            await UniTask.WaitUntil(() =>
                ExistButtonOfSceneNamed(SceneName.ConfirmationScreen, SceneName.SpaceControl));
            if (ExistButtonOfSceneNamed(SceneName.ConfirmationScreen))
            {
                // Cancels to download VirtualSpace
                Logger.LogInfo("Cancels to download VirtualSpace");
                PushButtonNamed("CancelButton");
                await UniTask.Yield();
                Assert.That(ExistButtonOfSceneNamed(SceneName.AvatarSelectionScreen), Is.True);
                Assert.That(ExistButtonOfSceneNamed(SceneName.ConfirmationScreen), Is.False);

                // Download VirtualSpace
                Logger.LogInfo("Download VirtualSpace");
                avatarSelectionGoButton.onClick.Invoke();
                await UniTask.WaitUntil(() => ExistButtonOfSceneNamed(SceneName.ConfirmationScreen));
                PushButtonNamed("OkButton");
                await UniTask.Yield();

                var loadedPercent = FindObjectOfTypeWithName<TMP_Text>("LoadedPercent");
                Assert.That(loadedPercent.text, Does.Contain("%"));

                await UniTask.WaitUntil(() => ExistButtonOfSceneNamed(SceneName.SpaceControl));
            }

            // Gets NgoClient
            Logger.LogInfo("Gets NgoClient");
            var appControlScope = FindObjectOfTypeAndAssert<ClientControlScope>();
            var ngoClient = appControlScope.Container.Resolve(typeof(NgoClient)) as NgoClient;
            Assert.That(ngoClient, Is.Not.Null);

            // Waits for ready to play
            Logger.LogInfo("Waits for ready to play");
            await WaitForPlayingReadyAsync(appState, ngoClient);

            // Toggles mute
            Logger.LogInfo("Toggles mute");
            var muteLabel = FindObjectOfTypeWithName<TMP_Text>("MuteLabel");
            Assert.That(muteLabel.text, Is.EqualTo("OFF"));

            PushButtonNamed("MuteButton");
            await UniTask.WaitUntil(() => muteLabel.text != "OFF");
            Assert.That(muteLabel.text, Is.EqualTo("ON"));

            PushButtonNamed("MuteButton");
            await UniTask.WaitUntil(() => muteLabel.text != "ON");
            Assert.That(muteLabel.text, Is.EqualTo("OFF"));

            // Sends a message
            Logger.LogInfo("Sends a message");
            const string message = "Make another player join to the space";
            var messageInput = FindObjectOfTypeAndAssert<TMP_InputField>();
            messageInput.text = message;
            PushButtonNamed("SendButton");

            await UniTask.WaitUntil(() => Object.FindObjectOfType<TextChatMessageView>() != null);

            var messageText = FindObjectOfTypeAndAssert<TextChatMessageView>().GetComponent<TMP_Text>();
            Assert.That(messageText, Is.Not.Null);
            Assert.That(messageText.text, Is.EqualTo(message));

            // Waits for another player's joining
            Logger.LogInfo("Waits for another player's joining");
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

            // Gets player
            Logger.LogInfo("Gets player");
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
            Logger.LogInfo("Moves player");
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
            Logger.LogInfo("Makes player jump");
            var initYPosition = playerInput.transform.position.y;
            playerInput.JumpInput(true);
            await UniTask.Delay(TimeSpan.FromSeconds(0.5));

            Assert.That(playerInput.transform.position.y - initYPosition, Is.Positive);

            // Goes back to AvatarSelectionScreen
            Logger.LogInfo("Goes back to AvatarSelectionScreen");
            PushButtonNamed("SpaceButton");
            await UniTask.WaitUntil(() => ExistButtonOfSceneNamed(SceneName.AvatarSelectionScreen));

            // Waits for not ready to play
            Logger.LogInfo("Waits for not ready to play");
            {
                var playingReady = true;

                using var playingReadyDisposable = appState.PlayingReady
                    .Where(value => !value)
                    .Subscribe(_ => playingReady = false);

                await UniTask.WaitUntil(() => !playingReady);
            }

            // Enters VirtualSpace again
            Logger.LogInfo("Enters VirtualSpace again");
            FindObjectOfTypeAndAssert<Button>().onClick.Invoke();
            await WaitForPlayingReadyAsync(appState, ngoClient);
        });

        private static T FindObjectOfTypeAndAssert<T>() where T : Object
        {
            var obj = Object.FindObjectOfType<T>();
            Assert.That(obj, Is.Not.Null);
            return obj;
        }

        private static T FindObjectOfTypeWithName<T>(string name) where T : Object
        {
            foreach (var obj in Object.FindObjectsOfType<T>())
            {
                if (obj.name == name)
                {
                    return obj;
                }
            }
            Assert.Fail("The object must be obtained");
            return null;
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
