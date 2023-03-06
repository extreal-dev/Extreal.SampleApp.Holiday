using Cysharp.Threading.Tasks;
using Extreal.Integration.Chat.Vivox;
using VContainer.Unity;

namespace Extreal.SampleApp.Holiday.Controls.ClientControl
{
    public class ClientControlPresenter : IInitializable
    {
        private readonly VivoxClient vivoxClient;

        public ClientControlPresenter(VivoxClient vivoxClient)
            => this.vivoxClient = vivoxClient;

        public void Initialize()
        {
            var authConfig = new VivoxAuthConfig(nameof(Holiday));
            vivoxClient.LoginAsync(authConfig).Forget();
        }
    }
}
