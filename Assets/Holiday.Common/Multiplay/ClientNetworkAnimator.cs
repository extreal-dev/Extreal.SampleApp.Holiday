using Unity.Netcode.Components;

namespace Extreal.SampleApp.Holiday.Common.Multiplay
{
    public class ClientNetworkAnimator : NetworkAnimator
    {
        protected override bool OnIsServerAuthoritative() => false;
    }
}
