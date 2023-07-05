using Unity.Netcode.Components;

namespace Extreal.SampleApp.Holiday.Controls.Common.Multiplay
{
    public class ClientNetworkAnimator : NetworkAnimator
    {
        protected override bool OnIsServerAuthoritative() => false;
    }
}
