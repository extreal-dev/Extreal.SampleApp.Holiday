using System.Text;
using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Extreal.SampleApp.Holiday.App;
using UniRx;
using Unity.Netcode;
using StarterAssets;
using Extreal.Core.Logging;
using TMPro;
using Extreal.Integration.Multiplay.NGO;
using System.IO;
using Extreal.SampleApp.Holiday.App.Config;
using UnityEngine.Profiling;
using System.Threading;

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

            var appScope = FindObjectOfType<AppScope>();
            var appState = appScope.Container.Resolve(typeof(AppState)) as AppState;
            var isPlaying = false;
            var isPlayingDisposable = appState.IsPlaying
                .Where(value => value)
                .Subscribe(value => isPlaying = value);

            var ngoClient = appScope.Container.Resolve(typeof(NgoClient)) as NgoClient;
            var isConnectionApprovalRejected = false;
            var isConnectionApprovalRejectedDisposable =
                ngoClient.OnConnectionApprovalRejected
                    .Subscribe(_ => isConnectionApprovalRejected = true);

            // Enters AvatarSelectionScreen
            FindObjectOfType<Button>().onClick.Invoke();
            await UniTask.WaitUntil(() =>
                FindObjectOfType<Button>()?.gameObject.scene.name == SceneName.AvatarSelectionScreen.ToString());

            // Enters VirtualSpace
            FindObjectOfType<Button>().onClick.Invoke();
            await UniTask.WaitUntil(() => isPlaying || isConnectionApprovalRejected);
            isPlayingDisposable.Dispose();
            isConnectionApprovalRejectedDisposable.Dispose();

            if (isConnectionApprovalRejected)
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_STANDALONE
                Application.Quit();
#endif
            }

            NetworkObject player = null;
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

                    if (UnityEngine.Random.Range(0, 600) == 0)
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
            var args = Environment.GetCommandLineArgs();
            if (args.Length == 1)
            {
                return;
            }

            var path = args[1];
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
    }
}
