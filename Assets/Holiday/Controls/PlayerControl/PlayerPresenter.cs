using Cysharp.Threading.Tasks;
using Extreal.SampleApp.Holiday.Models;
using VContainer;
using VContainer.Unity;

namespace Extreal.SampleApp.Holiday.Holiday.Controls.PlayerControl
{
    public class PlayerPresenter : IStartable
    {
        [Inject] private Player player;

        public void Start()
        {
            player.SpawnAsync().Forget();
        }
    }
}
