using Cysharp.Threading.Tasks;
using Extreal.SampleApp.Holiday.Models;
using VContainer.Unity;

namespace Extreal.SampleApp.Holiday.Controls.PlayerControl
{
    public class PlayerPresenter : IStartable
    {
        private readonly Player player;

        public PlayerPresenter(Player player) => this.player = player;

        public void Start() => player.SpawnAsync().Forget();
    }
}
