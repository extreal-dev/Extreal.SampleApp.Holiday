using System;
using Extreal.Integration.Chat.Vivox;
using Extreal.Integration.Multiplay.NGO;
using Cysharp.Threading.Tasks;
using Extreal.SampleApp.Holiday.App.Config;
using Extreal.SampleApp.Holiday.App.Avatars;
using UniRx;
using Object = UnityEngine.Object;
using Extreal.Core.Common.System;
using System.Diagnostics.CodeAnalysis;

namespace Extreal.SampleApp.Holiday.App.Data
{
    public class DataRepository : DisposableBase
    {
        public IObservable<string> OnConfirm => onConfirm;
        [SuppressMessage("CodeCracker", "CC0033")]
        private readonly Subject<string> onConfirm = new Subject<string>();

        public IObservable<string> LoadedPercent => loadedPercent;
        [SuppressMessage("CodeCracker", "CC0033")]
        private readonly Subject<string> loadedPercent = new Subject<string>();

        public IObservable<Unit> OnLoaded => onLoaded;
        [SuppressMessage("CodeCracker", "CC0033")]
        private readonly Subject<Unit> onLoaded = new Subject<Unit>();

        public AppConfig AppConfig { get; private set; }
        public AvatarService AvatarService { get; private set; }
        public VivoxAppConfig VivoxAppConfig { get; private set; }
        public NgoConfig NgoConfig { get; private set; }

        private readonly IDataLoader dataLoader;
        private readonly AppState appState;

        public DataRepository(IDataLoader dataLoader, AppState appState)
        {
            this.dataLoader = dataLoader;
            this.appState = appState;
        }

        protected override void ReleaseManagedResources()
        {
            onConfirm.Dispose();
            onLoaded.Dispose();
            loadedPercent.Dispose();
        }

        public async UniTask GetDownloadSizeAsync()
        {
            appState.SetIsLoading(true);

            var size = (double)await dataLoader.GetDownloadSizeAsync(nameof(AppConfigRepository));

            var prefix = new string[] { string.Empty, "k", "M", "G" };
            var count = 0;
            while (size > 1024d)
            {
                size /= 1024d;
                count++;
            }

            if (size != 0d)
            {
                appState.SetIsLoading(false);
                onConfirm.OnNext
                (
                    $"You will need to download {size:F2} {prefix[count]}B of data." + Environment.NewLine +
                    $"Would you like to download it?"
                );
            }
            else
            {
                LoadAsync(false).Forget();
            }
        }

        public async UniTask LoadAsync(bool withDownload = true)
        {
            appState.SetIsLoading(true);

            if (withDownload)
            {
                using var _ = dataLoader.OnDownloadStatusChanged
                                    .Where(state => state.name == nameof(AppConfigRepository))
                                    .Select(state => (state.downloadStatus.Percent * 100).ToString("F2") + "%")
                                    .Subscribe(loadedPercent.OnNext);
                await dataLoader.DownloadAsync<AppConfigRepository>(nameof(AppConfigRepository));
            }

            var appConfigRepository = await dataLoader.LoadAssetAsync<AppConfigRepository>(nameof(AppConfigRepository));
            AppConfig = appConfigRepository.ToAppConfig();

            var builtinAvatarRepository = await dataLoader.LoadAssetAsync<AvatarRepository>(nameof(AvatarRepository));
            AvatarService = builtinAvatarRepository.ToAvatarService();

            var chatConfig = await dataLoader.LoadAssetAsync<ChatConfig>(nameof(ChatConfig));
            VivoxAppConfig = chatConfig.ToVivoxAppConfig();

            var multiplayConfig = await dataLoader.LoadAssetAsync<MultiplayConfig>(nameof(MultiplayConfig));
            NgoConfig = multiplayConfig.ToNgoConfig();

            dataLoader.ReleaseAssets(
                new Object[] { appConfigRepository, builtinAvatarRepository, chatConfig, multiplayConfig });

            appState.SetIsLoading(false);
            onLoaded.OnNext(Unit.Default);
        }
    }
}
