using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using Extreal.Core.Logging;
using Extreal.Integration.Messaging;
using Extreal.Integration.Multiplay.Messaging;
using Extreal.SampleApp.Holiday.App;
using Extreal.SampleApp.Holiday.App.AssetWorkflow;
using Extreal.SampleApp.Holiday.App.Config;
using Extreal.SampleApp.Holiday.Controls.ClientControl;
using Extreal.SampleApp.Holiday.Controls.Common.Multiplay;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Extreal.SampleApp.Holiday.PerformanceTest
{
    public class PerformanceTest : MonoBehaviour
    {
        [SerializeField]
        private PerformanceTestConfig performanceTestConfig;

        private bool isDestroyed;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeCracker", "CC0033")]
        private readonly CancellationTokenSource cts = new CancellationTokenSource();

        private readonly Vector3 movableRangeMin = new Vector3(-13f, 0f, -3f);
        private readonly Vector3 movableRangeMax = new Vector3(21f, 0f, 30f);

        private readonly string[] messageRepertoire = new string[]
        {
            "Hello", "Hello world", "Good morning", "Good afternoon", "Good evening", "Good night",
            "Nice", "Great", "Good", "Cute", "Beautiful", "Wonderful"
        };

        private static ELogger logger;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeCracker", "CC0091")]
        private void Awake()
        {
#if HOLIDAY_PROD
            const LogLevel logLevel = LogLevel.Info;
#else
            const LogLevel logLevel = LogLevel.Debug;
#endif
            LoggingManager.Initialize(logLevel: logLevel);
            logger = LoggingManager.GetLogger(nameof(PerformanceTest));
        }

        private void Start()
        {
            DestroyInLifetimeSecondsAsync().Forget();
            OutputMemoryStatisticsAsync().Forget();
            StartTestAsync().Forget();
        }

        private void OnDestroy()
        {
            isDestroyed = true;
            cts?.Cancel();
            cts?.Dispose();
        }

        private async UniTaskVoid StartTestAsync()
        {
            // Loads application
            await SceneManager.LoadSceneAsync(nameof(App), LoadSceneMode.Additive);
            await UniTask.WaitUntil(() => ExistButtonOfSceneNamed(SceneName.TitleScreen));

            // Starts to download data and enters AvatarSelectionScreen
            PushButtonNamed("GoButton");
            await UniTask.WaitUntil(() =>
                ExistButtonOfSceneNamed(SceneName.ConfirmationScreen, SceneName.AvatarSelectionScreen));
            if (ExistButtonOfSceneNamed(SceneName.ConfirmationScreen))
            {
                PushButtonNamed("OkButton");
                await UniTask.WaitUntil(() => ExistButtonOfSceneNamed(SceneName.AvatarSelectionScreen));
            }

            // Selects avatar
            await UniTask.Yield();
            var avatarDropdown = FindObjectOfType<TMP_Dropdown>();
            avatarDropdown.value = UnityEngine.Random.Range(0, avatarDropdown.options.Count);
            await UniTask.Yield();

            // Enters Group Selection Screen
            PushButtonNamed("ScreenButton");
            await UniTask.WaitUntil(() =>
                ExistButtonOfSceneNamed(SceneName.GroupSelectionScreen));
            await UniTask.Yield();
            var roleDropdown = FindObjectOfType<TMP_Dropdown>();
            roleDropdown.value = (int)PerformanceTestArgumentHandler.Role;

            if (PerformanceTestArgumentHandler.Role == Role.Client)
            {
                await UniTask.Yield();
                var groupDropdown = FindObjectsOfType<TMP_Dropdown>()
                    .First(dropdown => dropdown.name == "GroupDropdown");
                PushButtonNamed("UpdateButton");
                await UniTask.WaitUntil(() => groupDropdown.options.Exists(option => option.text == PerformanceTestArgumentHandler.GroupName));
                await UniTask.Yield();
                groupDropdown.value = groupDropdown.options.FindIndex(option => option.text == PerformanceTestArgumentHandler.GroupName);
                await UniTask.Yield();
            }
            else
            {
                await UniTask.Yield();
                var groupName = FindObjectOfType<TMP_InputField>();
                groupName.text = PerformanceTestArgumentHandler.GroupName;
                groupName.onEndEdit.Invoke(groupName.text);
                await UniTask.Yield();
            }

            // Enters VirtualSpace
            PushButtonNamed("GoButton");
            await UniTask.WaitUntil(() =>
                ExistButtonOfSceneNamed(SceneName.TextChatControl));

            var clientControlScope = FindObjectOfType<ClientControlScope>();
            var appState = clientControlScope.Container.Resolve(typeof(AppState)) as AppState;
            var multiplayClient = clientControlScope.Container.Resolve(typeof(MultiplayClient)) as MultiplayClient;
            var messagingClient = clientControlScope.Container.Resolve(typeof(MessagingClient)) as MessagingClient;
            var assetHelper = clientControlScope.Container.Resolve(typeof(AssetHelper)) as AssetHelper;

            {
                var playingReady = false;
                var isMultiplayJoiningApprovalRejected = false;
                var isMessagingJoiningApprovalRejected = false;

                using var isPlayingDisposable = appState.PlayingReady
                    .Skip(1)
                    .Where(value => value)
                    .Subscribe(_ => playingReady = true);

                using var isMultiplayJoiningApprovalRejectedDisposable =
                    multiplayClient.OnJoiningApprovalRejected
                        .Subscribe(_ => isMultiplayJoiningApprovalRejected = true);

                using var isMessagingJoiningApprovalRejectedDisposable =
                    messagingClient.OnJoiningApprovalRejected
                        .Subscribe(_ => isMessagingJoiningApprovalRejected = true);

                await UniTask.WaitUntil(() => playingReady || isMultiplayJoiningApprovalRejected || isMessagingJoiningApprovalRejected);

                if (!playingReady)
                {
#if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_STANDALONE
                    Application.Quit();
#endif
                }
            }

            if (!string.IsNullOrEmpty(PerformanceTestArgumentHandler.SpaceName))
            {
                var isExistedSpace = assetHelper.SpaceConfig.Spaces.Exists(space => space.SpaceName == PerformanceTestArgumentHandler.SpaceName);
                if (PerformanceTestArgumentHandler.Role == Role.Host)
                {
                    var spaceDropdown = FindObjectOfType<TMP_Dropdown>();
                    if (isExistedSpace)
                    {
                        spaceDropdown.value = spaceDropdown.options.FindIndex(option => option.text == PerformanceTestArgumentHandler.SpaceName);
                        await UniTask.Yield();

                        // Enters specified space
                        await UniTask.WaitUntil(() => multiplayClient.JoinedUsers.Count == PerformanceTestArgumentHandler.GroupCapacity);
                        await UniTask.Delay(assetHelper.PeerConfig.P2PTimeout);
                        PushButtonNamed("GoButton");
                    }
                }

                if (isExistedSpace)
                {
                    var playingReady = false;
                    var isMultiplayJoiningApprovalRejected = false;
                    var isMessagingJoiningApprovalRejected = false;

                    using var isPlayingDisposable = appState.PlayingReady
                        .Skip(1)
                        .Where(value => value)
                        .Subscribe(_ => playingReady = true);

                    using var isMultiplayJoiningApprovalRejectedDisposable =
                        multiplayClient.OnJoiningApprovalRejected
                            .Subscribe(_ => isMultiplayJoiningApprovalRejected = true);

                    using var isMessagingJoiningApprovalRejectedDisposable =
                        messagingClient.OnJoiningApprovalRejected
                            .Subscribe(_ => isMessagingJoiningApprovalRejected = true);

                    await UniTask.WaitUntil(() => playingReady || isMultiplayJoiningApprovalRejected || isMessagingJoiningApprovalRejected);

                    if (!playingReady)
                    {
#if UNITY_EDITOR
                        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_STANDALONE
                        Application.Quit();
#endif
                    }
                }
            }

            if (!PerformanceTestArgumentHandler.SuppressMultiplayer)
            {
                var player = multiplayClient.LocalClient.NetworkObjects[0];
                var playerInput = player.GetComponent<HolidayPlayerInput>();
                RepeatMovePlayerAsync(player, playerInput).Forget();
            }

            if (!PerformanceTestArgumentHandler.SuppressTextChat)
            {
                var messageInput = FindObjectOfType<TMP_InputField>();
                var messagePeriod = PerformanceTestArgumentHandler.SendMessagePeriod;
                RepeatTextMessageSendAsync(messageInput, messagePeriod).Forget();
            }
        }

        private async UniTaskVoid RepeatTextMessageSendAsync(TMP_InputField messageInput, int messagePeriod)
        {
            while (true)
            {
                var message = messageRepertoire[UnityEngine.Random.Range(0, messageRepertoire.Length)];
                messageInput.text = message;
                PushButtonNamed("SendButton");

                if (logger.IsDebug())
                {
                    logger.LogDebug($"Send message: {message}");
                }

                await UniTask.Delay(messagePeriod * 1000);
            }
        }

        private async UniTaskVoid RepeatMovePlayerAsync(GameObject player, HolidayPlayerInput playerInput)
        {
            while (player != null)
            {
                var moveDuration = UnityEngine.Random.Range(1f, 5f);
                var moveDirection = new Vector2(UnityEngine.Random.Range(-1, 2), UnityEngine.Random.Range(-1, 2));
                while (moveDirection == Vector2.zero)
                {
                    moveDirection = new Vector2(UnityEngine.Random.Range(-1, 2), UnityEngine.Random.Range(-1, 2));
                }
                playerInput.SetSprint(UnityEngine.Random.Range(0, 10) < 5);

                if (logger.IsDebug())
                {
                    logger.LogDebug(
                        "move\n"
                        + $" duration: {moveDuration} sec\n"
                        + $" direction: ({moveDirection.x}, {moveDirection.y})\n"
                        + $" isSprint: {playerInput.HolidayValues.Sprint}");
                }

                for (var t = 0f; t < moveDuration && player != null && InRange(player.transform.position); t += Time.deltaTime)
                {
                    if (UnityEngine.Random.Range(0, 300) == 0)
                    {
                        if (logger.IsDebug())
                        {
                            logger.LogDebug("jump");
                        }
                        playerInput.SetJump(true);
                    }
                    playerInput.SetMove(moveDirection);

                    await UniTask.Yield();
                }
                if (player == null)
                {
                    return;
                }

                if (!InRange(player.transform.position))
                {
                    var direction4Zero = new Vector2(-player.transform.position.x, -player.transform.position.z).normalized;
                    playerInput.SetMove(direction4Zero);
                    for (var i = 0; i < 3; i++)
                    {
                        await UniTask.Yield();
                    }
                }
            }
        }

        private bool InRange(Vector3 position)
            => movableRangeMin.x <= position.x && position.x <= movableRangeMax.x
                && movableRangeMin.z <= position.z && position.z <= movableRangeMax.z;

        private static bool ExistButtonOfSceneNamed(params SceneName[] sceneNames)
        {
            foreach (var button in FindObjectsOfType<Button>())
            {
                foreach (var sceneName in sceneNames)
                {
                    if (button.gameObject.scene.name == sceneName.ToString())
                    {
                        if (logger.IsDebug())
                        {
                            logger.LogDebug($"Exist Button {button.gameObject.scene.name}");
                        }
                        return true;
                    }
                }
            }
            return false;
        }

        private static void PushButtonNamed(string name)
        {
            foreach (var button in FindObjectsOfType<Button>())
            {
                if (button.name == name)
                {
                    button.onClick.Invoke();
                    if (logger.IsDebug())
                    {
                        logger.LogDebug($"{button.name} Button Clicked");
                    }
                    return;
                }
            }
        }

        private async UniTaskVoid OutputMemoryStatisticsAsync()
        {
            var path = PerformanceTestArgumentHandler.MemoryUtilizationDumpFile;
            if (string.IsNullOrEmpty(path))
            {
                return;
            }
            if (File.Exists(path))
            {
                if (logger.IsDebug())
                {
                    logger.LogDebug($"There already exists a file at {path}");
                }
                return;
            }

            if (logger.IsDebug())
            {
                logger.LogDebug($"Creates a file {path} and writes data into it");
            }

            var file = File.Create(path);
            var writer = new StreamWriter(file, Encoding.UTF8);
            writer.WriteLine("Date Time TotalReservedMemory TotalAllocatedMemory TotalUnusedReservedMemory");

            while (!isDestroyed)
            {
                var currentTime = DateTime.Now;
                var totalReservedMemory = Profiler.GetTotalReservedMemoryLong();
                var totalAllocatedMemory = Profiler.GetTotalAllocatedMemoryLong();
                var totalUnusedReservedMemory = Profiler.GetTotalUnusedReservedMemoryLong();
                writer.WriteLine($"{currentTime} {totalReservedMemory} {totalAllocatedMemory} {totalUnusedReservedMemory}");

                await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: cts.Token);
            }

            file.Close();
        }

        private static async UniTaskVoid DestroyInLifetimeSecondsAsync()
        {
            if (PerformanceTestArgumentHandler.Lifetime == 0)
            {
                return;
            }

            await UniTask.Delay(TimeSpan.FromSeconds(PerformanceTestArgumentHandler.Lifetime));

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_STANDALONE
            Application.Quit();
#endif
        }
    }
}
