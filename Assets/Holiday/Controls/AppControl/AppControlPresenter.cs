using Cysharp.Threading.Tasks;
using Extreal.Integration.Chat.Vivox;
using VContainer.Unity;

namespace Extreal.SampleApp.Holiday.Controls.AppControl
{
    public class AppControlPresenter : IInitializable
    {
        private readonly VivoxClient vivoxClient;

        public AppControlPresenter(VivoxClient vivoxClient)
            => this.vivoxClient = vivoxClient;

        public void Initialize()
        {
            var authConfig = new VivoxAuthConfig(nameof(Holiday));
            vivoxClient.LoginAsync(authConfig).Forget();
        }
    }
}
