using System;
using System.IO;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using Extreal.Core.Logging;
using Extreal.Integration.Multiplay.NGO;
using Extreal.SampleApp.Holiday.App;
using Extreal.SampleApp.Holiday.App.Config;
using Extreal.SampleApp.Holiday.Controls.ClientControl;
using StarterAssets;
using TMPro;
using UniRx;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Extreal.SampleApp.Holiday.PerformanceTest
{
    public class PerformanceTest : MonoBehaviour
    {
        private bool isDestroyed;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeCracker", "CC0033")]
        private readonly CancellationTokenSource cts = new CancellationTokenSource();

        private readonly Vector3 movableRangeMin = new Vector3(-13f, 0f, -3f);
        private readonly Vector3 movableRangeMax = new Vector3(21f, 0f, 30f);

        private readonly string[] messageReparatory = new string[]
        {
            "Hello", "Hello world", "Good morning", "Good afternoon", "Good evening", "Good night",
            "Nice", "Great", "Good", "Cute", "Beautiful", "Wonderful"
        };

        private static ELogger logger;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeCracker", "CC0091")]
        private void Awake()
        {
#if HOLIDAY_PROD
            const Core.Logging.LogLevel logLevel = Core.Logging.LogLevel.Info;
#else
            const Core.Logging.LogLevel logLevel = Core.Logging.LogLevel.Debug;
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
            await UniTask.WaitUntil(() => FindObjectOfType<Button>() != null);

            // Starts to download data and enters AvatarSelectionScreen
            FindObjectOfType<Button>().onClick.Invoke();
            await UniTask.WaitUntil(() =>
                FindObjectOfType<Button>()?.gameObject.scene.name == SceneName.ConfirmationScreen.ToString()
                || FindObjectOfType<Button>()?.gameObject.scene.name == SceneName.AvatarSelectionScreen.ToString());
            if (FindObjectOfType<Button>()?.gameObject.scene.name == SceneName.ConfirmationScreen.ToString())
            {
                foreach (var button in FindObjectsOfType<Button>())
                {
                    if (button.name == "OkButton")
                    {
                        button.onClick.Invoke();
                        break;
                    }
                }
                await UniTask.WaitUntil(() =>
                    FindObjectOfType<Button>()?.gameObject.scene.name == SceneName.AvatarSelectionScreen.ToString());
            }

            // Selects avatar
            await UniTask.Yield();
            var avatarDropdown = FindObjectOfType<TMP_Dropdown>();
            avatarDropdown.value = UnityEngine.Random.Range(0, avatarDropdown.options.Count);

            // Enters VirtualSpace
            FindObjectOfType<Button>().onClick.Invoke();
            await UniTask.WaitUntil(() =>
                            FindObjectOfType<Button>()?.gameObject.scene.name == SceneName.ConfirmationScreen.ToString()
                            || FindObjectOfType<Button>()?.gameObject.scene.name == SceneName.TextChatControl.ToString());
            if (FindObjectOfType<Button>()?.gameObject.scene.name == SceneName.ConfirmationScreen.ToString())
            {
                foreach (var button in FindObjectsOfType<Button>())
                {
                    if (button.name == "OkButton")
                    {
                        button.onClick.Invoke();
                        break;
                    }
                }
                await UniTask.WaitUntil(() =>
                    FindObjectOfType<Button>()?.gameObject.scene.name == SceneName.TextChatControl.ToString());
            }

            var appControlScope = FindObjectOfType<ClientControlScope>();
            var appState = appControlScope.Container.Resolve(typeof(AppState)) as AppState;
            var ngoClient = appControlScope.Container.Resolve(typeof(NgoClient)) as NgoClient;

            {
                var playingReady = false;
                var isConnectionApprovalRejected = false;

                using var isPlayingDisposable = appState.PlayingReady
                    .Skip(1)
                    .Where(value => value)
                    .Subscribe(_ => playingReady = true);

                using var isConnectionApprovalRejectedDisposable =
                    ngoClient.OnConnectionApprovalRejected
                        .Subscribe(_ => isConnectionApprovalRejected = true);

                await UniTask.WaitUntil(() => playingReady || isConnectionApprovalRejected);

                if (isConnectionApprovalRejected)
                {
#if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_STANDALONE
                    Application.Quit();
#endif
                }
            }

            var player = default(NetworkObject);
            foreach (var networkObject in NetworkManager.Singleton.SpawnManager.SpawnedObjects.Values)
            {
                if (networkObject.IsOwner)
                {
                    if (logger.IsDebug())
                    {
                        logger.LogDebug("Get player object");
                    }
                    player = networkObject;
                }
            }
            var playerInput = player.GetComponent<StarterAssetsInputs>();

            var messageInput = FindObjectOfType<TMP_InputField>();
            Button sendButton = null;
            foreach (var button in FindObjectsOfType<Button>())
            {
                if (button.name == "SendButton")
                {
                    sendButton = button;
                }
            }

            var messagePeriod = PerformanceTestArgumentHandler.SendMessagePeriod;
            while (player != null)
            {
                var moveDuration = UnityEngine.Random.Range(1f, 5f);
                var moveDirection = new Vector2(UnityEngine.Random.Range(-1, 2), UnityEngine.Random.Range(-1, 2));
                while (moveDirection == Vector2.zero)
                {
                    moveDirection = new Vector2(UnityEngine.Random.Range(-1, 2), UnityEngine.Random.Range(-1, 2));
                }
                playerInput.SprintInput(UnityEngine.Random.Range(0, 10) < 5);

                if (logger.IsDebug())
                {
                    logger.LogDebug(
                        "move\n"
                        + $" duration: {moveDuration} sec\n"
                        + $" direction: ({moveDirection.x}, {moveDirection.y})\n"
                        + $" isSprint: {playerInput.sprint}");
                }

                for (var t = 0f; t < moveDuration && player != null && InRange(player.transform.position); t += Time.deltaTime)
                {
                    if (UnityEngine.Random.Range(0, 300) == 0)
                    {
                        if (logger.IsDebug())
                        {
                            logger.LogDebug("jump");
                        }
                        playerInput.JumpInput(true);
                    }
                    playerInput.MoveInput(moveDirection);

                    if (UnityEngine.Random.Range(0, messagePeriod) == 1)
                    {
                        var message = messageReparatory[UnityEngine.Random.Range(0, messageReparatory.Length)];
                        messageInput.text = message;
                        sendButton.onClick.Invoke();

                        if (logger.IsDebug())
                        {
                            logger.LogDebug($"Send message: {message}");
                        }
                    }

                    await UniTask.Yield();
                }
                if (player == null)
                {
                    return;
                }

                if (!InRange(player.transform.position))
                {
                    var direction4Zero = new Vector2(-player.transform.position.x, -player.transform.position.z).normalized;
                    playerInput.MoveInput(direction4Zero);
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
